using UnityEngine;
using System.Collections;
using MadLevelManager;

public class WorldSelectScreen : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) {
			GotoMainMenu();
		}
	}

	public void GotoMainMenu () {
		MadLevel.LoadLevelByName("Main Menu");
	}

}
