using UnityEngine;
using System.Collections;

public class AnOtherFeaturesPreview : MonoBehaviour {

	void OnGUI() {
		if(GUI.Button(new Rect(20, 20, 180, 50), "Enable Immersive Mode")) {
			ImmersiveMode.instance.EnableImmersiveMode();
		}
	}
}
