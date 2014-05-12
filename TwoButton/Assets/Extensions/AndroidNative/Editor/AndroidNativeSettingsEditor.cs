
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(AndroidNativeSettings))]
public class AndroidNativeSettingsEditor : Editor {

	public static bool ShowStoreKitParams = false;
	public static bool ShowPSSettings = false;

	
	
	GUIContent GamesApiLabel   = new GUIContent("Enable Games API [?]:", "API used for achivements and leaderboards");
	GUIContent AppSateApiLabel = new GUIContent("Enable App State API [?]:", "API used for cloud data save");

	GUIContent LoadProfileImages   = new GUIContent("Load Profile Images [?]:", "Profile Images will be loaded for all player");
	GUIContent LoadProfileIcons    = new GUIContent("Load Profile Icons [?]:", "Profile Images will be loaded for all player");

	GUIContent Base64KeyLabel = new GUIContent("Base64 Key[?]:", "Base64 Key app key.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");


	private AndroidNativeSettings settings;

	public override void OnInspectorGUI() {
		settings = target as AndroidNativeSettings;

		GUI.changed = false;



		GeneralOptions();
		PlayServiceSettings();
		BillingSettings();


		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}
	}




	private void GeneralOptions() {


		EditorGUILayout.HelpBox("Android Native Plugin Settings", MessageType.None);


		


		EditorGUILayout.Space();

	}

	

	private void PlayServiceSettings() {
		EditorGUILayout.HelpBox("(Optional) PlayService Parameters", MessageType.None);
		ShowPSSettings = EditorGUILayout.Foldout(ShowPSSettings, "PlayService Settings");
		if(ShowPSSettings) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(GamesApiLabel);
			settings.EnableGamesAPI	 	= EditorGUILayout.Toggle(settings.EnableGamesAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(AppSateApiLabel);
			settings.EnableAppStateAPI	 	= EditorGUILayout.Toggle(settings.EnableAppStateAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(LoadProfileIcons);
			settings.LoadProfileIcons	 	= EditorGUILayout.Toggle(settings.LoadProfileIcons);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(LoadProfileImages);
			settings.LoadProfileImages	 	= EditorGUILayout.Toggle(settings.LoadProfileImages);
			EditorGUILayout.EndHorizontal();
		}


	}

	private void BillingSettings() {
		EditorGUILayout.HelpBox("(Optional) In-app Billing Parameters", MessageType.None);
		ShowStoreKitParams = EditorGUILayout.Foldout(ShowStoreKitParams, "Billing Settings");
		if(ShowStoreKitParams) {

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Base64KeyLabel);
			settings.base64EncodedPublicKey	 	= EditorGUILayout.TextField(settings.base64EncodedPublicKey);
			EditorGUILayout.EndHorizontal();


			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No products added", MessageType.Warning);
			}
		

			int i = 0;
			foreach(string str in settings.InAppProducts) {
				EditorGUILayout.BeginHorizontal();
				settings.InAppProducts[i]	 	= EditorGUILayout.TextField(settings.InAppProducts[i]);
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.InAppProducts.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.InAppProducts.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
		}
	}




	private void AboutGUI() {




		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, AndroidNativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(AndroidNativeSettings.Instance);
		#endif
	}
	
	
}
