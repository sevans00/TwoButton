////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AppPreviewController : MonoBehaviour {

	void OnGUI() {

		if(GUI.Button(new Rect(10, 10, 150, 50), "Billin Preview")) {
			Application.LoadLevel("BillingExample");
		}

		if(GUI.Button(new Rect(10, 80, 150, 50), "Play Service Preview")) {
			Application.LoadLevel("PlayServiceExample");
		}

		if(GUI.Button(new Rect(10, 150, 150, 50), "Facebook")) {
			Application.LoadLevel("FacebookExample");
		}

		if(GUI.Button(new Rect(10, 220, 150, 50), "Twitter")) {
			Application.LoadLevel("TwitterExample");
		}

		if(GUI.Button(new Rect(200, 10, 150, 50), "Ads")) {
			Application.LoadLevel("GoogleAdExample");
		}
	}
}
