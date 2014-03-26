/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[CustomEditor(typeof(MadLevelIcon))]
public class MadLevelIconInspector : MadSpriteInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    SerializedProperty unlockOnComplete;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    protected new void OnEnable() {
        base.OnEnable();
        
        unlockOnComplete = serializedObject.FindProperty("unlockOnComplete");
    }

    public override void OnInspectorGUI() {
        var levelIcon = target as MadLevelIcon;

        if (levelIcon.generated && SetupMethodGenerate()) {
            if (MadGUI.WarningFix("This icon instance has been generated. If you want to modify this icon, "
                + "please switch your Setup Method to Manual or change the template.", "Help")) {
                    Application.OpenURL(MadLevelHelp.IconGenerated);
            }

            GUI.enabled = false;
        }

        if (MadGameObject.IsActive(levelIcon.gameObject)) {
            Properties();
        } else {
            MadGUI.Warning("Not all functions are available if this object is disabled! Before editing please enable this game object!");
        }
        
        MadGUI.BeginBox("Special Properties");
        MadGUI.Indent(() => {
            if (levelIcon.generated) {
                int levelCount = levelIcon.configuration.LevelCount(MadLevel.Type.Level);
                if (levelCount > levelIcon.levelIndex) {
                    var level = levelIcon.level;
                    if (level != null) {
                        MadGUI.Disabled(() => {
                            EditorGUILayout.TextField("Level Name", level.name);
                            EditorGUILayout.TextField("Level Arguments", level.arguments);
                        });
                    } else {
                        MadGUI.Warning("Level for this icon no longer exists.");
                    }
                }
                if (MadGUI.InfoFix("These values are set and managed by level configuration.",
                    "Configuration")) {
                    Selection.objects = new Object[] { levelIcon.configuration };
                }
            }

            if (levelIcon.completedProperty == null && levelIcon.lockedProperty == null && levelIcon.levelNumber == null) {
                if (MadGUI.WarningFix("You haven't set any special property yet. Are you sure that you don't have any?", "Help")) {
                    Application.OpenURL(MadLevelHelp.SpecialProperty);
                }
            }
        
            //
            // Completed property select popup
            //
            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Completed\" Property",
                ref levelIcon.completedProperty,
                PropertyList(),
                false
            );
            
            MadGUI.PropertyFieldObjectsPopup<MadLevelProperty>(
                target,
                "\"Locked\" Property",
                ref levelIcon.lockedProperty,
                PropertyList(),
                false
            );
        
            MadGUI.PropertyFieldObjectsPopup<MadText>(
                target,
                "Level Number Text",
                ref levelIcon.levelNumber,
                TextList(),
                false
            );

            if (levelIcon.generated) {
                serializedObject.Update();
                if (MadGUI.Foldout("Unlock On Complete", false)) {
                    var arrayList = new MadGUI.ArrayList<MadLevelIcon>(
                        unlockOnComplete, (p) => { MadGUI.PropertyField(p, ""); });
                    arrayList.Draw();
                }
                serializedObject.ApplyModifiedProperties();
            }
        });
        MadGUI.EndBox();
        
        MadGUI.BeginBox("Sprite");
        MadGUI.Indent(() => {
            SectionSprite();
        });
        MadGUI.EndBox();
        
    }

    private void Properties() {
        MadGUI.BeginBox("Properties");
        MadGUI.Indent(() => {

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Create:");

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Empty")) {
                CreateEmptyProperty();
            }
            if (GUILayout.Button("Sprite")) {
                CreateSpriteProperty();
            }
            if (GUILayout.Button("Text")) {
                CreateTextProperty();
            }
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            var properties = PropertyList();
            foreach (MadLevelProperty property in properties) {
                GUILayout.BeginHorizontal();
                MadGUI.LookLikeControls(0, 150);
                property.name = EditorGUILayout.TextField(property.name, GUILayout.Width(120));
                MadGUI.LookLikeControls(0);
                //GUILayout.Label(property.name, GUILayout.Width(170));

                GUILayout.FlexibleSpace();

                GUILayout.Label("Default State: ");
                if (property.linked) {
                    if (MadGUI.Button("LINKED", Color.cyan, GUILayout.Width(60))) {
                        if (EditorUtility.DisplayDialog(
                            "State Linked",
                            "This property state is linked by '" + property.linkage.name
                            + "' property and cannot be changed directly. Do you want to select the linkage owner?",
                            "Yes", "No")) {
                            Selection.activeObject = property.linkage.gameObject;
                        }
                    }
                } else if (property.propertyEnabled) {
                    if (MadGUI.Button("ON", Color.green, GUILayout.Width(60))) {
                        property.propertyEnabled = false;
                        EditorUtility.SetDirty(property);
                    }
                } else {
                    if (MadGUI.Button("OFF", Color.red, GUILayout.Width(60))) {
                        property.propertyEnabled = true;
                        EditorUtility.SetDirty(property);
                    }
                }

                GUILayout.FlexibleSpace();

                GUI.backgroundColor = Color.yellow;

                if (GUILayout.Button("Select", GUILayout.Width(55))) {
                    Selection.activeGameObject = property.gameObject;
                }

                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
        });
        MadGUI.EndBox();
    }

    private bool SetupMethodGenerate() {
        var gridLayout = MadTransform.FindParent<MadLevelGridLayout>(sprite.transform);
        if (gridLayout != null && gridLayout.setupMethod == MadLevelGridLayout.SetupMethod.Generate) {
            return true;
        } else {
            return false;
        }
    }

    private void CreateEmptyProperty() {
        var property = sprite.CreateChild<MadLevelProperty>("new_empty_property");

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        Selection.activeGameObject = property.gameObject;
    }

    private void CreateSpriteProperty() {
        var property = MadTransform.CreateChild<MadLevelProperty>(sprite.transform, "new_sprite_property");
        var s = property.gameObject.AddComponent<MadSprite>();

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        s.ResizeToTexture();
        s.guiDepth = sprite.guiDepth + 1;

        Selection.activeGameObject = s.gameObject;
    }

    private void CreateTextProperty() {
        var property = sprite.CreateChild<MadLevelProperty>("new_text_property");
        var text = property.gameObject.AddComponent<MadText>();

        property.transform.localPosition = Vector3.zero;
        property.transform.localScale = Vector3.one;

        text.text = "Sample Text";
        text.guiDepth = sprite.guiDepth + 1;

        Selection.activeGameObject = text.gameObject;
    }
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    List<MadLevelProperty> PropertyList() {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        return new List<MadLevelProperty>(properties);
    }
    
    List<MadText> TextList() {
        var texts = ((MonoBehaviour) target).GetComponentsInChildren<MadText>();
        return new List<MadText>(texts);
    }
    
    MadLevelProperty GetProperty(string name) {
        var properties = ((MonoBehaviour) target).GetComponentsInChildren<MadLevelProperty>();
        foreach (var property in properties) {
            if (property.name == name) {
                return property;
            }
        }
        
        Debug.LogError("Property " + name + " not found?!");
        return null;
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