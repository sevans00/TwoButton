﻿#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#else
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SoundGroupAttribute))]
public class SoundGroupPropertyDrawer : PropertyDrawer {
    public int index;
    public bool typeIn;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!typeIn) {
            return base.GetPropertyHeight(property, label);
		} else {
            return base.GetPropertyHeight(property, label) + 16;
		}
	}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var ma = MasterAudio.Instance;

		if (ma == null) {
			Debug.LogError("No Master Audio prefab in Scene. Cannot use Sound Group Property Drawer.");
			return;
		}

		index = ma.GroupNames.IndexOf(property.stringValue);

        if (typeIn || index == -1) {
            index = 0;
            typeIn = true;
            position.height -= 16;
        }

        index = EditorGUI.Popup(position, label.text, index, MasterAudio.Instance.GroupNames.ToArray());
			
        var groupName = MasterAudio.Instance.GroupNames[index];

        switch (groupName) {
            case "[Type In]":
                typeIn = true;
                position.yMin += 16;
                position.height += 16;
                EditorGUI.BeginChangeCheck();
                property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
                EditorGUI.EndChangeCheck();
                break;
            default:
                typeIn = false;
                property.stringValue = groupName;
                break;
        }
    }
}
#endif