using MadLevelManager;
using UnityEngine;
using System.Collections;

public class InLevelMenu : MonoBehaviour {

	public GameObject inLevelMenu;
	public GameObject characterSelectScreen;

	public GameObject endOfLevelMenu;
	public Camera guiCamera;
	public tk2dSprite bgSprite;

	public GameObject levelNameGameObject;
	public Vector3 levelNameDeployedPosition;
	public Vector3 levelNameHiddenPosition;

	private Vector3 deployedPosition;
	private Vector3 hiddenPosition;
	private float animateTime = 0.4f;

	public bool isShown = false;
	public bool isHidden = false;

	// Use this for initialization
	public void Awake () {
		deployedPosition = inLevelMenu.transform.position;
		hiddenPosition = deployedPosition + Vector3.down * 0.46f;
		inLevelMenu.transform.position = hiddenPosition;
		inLevelMenu.SetActive(false);
		levelNameDeployedPosition = levelNameGameObject.transform.position;
		levelNameHiddenPosition = levelNameGameObject.transform.position+Vector3.up*0.4f;
		levelNameGameObject.transform.position = levelNameHiddenPosition;
		levelNameGameObject.SetActive(false);
	}

	public void DoLevelWasLoaded () {
		iTween.StopByName("inlevel_hide");
		iTween.StopByName("inlevel_hide_levelname");
		//Set to "shown" view:
		inLevelMenu.SetActive(true);
		inLevelMenu.transform.position = deployedPosition;
		levelNameGameObject.SetActive(true);
		levelNameGameObject.transform.position = levelNameDeployedPosition;
	}

	// Update is called once per frame
	void Update () {
		//Touch to unpause:
		if ( inLevelMenu.activeSelf && isShown ) {
			if ( Input.GetMouseButton(0) ) {
				Vector3 mousePositionInWorld = guiCamera.ScreenToWorldPoint( Input.mousePosition );
				if ( mousePositionInWorld.y > bgSprite.transform.TransformPoint(bgSprite.GetBounds().min).y ){
					Hide();
				}
			}
		}

		if ( Input.GetKeyDown ( KeyCode.Escape ) && !MadLevel.currentGroupName.Equals(MadLevel.defaultGroupName) && !Game.instance.endLevel ) {
			if ( inLevelMenu.activeSelf ) {
				Hide();
			} else {
				Show();
			}
		}
	}

	public void Show () {
		isHidden = false;
		iTween.StopByName("inlevel_hide");
		iTween.StopByName("inlevel_hide_levelname");
		inLevelMenu.SetActive(true);
		levelNameGameObject.SetActive(true);
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
		iTween.MoveTo( levelNameGameObject, iTween.Hash(
			"name", "inlevel_show_levelname",
			"oncompletetarget", gameObject,
			"oncomplete", "onShowComplete",
			"position", levelNameDeployedPosition,
			"time", animateTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
	}

	public void onShowComplete () {
		isShown = true;
		isHidden = false;
	}
	
	public void Hide () {
		isShown = false;
		iTween.StopByName("inlevel_show");
		iTween.StopByName("inlevel_show_levelname");
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
		iTween.MoveTo( levelNameGameObject, iTween.Hash(
			"name", "inlevel_hide_levelname",
			"position", levelNameHiddenPosition,
			"time", animateTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void onHideComplete () {
		isShown = false;
		isHidden = true;
		inLevelMenu.SetActive(false);
		levelNameGameObject.SetActive(false);
		CameraPathAnimator pathAnimator = GameObject.FindObjectOfType<CameraPathAnimator>();
		if ( pathAnimator != null ) {
			pathAnimator.Play();
			return;
		}
		if ( Game.instance != null ) {
			//Game.instance.unpause(); //Preview camera does this
		}
	}
	public void setHidden () {
		isShown = false;
		isHidden = true;
		inLevelMenu.SetActive(false);
		levelNameGameObject.SetActive(false);
		inLevelMenu.transform.position = hiddenPosition;
		levelNameGameObject.transform.position = levelNameHiddenPosition;
	}
	public void setShown () {
		isShown = true;
		isHidden = false;
		inLevelMenu.SetActive(true);
		levelNameGameObject.SetActive(true);
		inLevelMenu.transform.position = deployedPosition;
		levelNameGameObject.transform.position = levelNameDeployedPosition;
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
		if ( MadLevel.currentGroupName == "World4" ) {
			MadLevel.LoadLevelByName("Main Menu");
			return;
		}
		MadLevel.LoadLevelByName(levelName); //Note this is a hack
	}
	public void QuitGame() {
		Application.Quit();
	}




}
