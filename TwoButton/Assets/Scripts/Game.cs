using MadLevelManager;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public static Game instance;

	public GameObject JumperPrefab;
	public GameObject jumper;
	public JumperSMB jumperScript;
	public Transform SpawnPoint;

	public InteractiveTile [] interactiveTiles;

	public float leftTime = 0;
	public bool left = false;
	public float rightTime = 0;
	public bool right = false;

	public bool jump = false;
	
	public float spawnTime = 0f;
	public float timeElapsed;
	public float displayTime;
	public tk2dTextMesh elapsedTimeTextMesh;
	public EndOfLevelMenu endOfLevelMenu;
	public StarCount starCount;
	public tk2dTextMesh levelNameTextMesh;
	public InLevelMenu inLevelMenu; //Has all the above within it

	public bool paused = false;

	public bool gameOver = false;
	public bool endLevel = false;

	public bool isGameLevel = false;

	public Camera mainCamera;

	public PreviewCamera previewCamera;

	private InteractiveTile [] tiles;
	// Use this for initialization
	void Start () {
		Game.instance = this;
		DontDestroyOnLoad(gameObject);
		OnLevelWasLoaded();
	}

	void OnLevelWasLoaded () {
		//Disable preview camera:
		//previewCamera.camera.enabled = false;

		//Debug.Log("Level loaded!");
		//Debug to check if the level is even present - we don't care, we just don't want the if statement complaining
		bool hasMany;
		MadLevelConfiguration.Level levelName = MadLevel.activeConfiguration.FindFirstForScene(Application.loadedLevelName, out hasMany);
		if ( levelName != null && MadLevel.currentGroupName == MadLevel.defaultGroupName ) {
			elapsedTimeTextMesh.gameObject.SetActive(false);
			starCount.gameObject.SetActive(false);
			isGameLevel = false;
		} else {
			elapsedTimeTextMesh.gameObject.SetActive(true);
			starCount.gameObject.SetActive(true);
			starCount.Reset();
			isGameLevel = true;
		}

		//Is there a DisableGame object?  If so, we'll want to not set some things up:
		if ( GameObject.FindObjectOfType<DisableGame>() != null ) {
			elapsedTimeTextMesh.gameObject.SetActive(false);
			starCount.gameObject.SetActive(false);
			isGameLevel = false;
			//Hack to disable cameras on comiclevel
			previewCamera.camera.enabled = false;
			previewCamera.mainCamera.enabled = false;
			return;
		}

		//No spawn point, don't do anything else:
		if ( GameObject.FindObjectOfType<SpawnPoint>() == null ) {
			Debug.LogWarning("Warning - Game could not find spawn point");
			isGameLevel = false; //Can't have a game without a spawn point
			return;
		}
		SpawnPoint = GameObject.FindObjectOfType<SpawnPoint>().transform;
		//Caching interactive tiles:
		tiles = GameObject.FindObjectsOfType<InteractiveTile>();

		//Determine level name:
		MadLevelConfiguration.Level level = MadLevel.activeConfiguration.FindLevelByName(MadLevel.currentLevelName);
		List<MadLevelConfiguration.Level> levelList = level.group.GetLevels();
		int ii;
		for ( ii = 0; ii < levelList.Count; ii++ ) {
			if ( levelList[ii].name == MadLevel.currentLevelName ) {
				ii++;
				break;
			}
		}
		if ( level.group.name == "World0" ) {
			levelNameTextMesh.text = "Tutorial Level "+ii;
		} else {
			levelNameTextMesh.text = "World "+ level.groupId +" Level "+ii;
		}
		inLevelMenu.DoLevelWasLoaded();

		//Spawn player:
		if ( SpawnPoint != null ) {
			spawnPlayer();
		} else {
			Debug.LogWarning("Warning - Game could not find spawn point");
		}
		pause();
		//Setup references for FixedUpdates:
		interactiveTiles = FindObjectsOfType<InteractiveTile>();

		//Start preview camera
		previewCamera.camera.enabled = true;
		previewCamera.DoLevelWasLoaded();
	}

	//Fixed Update is called every physics frame: It handles all other fixedupdates of other objects
	public void FixedUpdate () {
		if ( !isGameLevel || paused || gameOver ) { //If this isn't a game level, we don't care
			return;
		}
		updateElapsedTime(); //Test
		//Jumper input and collisions:
		jumperScript.DoInputAndCollisions();
		//Interactive objects:
		foreach ( InteractiveTile interactiveTile in interactiveTiles ) {
			interactiveTile.DoFixedUpdate();
		}
		//Jumper elevator update:
		jumperScript.DoElevatorCheck();
		//Jumper regular update:
		jumperScript.DoFixedUpdate();
	}


	// Update is called once per frame
	/*void Update () {
		if ( paused ) {
			return;
		}
		updateElapsedTime(); //Test
	}*/

	public void updateElapsedTime () {
		if ( !gameOver && !paused ) {
			timeElapsed += Time.deltaTime;
			displayTime = (Mathf.Ceil(timeElapsed*100)/100);
			elapsedTimeTextMesh.text = string.Format("{0:0.00}", displayTime);
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
		yield return new WaitForSeconds(0.4f);//May not be needed
		ResetLevel();
		spawnPlayer();
	}
	//Reset level:
	public void RestartLevel() {
		DestroyImmediate(jumper);
		ResetLevel();
		spawnPlayer();
	}

	//Spawn player:
	public void spawnPlayer (){
		jumper = Instantiate(JumperPrefab, SpawnPoint.position - Vector3.up*0.63f, Quaternion.identity) as GameObject;
		jumperScript = jumper.GetComponent<JumperSMB>();
		CameraFollow.instance.target = jumper.transform;
		timeElapsed = 0f;
		elapsedTimeTextMesh.text = string.Format("{0:0.00}", 0f);//Reset the counter
		elapsedTimeTextMesh.Commit();
		gameOver = false;
		endLevel = false;
	}

	//Reset the level by resetting all objects inside it
	public void ResetLevel () {
		timeElapsed = 0f;
		gameOver = false;
		endLevel = false;
		foreach ( InteractiveTile tile in tiles ) {
			tile.Reset();
		}
	}

	public void ResetProfile () {
		PlayerPrefs.SetInt("First Time", 0);
		MadLevelProfile.Reset();
	}

	public void EndLevel (GameObject finishBlock = null) {
		gameOver = true;
		endLevel = true;
		//Debug.Log("Level Complete!");
		//Set stars:
		StarBlock[] starBlocks = GameObject.FindObjectsOfType<StarBlock>();
		foreach ( StarBlock starBlock in starBlocks ) {
			if (starBlock.got ) {
				MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_"+starBlock.starId, true);
			}
		}
		MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
		//Analytics:
		GA.API.Design.NewEvent("Game:Level:"+MadLevel.currentLevelName+":Complete", displayTime);

		_finishBlock = finishBlock;

		StartCoroutine("_endLevelAnimCoroutine");
	}
	private GameObject _finishBlock;
	private IEnumerator _endLevelAnimCoroutine () {
		//Pause player:
		//jumper = GameObject.FindGameObjectWithTag("Player");
		jumper.GetComponent<JumperSMB>().enabled = false; //This will disable physics and death as well
		jumper.transform.position = new Vector3(jumper.transform.position.x, _finishBlock.transform.position.y-0.64f, jumper.transform.position.z);

		jumper.GetComponent<CharacterAnimator>().spriteAnimator.Play(jumper.GetComponent<CharacterAnimator>().idle_animationName);
		//Face the flag:
		tk2dSprite sprite = jumper.GetComponent<tk2dSprite>();
		if ( jumper.transform.position.x < _finishBlock.transform.position.x ) {
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
		}
		if ( jumper.transform.position.x > _finishBlock.transform.position.x ) {
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
		}
		//Play flag:
		_finishBlock.GetComponent<FinishBlock>().PlayFlag();

		yield return new WaitForSeconds(0.625f);

		//Celebrate!
		jumper.transform.position -= Vector3.forward;
		jumper.GetComponent<CharacterAnimator>().spriteAnimator.Play(jumper.GetComponent<CharacterAnimator>().victory_animationName);

		//_celebrationZoom = Camera.main.orthographicSize;
		//Camera.main.orthographicSize = 3f;
		//End celebration!
		//Invoke("EndCelebration", 2f);
		//EndCelebration();
		
		//pause();

		yield return new WaitForSeconds(1.2f);

		EndCelebration();
	}

	private float _celebrationZoom;
	public void EndCelebration () {
		//Camera.main.orthographicSize = _celebrationZoom;
		endOfLevelMenu.Show();
	}
	//Not used - endOfLevelMenu is doing this now
	public void NextLevel () {
		//Debug.Log("Next Level!");
		//unpause();
		if ( MadLevel.HasNextInGroup(MadLevel.Type.Level) ) {
			MadLevel.LoadNextInGroup(MadLevel.Type.Level);
		} else {
			MadLevel.LoadLevelByName(MadLevel.currentGroupName);
		}
	}

	private float timeScale = 1;
	public void pause() {
		paused = true;
		//CameraPathAnimator pathAnimator = GameObject.FindObjectOfType<CameraPathAnimator>();
//		if ( timeScale == 0 ) {
//			timeScale = Time.timeScale;
//		}
//		Time.timeScale = 0;
	}
	public void unpause() {
		paused = false;
//		Time.timeScale = timeScale;
	}



}
