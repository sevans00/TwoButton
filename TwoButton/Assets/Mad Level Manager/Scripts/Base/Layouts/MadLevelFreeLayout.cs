/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif
 
[ExecuteInEditMode]   
public class MadLevelFreeLayout : MadLevelAbstractLayout {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public Vector2 offset = new Vector2(16, -16);
    public Texture2D backgroundTexture;
    
    public MadFreeDraggable draggable {
        get {
            if (_draggable == null) {
                _draggable = MadTransform.GetOrCreateChild<MadFreeDraggable>(transform, "Draggable");
            }

            return _draggable;
        }
    }
    private MadFreeDraggable _draggable;
    
    [HideInInspector]    
    public bool dirty;
    int lastHash;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    public override MadLevelIcon GetIcon(string levelName) {
        MadDebug.Assert(!string.IsNullOrEmpty(levelName), "null or empty level name");
        return MadTransform.FindChild<MadLevelIcon>(draggable.transform, (icon) => {
            return icon.level.name == levelName;
        }, 0);
    }
    
    public override MadLevelIcon FindClosestIcon(Vector3 position) {
        var icons = MadTransform.FindChildren<MadLevelIcon>(draggable.transform, (ic) => true, 0);
        
        float closestsDistance = float.PositiveInfinity;
        MadLevelIcon closestIcon = null;
        
        foreach (var icon in icons) {
            float distance = Vector3.Distance(icon.transform.position, position);
            if (distance < closestsDistance) {
                closestsDistance = distance;
                closestIcon = icon;
            }
        }
        
        return closestIcon;
    }
    
    public override void LookAtIcon(MadLevelIcon icon) {
        LookAtIcon(icon, default(MadiTween.EaseType), 0);
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    #region Public API
    
    public void LookAtIcon(MadLevelIcon icon, MadiTween.EaseType easeType, float time) {
        draggable.MoveToLocal(icon.transform.localPosition, easeType, time);
    }
    
    public void LookAtLevel(string levelName, MadiTween.EaseType easeType, float time) {
        var icon = GetIcon(levelName);
        if (icon != null) {
            LookAtIcon(icon, easeType, time);
        } else {
            Debug.LogError("No icon found for level '" + levelName + "'");
        }
    }

    #endregion

    #region Public Editor API
#if UNITY_EDITOR
    /// <summary>
    /// Will replace all icons in the layout with selected icon. Position, scale and rotation will be preserved.
    /// This method is meant for editor-use only.
    /// </summary>
    /// <param name="newIcon"></param>
    public void ReplaceIcons(GameObject newIcon) {
        MadUndo.LegacyRegisterSceneUndo("Replaced Icons");

        var icons = MadTransform.FindChildren<MadLevelIcon>(draggable.transform);
        var activeIcons = from i in icons where MadGameObject.IsActive(i.gameObject) select i;

        // keep track of unlock on complete settings
        Dictionary<int, List<int>> unlockOnCompleteDict = new Dictionary<int, List<int>>();
        List<MadLevelIcon> createdIcons = new List<MadLevelIcon>();

        foreach (var icon in activeIcons) {
            var position = icon.transform.position;
            var rotation = icon.transform.rotation;
            var localScale = icon.transform.localScale;
            var name = icon.name;
            var baseDepth = icon.guiDepth;
            var levelIndex = icon.levelIndex;
            var configuration = icon.configuration;

            // build unlock on complete dict
            List<int> toUnlockList = new List<int>();
            foreach (var unlock in icon.unlockOnComplete) {
                toUnlockList.Add(unlock.levelIndex);
            }
            unlockOnCompleteDict[icon.levelIndex] = toUnlockList;


            MadUndo.DestroyObjectImmediate(icon.gameObject);

            var go = MadTransform.CreateChild(draggable.transform, name, newIcon);
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.localScale = localScale;
            var nIcon = go.GetComponent<MadLevelIcon>();
            nIcon.guiDepth = baseDepth;
            nIcon.levelIndex = levelIndex;
            nIcon.configuration = configuration;
            nIcon.hasLevelConfiguration = true;

            createdIcons.Add(nIcon);
            

            var childSprites = MadTransform.FindChildren<MadSprite>(go.transform);
            foreach (var cs in childSprites) {
                cs.guiDepth += baseDepth;
            }

            MadUndo.RegisterCreatedObjectUndo(go.gameObject, "Replaced Icons");
        }

        // apply unlock on complete list
        foreach (var icon in createdIcons) {
            List<int> unlockList = unlockOnCompleteDict[icon.levelIndex];
            foreach (var unlockLevelIndex in unlockList) {
                var query = from i in createdIcons where i.levelIndex == unlockLevelIndex select i;
                var iconToUnlock = query.First();
                icon.unlockOnComplete.Add(iconToUnlock);
            }
        }
    }
#endif
    #endregion

    protected override void OnEnable() {
        base.OnEnable();

        configuration.callbackChanged = () => {
            if (this != null) {
                Build();
            }
        };
        
        if (IsDirty()) {
            Build();
        }
    }

    protected override void Update() {
        base.Update();
    
        if (IsDirty()) {
            Build();
        }
    }
    
    bool IsDirty() {
        if (dirty) {
            dirty = false;
            return true;
        }
        
        if (configuration == null || iconTemplate == null) {
            return false;
        }
        
        int hash = 11;
        hash = hash * 37 + configuration.GetHashCode();
        hash = hash * 37 + iconTemplate.GetHashCode();
        hash = hash * 37 + (backgroundTexture != null ? backgroundTexture.GetHashCode() : 0);
        
        if (hash != lastHash) {
            lastHash = hash;
            return true;
        }
        
        return false;
    }
    
    void Build() {
        int levelCount = configuration.LevelCount(MadLevel.Type.Level, configurationGroup);
        Vector2 currentOffset = Vector2.zero;
        
        MadLevelIcon previousIcon = null;
        
        // find out min and max depth
        int min, max;
        iconTemplate.MinMaxDepthRecursively(out min, out max);
        int depthDiff = (max - min) + 1;
        
        const string name = "level {0:D3}";
        
        for (int levelIndex = 0; levelIndex < levelCount; ++levelIndex) {
            MadLevelIcon levelIcon = MadTransform.FindChild<MadLevelIcon>(
                    draggable.transform, (ic) => ic.levelIndex == levelIndex, 0);
            bool newInstance = levelIcon == null;
            
            // create new icon instance if it's not exists
            if (newInstance) {
                levelIcon = MadTransform.CreateChild(
                    draggable.transform, string.Format(name, levelIndex + 1), iconTemplate);
                    
                // adjust gui depth for each icon
                levelIcon.guiDepth += levelIndex * depthDiff;
                var sprites = MadTransform.FindChildren<MadSprite>(levelIcon.transform);
                foreach (var sprite in sprites) {
                    sprite.guiDepth += levelIndex * depthDiff;
                }
                
                // position & scale
                levelIcon.pivotPoint = MadSprite.PivotPoint.Center;
                levelIcon.transform.localPosition = currentOffset;
                currentOffset += offset;
                    
                levelIcon.transform.localScale = Vector3.one;
            }
            
            // make it active if deactivated
            if (!MadGameObject.IsActive(levelIcon.gameObject)) {
                MadGameObject.SetActive(levelIcon.gameObject, true);
            }
            
            // setup level properties
            levelIcon.levelGroup = configurationGroup;
            levelIcon.levelIndex = levelIndex;
            levelIcon.configuration = configuration;
            levelIcon.hasLevelConfiguration = true;
            
            // set level number if exists
            if (levelIcon.levelNumber != null) {
                levelIcon.levelNumber.text = (levelIndex + 1).ToString();
            }
            
            // level unlock if set
            if (previousIcon != null) {
                if (newInstance) {
                    previousIcon.unlockOnComplete.Add(levelIcon);
                }
            } else {
                levelIcon.locked = false;
            }
            
            previousIcon = levelIcon;
        }
        
        BuildBackgroundTexture();
    }
    
    void BuildBackgroundTexture() {
        if (backgroundTexture != null) {
            var background = MadTransform.GetOrCreateChild<MadSprite>(draggable.transform, "background");
            background.texture = backgroundTexture;
            background.guiDepth = -1;
        } else {
            var background = MadTransform.FindChildWithName<MadSprite>(draggable.transform, "background");
            if (background != null) {
                DestroyImmediate(background.gameObject);
            }
        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif