using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {
	
	// Update is called once per frame
	void OnGUI () {
		Color c = GUI.color;
		GUI.color = Color.green;
		if(GUI.Button(new Rect(10, 400, 150, 50), "Back")) {
			
			Application.LoadLevel ("Preview");
			
		}
		
		GUI.color = c;
	}
}
