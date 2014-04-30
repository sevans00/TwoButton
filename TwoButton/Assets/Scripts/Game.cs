using MadLevelManager;
using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public static Game instance;

	public GameObject JumperPrefab;
	public Transform SpawnPoint;

	public float leftTime = 0;
	public bool left = false;
	public float rightTime = 0;
	public bool right = false;

	public bool jump = false;
	
	public float spawnTime = 0f;
	public float timeElapsed;
	public tk2dTextMesh elapsedTimeTextMesh;
	public EndOfLevelMenu endOfLevelMenu;

	public bool gameOver = false;

	private InteractiveTile [] tiles;
	// Use this for initialization
	void Start () {
		Game.instance = this;
		DontDestroyOnLoad(gameObject);
		OnLevelWasLoaded();
	}

	void OnLevelWasLoaded () {
		//Debug.Log("Level loaded!");
		//Debug to check if the level is even present - we don't care, we just don't want the if statement complaining
		MadLevelConfiguration.Level levelName = MadLevel.activeConfiguration.FindFirstForScene(Application.loadedLevelName);
		if ( levelName != null && MadLevel.currentGroupName == MadLevel.defaultGroupName ) {
			elapsedTimeTextMesh.gameObject.SetActive(false);
		} else {
			elapsedTimeTextMesh.gameObject.SetActive(true);
		}

		//No spawn point, don't do anything else:
		if ( GameObject.FindObjectOfType<SpawnPoint>() == null ) {
			Debug.LogWarning("Warning - Game could not find spawn point");
			return;
		}
		SpawnPoint = GameObject.FindObjectOfType<SpawnPoint>().transform;
		//Caching things:
		tiles = GameObject.FindObjectsOfType<InteractiveTile>();


		//Spawn player:
		if ( SpawnPoint != null ) {
			spawnPlayer();	
		} else {
			Debug.LogWarning("Warning - Game could not find spawn point");
		}

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKey(KeyCode.W) ) {
			jump = true;
		} else {
			jump = false;
		}

		bool rightnew = false;
		bool leftnew = false;
		if ( Input.touchCount > 0 ) {
			//Multitouch:
			foreach ( Touch touch in Input.touches ) {

				//Jumping check:
				if ( touch.position.y < Screen.height - Screen.height / 3 ) {
					//Regular moving code
					if ( touch.position.x > Screen.width / 2 ) {
						rightnew = true;
					}
					//Regular moving code
					if ( touch.position.x < Screen.width / 2 ) {
						leftnew = true;
					}
				}
				//Jumping:
				if ( touch.position.y > Screen.height - Screen.height / 3 ) {
					jump = true;
				}
			}
		}
		
		//Keyboard for debugging:
		if ( Input.GetKey(KeyCode.A) ) {
			leftnew = true;
		}
		if ( Input.GetKey(KeyCode.D) ) {
			rightnew = true;
		}


		//Actually set left and right:
		if ( !right && rightnew ) {
			RightDown();
		}
		right = rightnew;
		if ( !left && leftnew ) {
			LeftDown();
		}
		left = leftnew;

		updateElapsedTime();
	}

	public void updateElapsedTime () {
		if ( !gameOver ) {
			timeElapsed = Time.time - spawnTime;
			timeElapsed = (Mathf.Ceil(timeElapsed*100)/100);
			elapsedTimeTextMesh.text = string.Format("{0:0.00}", timeElapsed);
			elapsedTimeTextMesh.Commit();
		}
	}

	
	public void LeftUp () {
		left = false;
	}
	public void LeftDown () {
		leftTime = Time.time;
		left = true;
	}
	public void RightUp () {
		right = false;
	}
	public void RightDown () {
		rightTime = Time.time;
		right = true;
	}




	public void GameOver () {
		StartCoroutine("doGameOver");
	}
	IEnumerator doGameOver() {
		gameOver = true;
		yield return new WaitForSeconds(0.8f);
		ResetLevel();
		spawnPlayer();
	}
	//Reset level:
	public void RestartLevel() {
		GameObject currentPlayer = GameObject.FindWithTag("Player");
		if ( currentPlayer != null ) {
			currentPlayer.SendMessage("Kill");
		}
	}

	//Spawn player:
	public void spawnPlayer (){
		GameObject jumper = Instantiate(JumperPrefab, SpawnPoint.position, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<CameraFollow>().target = jumper.transform;
		spawnTime = Time.time;
		gameOver = false;
	}

	//Reset the level by resetting all objects inside it
	public void ResetLevel () {
		spawnTime = Time.time;
		gameOver = false;
		foreach ( InteractiveTile tile in tiles ) {
			tile.Reset();
		}
	}

	public void ResetProfile () {
		MadLevelProfile.Reset();
	}

	public void EndLevel () {
		gameOver = true;
		pause();
		Debug.Log("Level Complete!");
		//float timeElapsed = Time.time - spawnTime;
		//string formattedTime = string.Empty+(Mathf.Ceil(timeElapsed*100)/100);
		//MadLevelProfile.SetLevelString(MadLevel.currentLevelName, "time", formattedTime);
		//Set stars:
		StarBlock[] starBlocks = GameObject.FindObjectsOfType<StarBlock>();
		foreach ( StarBlock starBlock in starBlocks ) {
			if ( starBlock.got ) {
				MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_"+starBlock.starId, true);
			}
		}
		MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
		endOfLevelMenu.Show();
		//Analytics:
		GA.API.Design.NewEvent("Game:Level:"+MadLevel.currentLevelName+":Complete", timeElapsed);
	}
	//Not used - endOfLevelMenu is doing this now
	public void NextLevel () {
		Debug.Log("Next Level!");
		unpause();
		if ( MadLevel.HasNextInGroup(MadLevel.Type.Level) ) {
			MadLevel.LoadNextInGroup(MadLevel.Type.Level);
		} else {
			MadLevel.LoadLevelByName(MadLevel.currentGroupName);
		}
		/*
		int level = Application.loadedLevel + 1;
		if ( level >= Application.levelCount ) {
			level = 0;
		}
		Debug.Log("Do Level! "+level);
		unpause();
		Application.LoadLevel(level);
		*/
	}

	private float timeScale = 1;
	public void pause() {
		if ( timeScale != 0 ) {
			timeScale = Time.timeScale;
		}
		Time.timeScale = 0;
	}
	public void unpause() {
		Time.timeScale = timeScale;
	}



}
