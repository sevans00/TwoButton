using MadLevelManager;
using UnityEngine;
using System.Collections;

public class InLevelMenu : MonoBehaviour {

	public GameObject inLevelMenu;
	public GameObject characterSelectScreen;

	// Use this for initialization
	void Start () {
		Hide();
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.Escape ) && !MadLevel.currentGroupName.Equals(MadLevel.defaultGroupName) ) {
			if ( inLevelMenu.activeSelf ) {
				if ( characterSelectScreen.activeSelf ) {
					characterSelectScreen.SetActive(false);
				} else {
					inLevelMenu.SetActive(false);
					Game.instance.unpause();
				}
			} else {
				Show();
			}
		}
	}

	public void Show () {
		inLevelMenu.SetActive(true);
		characterSelectScreen.SetActive(false);
		Game.instance.pause();
	}

	public void Hide () {
		inLevelMenu.SetActive(false);
		characterSelectScreen.SetActive(false);
		if ( Game.instance != null ) {
			Game.instance.unpause();
		}
	}

	public void Resume() {
		Hide ();
	}
	public void Restart() {
		Game.instance.RestartLevel();
		Hide ();
	}
	public void ChangeCharacter() {
		inLevelMenu.SetActive(false);
		characterSelectScreen.SetActive(true);
	}
	public void ExitLevel() {
		Hide ();
		string levelName = "World Select";
		levelName = MadLevel.currentGroupName;
		MadLevel.LoadLevelByName(levelName); //Note this is a hack
	}
	public void QuitGame() {
		Application.Quit();
	}




}
