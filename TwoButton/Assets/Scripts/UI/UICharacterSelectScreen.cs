using UnityEngine;
using System.Collections;

/*
 * A quick, hacked together Character select screen:
 */
public class UICharacterSelectScreen : UIMenu {

	public static UICharacterSelectScreen instance;

	public void Awake () {
		UICharacterSelectScreen.instance = this;
	}

	public void SelectNewCharacter( GameObject newJumperPrefab ) {
		//Set the new character:
		if ( newJumperPrefab == null ) {
			return;
		}
		Game.instance.JumperPrefab = newJumperPrefab;

		//Kill the old character:
		GameObject currentPlayer = GameObject.FindWithTag("Player");
		if ( currentPlayer != null ) {
			Destroy(currentPlayer.gameObject);
			Game.instance.GameOver();
		}


	}

}
