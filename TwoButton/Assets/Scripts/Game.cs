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




	// Use this for initialization
	void Start () {
		Game.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) {
			showCharacterSelectScreen();
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
		if ( GameObject.FindObjectOfType<LevelTileMap>() != null ) {
			GameObject.FindObjectOfType<LevelTileMap>().ResetLevel();
		} else {
			Debug.LogWarning("Game.cs could not find a LevelTileMap to reset all tiles!");
		}
		GameObject jumper = Instantiate(JumperPrefab, SpawnPoint.position, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<CameraFollow>().target = jumper.transform;
	}

	public void EndLevel () {
		pause();
		Debug.Log("Level Complete!");
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
