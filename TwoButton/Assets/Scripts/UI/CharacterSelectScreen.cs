using UnityEngine;
using System.Collections;

/*
 * A quick, hacked together Character select screen:
 */
public class CharacterSelectScreen : MonoBehaviour {

	public static CharacterSelectScreen instance;
	public InLevelMenu inLevelMenu;

	//Initialization
	void Awake () {
		CharacterSelectScreen.instance = this;
	}

	void Start() {
		//HideCharacterSelectScreen();
	}

	void Update () {

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
	}
	public void HideCharacterSelectScreen () {
		gameObject.SetActive(false);
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

		inLevelMenu.Hide();
	}



}
