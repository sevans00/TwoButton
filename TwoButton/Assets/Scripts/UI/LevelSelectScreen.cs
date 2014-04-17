using UnityEngine;
using System.Collections;
using MadLevelManager;

public class LevelSelectScreen : MonoBehaviour {

	public void Awake () {

	}

	public void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) { //Back
			GotoWorldSelectScreen();
		}
	}

	public void GotoWorldSelectScreen () {
		MadLevel.LoadLevelByName("World Select Screen");
	}

}
