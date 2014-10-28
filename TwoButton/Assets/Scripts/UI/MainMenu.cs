using MadLevelManager;
using UnityEngine;
using System.Collections;

public class MainMenu : UIMenu {

	public GameObject quitGameDialog;
	public GameObject rootMenu;
	public tk2dUIItem playButton;

	// Use this for initialization
	void Start () {
		HideQuitGame();

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
			if ( quitGameDialog.activeSelf ) {
				HideQuitGame();
			} else {
				ShowQuitGame();
			}
		}
	}


	public void ShowQuitGame() {
		quitGameDialog.SetActive(true);
		rootMenu.SetActive(false);
	}
	
	public void HideQuitGame() {
		quitGameDialog.SetActive(false);
		rootMenu.SetActive(true);
	}

	public void QuitGame() {
		Application.Quit();
	}
	
	public void PlayGame() {
		//Find the furthest unlocked level, or if all completed, world select screen


		string levelToLoad = "World Select Screen";
		Debug.LogWarning("levelToLoad = "+levelToLoad);

		if ( PlayerPrefs.GetInt("First Time", 0) == 0 ) {
			MadLevel.LoadLevelByName("0_IntroComic");
			PlayerPrefs.SetInt("First Time", 1);
		} else {
			if ( levelToLoad != "" ) {
				MadLevel.LoadLevelByName( levelToLoad );
			} else {
				MadLevel.LoadLevelByName("World Select Screen");
			}
		}
	}

	public void WorldSelect () {
		MadLevel.LoadLevelByName("World Select Screen");
	}


	public string lastLevelUnlocked () {
		/*
		string lastCompleted = MadLevel.FindLastCompletedLevelName();
		if ( lastCompleted == null || lastCompleted == "" ) {
			return "World Select Screen";
		}
		return lastCompleted;
		string lastUnlocked = MadLevel.GetNextLevelNameTo(lastCompleted);
		MadLevelConfiguration config = MadLevel.activeConfiguration;
		if ( lastUnlocked == null || lastUnlocked == "" ) {
			Debug.LogWarning("lastUnlocked is null");
			MadLevelConfiguration.Level level = config.FindLevelByName(lastCompleted);
			config.FindNextLevel(lastCompleted);
			//Next level in world 1:
			if ( level.group.name == "World0" ){
				return "2_JumpLevel";
			}
			return "World Select Screen";
		}
		Debug.LogWarning("Last Unlocked: "+lastUnlocked);
		return lastUnlocked;
		//*/

		//Can't rearrange groups nonsense:
		MadLevelConfiguration config = MadLevel.activeConfiguration;
		string lastCompleted = MadLevel.FindLastCompletedLevelName();
		MadLevelConfiguration.Level level1 = config.FindLevelByName(lastCompleted);
		if ( level1 == null ) {
			return "0_IntroComic";
		}
		if ( level1.group.name == "World0" ) {
			MadLevelConfiguration.Level nextingroup = MadLevel.activeConfiguration.FindNextLevel(level1.name, true);
			if ( nextingroup != null ) {
				return nextingroup.name;
			}
		}


		//This works if the level is not world 0:
		MadLevelConfiguration.Level currentLevel = null;
		foreach ( MadLevelConfiguration.Group group in MadLevel.activeConfiguration.groups ) {
			if ( group.name == "World0" ) {
				break;
			}
			if ( group.GetLevels().Count > 0 ) {
				currentLevel = group.GetLevels()[0];
			}
			foreach ( MadLevelConfiguration.Level level in group.GetLevels() ) {
				if ( level.type == MadLevel.Type.Level ) {
					Debug.LogWarning("Checking "+level.name);
					if ( !MadLevelProfile.IsCompleted(level.name) ) {
						return level.name; //Return the last level that's not locked
					}
					currentLevel = level;
				}
			}
		}
		string lastGroup = PlayerPrefs.GetString("LastCompletedGroup","World0");
		return lastGroup;
		return "World Select Screen";
	}
}

