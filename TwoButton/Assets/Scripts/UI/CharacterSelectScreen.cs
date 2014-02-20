using UnityEngine;
using System.Collections;

/*
 * A quick, hacked together Character select screen:
 */
public class CharacterSelectScreen : MonoBehaviour {

	public static CharacterSelectScreen instance;

	//Initialization
	void Awake () {
		CharacterSelectScreen.instance = this;
	}

	void Start() {
		HideCharacterSelectScreen();
	}

	void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) {
			Application.Quit();
		}
	}
	
	public void ToggleCharacterSelectScreen () {
		if ( gameObject.activeSelf ) {
			HideCharacterSelectScreen();
		} else {
			ShowCharacterSelectScreen();
		}
	}

	public void ShowCharacterSelectScreen () {
		gameObject.SetActive(true);
		Game.instance.pause();
	}
	public void HideCharacterSelectScreen () {
		gameObject.SetActive(false);
		if ( Game.instance != null ) {
			Game.instance.unpause();
		}
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
			currentPlayer.SendMessage("Kill");
		}

		HideCharacterSelectScreen();
	}



}
