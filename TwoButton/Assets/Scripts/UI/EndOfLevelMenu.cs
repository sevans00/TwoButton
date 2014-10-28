using UnityEngine;
using System.Collections;
using MadLevelManager;


//Menu for the end of level - should display player stats for the level as well as elapsed time, best time, and three buttons:
// - Next level
// - Retry level
// - Back to level select screen
public class EndOfLevelMenu : MonoBehaviour {

	public Vector3 deployedPosition;
	public Vector3 hiddenPositionRestart;
	public Vector3 hiddenPositionBack;
	public Vector3 hiddenPositionNext;
	private float animateTime = 0.4f;

	public void Start () {
		deployedPosition = transform.position;
		hiddenPositionRestart = deployedPosition + Vector3.down * 1.78f;
		hiddenPositionBack = deployedPosition + Vector3.up * 2f;
		hiddenPositionNext = deployedPosition + Vector3.left * 4f;
		gameObject.transform.position = hiddenPositionRestart;
		gameObject.SetActive(false);
	}

	public void OnLevelLoaded () {
		Hide ();
	}

	//Stars, times, and ads to go here
	public tk2dTextMesh timeText;
	public tk2dTextMesh bestTimeText;
	public tk2dTextMesh bestTimeNewText;
	public tk2dSprite star1;
	public tk2dSprite star2;
	public tk2dSprite star3;
	public tk2dTextMesh levelName;
	
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

		levelName.text = Game.instance.levelNameTextMesh.text;

		SetStars();
	}

	public void SetStars () {
		star1.color = Game.instance.starCount.star1.color;
		star1.SetSprite(Game.instance.starCount.star1.spriteId);
		star2.color = Game.instance.starCount.star2.color;
		star2.SetSprite(Game.instance.starCount.star2.spriteId);
		star3.color = Game.instance.starCount.star3.color;
		star3.SetSprite(Game.instance.starCount.star3.spriteId);
	}




	//
	//Button callbacks:
	//

	//Next level
	public void NextLevel () {
		Game.instance.pause();
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "endlevel_hide",
			"position", hiddenPositionRestart,
			"time", animateTime,
			"oncomplete", "onNextHideComplete",
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onNextHideComplete () {
		Hide();
		if ( MadLevel.HasNextInGroup(MadLevel.Type.Level) ) {
			MadLevel.LoadNextInGroup(MadLevel.Type.Level);
		} else {
			MadLevel.LoadLevelByName(MadLevel.currentGroupName); //TODO: Determine if this is what we want
		}
		Game.instance.unpause();
	}
	//Retry
	public void RetryLevel () {
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "endlevel_hide",
			"position", hiddenPositionRestart,
			"time", animateTime,
			"oncomplete", "onRetryHideComplete",
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onRetryHideComplete () {
		Hide();
		Game.instance.RestartLevel();
		Game.instance.unpause();
	}
	//Back to level select
	public void BackToLevelSelect () {
		Hide ( hiddenPositionBack );
		MadLevel.LoadLevelByName(MadLevel.currentGroupName);
	}










	public void Hide() {
		iTween.StopByName("endlevel_hide");
		gameObject.transform.position = hiddenPositionRestart;
		gameObject.SetActive(false);
	}
	public void Hide( Vector3 animateTarget ) {
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "endlevel_hide",
			"position", animateTarget,
			"time", animateTime,
			"oncomplete", "onHideComplete",
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onHideComplete () {
		Game.instance.unpause();
		Hide();
	}
	public void Show() {
		gameObject.transform.position = hiddenPositionRestart;
		populateFields();
		gameObject.SetActive(true);
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "endlevel_show",
			//"oncomplete", "onShowComplete",
			"position", deployedPosition,
			"time", animateTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onShowComplete () {

	}
}
