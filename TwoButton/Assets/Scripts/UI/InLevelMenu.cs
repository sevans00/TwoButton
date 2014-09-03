using MadLevelManager;
using UnityEngine;
using System.Collections;

public class InLevelMenu : MonoBehaviour {

	public GameObject inLevelMenu;
	public GameObject characterSelectScreen;

	public GameObject endOfLevelMenu;

	public Vector3 deployedPosition;
	public Vector3 hiddenPosition;
	private float animateTime = 0.4f;

	// Use this for initialization
	void Start () {
		deployedPosition = inLevelMenu.transform.position;
		hiddenPosition = deployedPosition + Vector3.down * 0.46f;
		inLevelMenu.transform.position = hiddenPosition;
		inLevelMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.Escape ) && !MadLevel.currentGroupName.Equals(MadLevel.defaultGroupName) && !Game.instance.endLevel ) {
			if ( inLevelMenu.activeSelf ) {
				Hide();
			} else {
				Show();
			}
		}
	}

	public void Show () {
		iTween.StopByName("inlevel_hide");
		inLevelMenu.SetActive(true);
		Game.instance.pause();
		if ( Game.instance.isGameLevel ) {
			PreviewCamera.instance.zoomOut();
		} else {
			CameraPathAnimator pathAnimator = GameObject.FindObjectOfType<CameraPathAnimator>();
			if ( pathAnimator != null ) {
				pathAnimator.Pause();
			}
		}
		iTween.MoveTo( inLevelMenu, iTween.Hash(
			"name", "inlevel_show",
			"position", deployedPosition,
			"time", animateTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
	}

	public void Hide () {
		iTween.StopByName("inlevel_show");
		if ( Game.instance.isGameLevel ) {
			PreviewCamera.instance.zoomIn();
		}
		iTween.MoveTo( inLevelMenu, iTween.Hash(
			"name", "inlevel_hide",
			"position", hiddenPosition,
			"time", animateTime,
			"oncompletetarget", gameObject,
			"oncomplete", "onHideComplete",
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onHideComplete () {
		inLevelMenu.SetActive(false);
		CameraPathAnimator pathAnimator = GameObject.FindObjectOfType<CameraPathAnimator>();
		if ( pathAnimator != null ) {
			pathAnimator.Play();
			return;
		}
		if ( Game.instance != null ) {
			//Game.instance.unpause();
		}
	}










	//Resume button
	public void Resume() {
		Hide ();
	}
	//Restart
	public void Restart() {
		CameraPathAnimator pathAnimator = GameObject.FindObjectOfType<CameraPathAnimator>();
		if ( pathAnimator != null ) {
			//Debug.LogWarning("Restart camera path");
			pathAnimator.Seek(0);
			//pathAnimator.Play();
			Hide ();
			return;
		}
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
