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

[CustomEditor(typeof(MadLevelFreeLayout))]
public class MadLevelFreeLayoutInspector : MadLevelAbstractLayoutInspector {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // won't display offset because it's used to placing icons and it's mostly useless for users
//    SerializedProperty offset;
    
    SerializedProperty backgroundTexture;

    MadLevelFreeLayout script;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected override void OnEnable() {
        base.OnEnable();

        script = target as MadLevelFreeLayout;

        backgroundTexture = serializedObject.FindProperty("backgroundTexture");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();
        
        GUILayout.Label("Fundaments", "HeaderLabel");
        MadGUI.Indent(() => {
            MadGUI.PropertyField(configuration, "Configuration", MadGUI.ObjectIsSet);
            MadGUI.PropertyField(iconTemplate, "Icon Template", MadGUI.ObjectIsSet);
            MadGUI.PropertyField(backgroundTexture, "Background Texture");

            MadGUI.Info("Use the button below if you've updated your icon template and you want to replace all icons in your layout with it.");

            if (MadGUI.Button("Replace All Icons", Color.yellow)) {
                ReplaceAllIcons();
            }

            MadGUI.Info("More customization options are available in the Draggable object.");

            if (MadGUI.Button("Select Draggable", Color.magenta)) {
                SelectDraggable();
            }
        });
        
        GUILayout.Label("Mechanics", "HeaderLabel");
        
        MadGUI.Indent(() => {
            LookAtLastLevel();
            EditorGUILayout.Space();
            HandleMobileBack();
            EditorGUILayout.Space();
            TwoStepActivation();
        });
        
        serializedObject.ApplyModifiedProperties();
    }

    private void SelectDraggable() {
        Selection.activeGameObject = script.draggable.gameObject;
    }

    private void ReplaceAllIcons() {
        if (EditorUtility.DisplayDialog("Replace all icons?",
            "Are you sure that you want to replace all icons with selected prefab?",
            "Replace", "Cancel")) {

                script.ReplaceIcons(script.iconTemplate.gameObject);
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