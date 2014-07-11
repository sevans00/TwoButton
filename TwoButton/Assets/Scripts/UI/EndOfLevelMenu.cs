using UnityEngine;
using System.Collections;
using MadLevelManager;


//Menu for the end of level - should display player stats for the level as well as elapsed time, best time, and three buttons:
// - Next level
// - Retry level
// - Back to level select screen
public class EndOfLevelMenu : MonoBehaviour {

	public void Start () {
		Hide ();
	}

	//Stars, times, and ads to go here
	public tk2dTextMesh timeText;
	public tk2dTextMesh bestTimeText;
	public tk2dTextMesh bestTimeNewText;
	public GameObject star1;
	public GameObject star2;
	public GameObject star3;

	
	//Populate the fields
	public void populateFields () {
		float timeElapsed = Game.instance.timeElapsed;
		string formattedTime = Game.instance.elapsedTimeTextMesh.text;
		string prevFormattedTime = MadLevelProfile.GetLevelString(MadLevel.currentLevelName, "time", formattedTime);
		prevFormattedTime = string.Format("{0:0.00}", prevFormattedTime);
		float prevTime = float.Parse(prevFormattedTime);

		if ( timeElapsed <= prevTime ) {
			bestTimeNewText.gameObject.SetActive(true);
			MadLevelProfile.SetLevelString(MadLevel.currentLevelName, "time", formattedTime);
		} else {
			bestTimeNewText.gameObject.SetActive(false);
		}

		timeText.text = formattedTime;
		timeText.Commit();
		bestTimeText.text = prevFormattedTime;
		bestTimeText.Commit();
		star1.SetActive( MadLevelProfile.GetLevelBoolean(MadLevel.currentLevelName, "star_1", false) );
		star2.SetActive( MadLevelProfile.GetLevelBoolean(MadLevel.currentLevelName, "star_2", false) );
		star3.SetActive( MadLevelProfile.GetLevelBoolean(MadLevel.currentLevelName, "star_3", false) );
	}


	//Button callbacks:
	public void NextLevel () {
		Hide ();
		if ( MadLevel.HasNextInGroup(MadLevel.Type.Level) ) {
			MadLevel.LoadNextInGroup(MadLevel.Type.Level);
		} else {
			MadLevel.LoadLevelByName(MadLevel.currentGroupName);
		}
		Game.instance.unpause();
	}

	public void RetryLevel () {
		Hide ();
		GameObject currentPlayer = GameObject.FindWithTag("Player");
		if ( currentPlayer != null ) {
			Destroy(currentPlayer);
		}
		Game.instance.unpause();
		Game.instance.ResetLevel();
		Game.instance.spawnPlayer();	
	}

	public void BackToLevelSelect () {
		Hide ();
		Game.instance.unpause();
		MadLevel.LoadLevelByName(MadLevel.currentGroupName);
	}






	public void Hide() {
		gameObject.SetActive(false);
	}
	public void Show() {
		//TODO: Populate the fields
		populateFields();
		gameObject.SetActive(true);
	}

}
