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

	
	private InteractiveTile [] tiles;
	// Use this for initialization
	void Start () {
		Game.instance = this;
		tiles = GameObject.FindObjectsOfType<InteractiveTile>();
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) {
			showCharacterSelectScreen();
		}

		if ( Input.GetKey(KeyCode.W) ) {
			jump = true;
		} else {
			jump = false;
		}

		if ( Input.GetKeyDown(KeyCode.A) ) {
			LeftDown();
		}
		if ( Input.GetKeyUp(KeyCode.A) ) {
			LeftUp();
		}
		if ( Input.GetKeyDown(KeyCode.D) ) {
			RightDown();
		}
		if ( Input.GetKeyUp(KeyCode.D) ) {
			RightUp();
		}

		if ( Input.touchCount > 0 ) {
			//Multitouch:
			foreach ( Touch touch in Input.touches ) {

				//Jumping check:
				if ( touch.position.y < Screen.height - Screen.height / 3 ) {//
					//Regular moving code
					if ( touch.phase == TouchPhase.Began && touch.position.x > Screen.width / 2 ) {
						RightDown();
					}
					if ( touch.phase == TouchPhase.Ended && touch.position.x > Screen.width / 2 ) {
						RightUp();
					}
					if ( touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2 ) {
						LeftDown();
					}
					if ( touch.phase == TouchPhase.Ended && touch.position.x < Screen.width / 2 ) {
						LeftUp();
					}
				}
				//Jumping:
				if ( touch.position.y > Screen.height - Screen.height / 3 ) {
					jump = true;
				}
			}
		}

	}

	public void showCharacterSelectScreen() {
		CharacterSelectScreen.instance.ToggleCharacterSelectScreen();
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
		yield return new WaitForSeconds(0.8f);
		ResetLevel();
		GameObject jumper = Instantiate(JumperPrefab, SpawnPoint.position, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<CameraFollow>().target = jumper.transform;
	}

	//Reset the level by resetting all objects inside it
	public void ResetLevel () {
		foreach ( InteractiveTile tile in tiles ) {
			tile.Reset();
		}
	}

	public void EndLevel () {
		pause();
		Debug.Log("Level Complete!");
		NextLevel();
	}
	public void NextLevel () {
		Debug.Log("Next Level!");
		int level = Application.loadedLevel + 1;
		if ( level >= Application.levelCount ) {
			level = 0;
		}
		Debug.Log("Do Level! "+level);
		unpause();
		Application.LoadLevel(level);
	}

	private float timeScale = -1;
	public void pause() {
		if ( timeScale == -1 ) {
			timeScale = Time.timeScale;
		}
		Time.timeScale = 0;
	}
	public void unpause() {
		Time.timeScale = timeScale;
	}



}
