/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MadBigMeshRenderer))]
[RequireComponent(typeof(MadMaterialStore))]
public class MadPanel : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    private static List<MadPanel> panels = new List<MadPanel>();
    
    public bool halfPixelOffset = true;
    
    [NonSerialized]
    public HashSet<MadSprite> sprites = new HashSet<MadSprite>();
    
    public MadMaterialStore materialStore {
        get {
            if (_materialStore == null) {
                _materialStore = gameObject.AddComponent<MadMaterialStore>();
            }
            
            return _materialStore;
        }
        
        private set {
            _materialStore = value;
        }
    }
    private MadMaterialStore _materialStore;
    
    [HideInInspector]
    MadSprite _focusedSprite;
    int _focusedSpriteModCount;
    public MadSprite focusedSprite {
        get { return _focusedSprite; }
        set {
            _focusedSprite = value;
            _focusedSpriteModCount++;
            if (onFocusChanged != null) {
                onFocusChanged(_focusedSprite);
            }
        }
    }

    Camera currentCamera {
        get {
            if (_currentCamera == null || (_currentCamera.cullingMask & (1 << gameObject.layer)) == 0) {
                _currentCamera = null;

                Camera[] cameras = FindObjectsOfType(typeof(Camera)) as Camera[];

                for (int i = 0; i < cameras.Length; ++i) {
                    var camera = cameras[i];
                    if ((camera.cullingMask & (1 << gameObject.layer)) != 0) {
                        if (_currentCamera != null) {
                            Debug.Log("There are multiple cameras that are rendering the \""
                                + LayerMask.LayerToName(gameObject.layer)
                                + "\" layer. Please adjust your culling masks and/or change layer of this Panel object.", this);
                        } else {
                            _currentCamera = camera;
                        }
                    }
                }
            }

            return _currentCamera;
        }
    }

    private Camera _currentCamera;
    
    // input helpers
    HashSet<MadSprite> hoverSprites = new HashSet<MadSprite>();

    // set of sprites that has been clicked or touched. When mouse button or finger is up,
    // and sprite still resists in here, it may be treated as "pressed"
    HashSet<MadSprite> touchDownSprites = new HashSet<MadSprite>();
    
    // ===========================================================
    // Events
    // ===========================================================
    
    public delegate void Event1<T>(T t);
    
    /// <summary>
    /// Occurs when on focus changed. Passes focused sprite or null if nothing is currently focused.
    /// </summary>
    public event Event1<MadSprite> onFocusChanged;

    // ===========================================================
    // Methods
    // ===========================================================

    void Awake() {
        panels.Add(this);
    }

    void OnDestroy() {
        panels.Remove(this);
    }
    
    void OnEnable() {
        materialStore = GetComponent<MadMaterialStore>();
        
        var meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null) {
            MadGameObject.SafeDestroy(meshRenderer);
        }

        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null) {
            MadGameObject.SafeDestroy(meshFilter);
        }

    }

    void Update() {
        // fix the offset
        if (halfPixelOffset) {
            var root = FindParent<MadRootNode>();
            float pixelSize = root.pixelSize;
            
            float x = 0, y = 0;
            if (Screen.height % 2 == 0) {
                y = pixelSize;
            }
            
            if (Screen.width % 2 == 0) {
                x = pixelSize;
            }
            
            MadTransform.SetLocalPosition(transform, new Vector3(x, y, 0));
        }
        
        UpdateInput();
    }
    
    void UpdateInput() {
#if UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8 || UNITY_BLACKBERRY
        UpdateTouchInput();
        if (Application.isEditor) {
            UpdateMouseInput();
        }
#else
        UpdateMouseInput();
#endif
    }
    
    void UpdateTouchInput() {
        var touches = Input.touches;
        foreach (var touch in touches) {

            var sprites = AllSpritesForScreenPoint(touch.position);
            foreach (var sprite in sprites) {

                if (touch.phase == TouchPhase.Began) {
                    touchDownSprites.Add(sprite);
                } else if (touch.phase == TouchPhase.Ended && touchDownSprites.Contains(sprite)) {
                    sprite.onTap(sprite);
                    sprite.TryFocus();
                } else {
                    // will remove sprite from mouse down if dragging is registered
                    if (IsDragging(sprite)) {
                        touchDownSprites.Remove(sprite);
                    }
                }
            }
        }
    }

    void UpdateMouseInput() {
        var sprites = new HashSet<MadSprite>(AllSpritesForScreenPoint(Input.mousePosition));

        foreach (var sprite in sprites) {
            // find newly hovered sprites
            if (hoverSprites.Add(sprite)) {
                sprite.onMouseEnter(sprite);
            }

            // check if any of the sprites have registered draggable which is in dragging state
            // if so, it should be removed from mouseDownSprites
            if (IsDragging(sprite)) {
                touchDownSprites.Remove(sprite);
            }
        }

        // find sprites that are no longer hovered
        if (sprites.Count != hoverSprites.Count) {
            List<MadSprite> unhovered = new List<MadSprite>();
            foreach (var hoverSprite in hoverSprites) {
                if (!sprites.Contains(hoverSprite)) {
                    unhovered.Add(hoverSprite);
                    hoverSprite.onMouseExit(hoverSprite);
                }
            }
            
            foreach (var u in unhovered) {
                hoverSprites.Remove(u);
            }
        }
        
        if (Input.GetMouseButtonDown(0)) {
            foreach (var sprite in hoverSprites) {
                sprite.onMouseDown(sprite);
                touchDownSprites.Add(sprite);
            }
        }
        
        if (Input.GetMouseButtonUp(0)) {
            int modCount = _focusedSpriteModCount;
            foreach (var mouseDownSprite in touchDownSprites) {
                if (sprites.Contains(mouseDownSprite)) {
                    mouseDownSprite.onMouseUp(mouseDownSprite);
                    mouseDownSprite.TryFocus();
                }
            }
            
            touchDownSprites.Clear();
            
            if (modCount == _focusedSpriteModCount && focusedSprite != null) {
                // focus lost to nothing
                focusedSprite.hasFocus = false;
            }
        }
    }

    bool IsDragging(MadSprite sprite) {
        var dragHandler = sprite.FindParent<MadDraggable>();
        if (dragHandler != null) {
            if (dragHandler.dragging) {
                return true;
            }
        }

        return false;
    }
    
    IEnumerable<MadSprite> AllSpritesForScreenPoint(Vector2 point) {
        var ray = currentCamera.ScreenPointToRay(point);
        RaycastHit[] hits = Physics.RaycastAll(ray, 4);
        foreach (var hit in hits) {
            var collider = hit.collider;
            var sprite = collider.GetComponent<MadSprite>();
            if (sprite != null && sprite.panel == this) {
                yield return sprite;
            }
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    public static MadPanel FirstOrNull(Transform currentTransform) {
        // first try to find panel as parent
        if (currentTransform != null) {
            var panel = MadTransform.FindParent<MadPanel>(currentTransform);
            if (panel != null) {
                return panel;
            }
        }

        // then try to locate the panel on the static list
        if (panels.Count > 0) {
            return panels[0];
        }

        // if all above fails, try to locate panel using the slow FindObjectOfType method
        return GameObject.FindObjectOfType(typeof(MadPanel)) as MadPanel;
    }

    public static MadPanel UniqueOrNull() {
        if (MadPanel.panels.Count == 1) {
            return MadPanel.panels[0];
        }

        var panels = GameObject.FindObjectsOfType(typeof(MadPanel));
        if (panels.Length == 1) {
            return panels[0] as MadPanel;
        } else {
            return null;
        }
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif