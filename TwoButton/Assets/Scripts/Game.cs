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
		resetTileMap();
		Debug.Log("NEW PLAYER!");
		GameObject jumper = Instantiate(JumperPrefab, SpawnPoint.position, Quaternion.identity) as GameObject;
		Camera.main.GetComponent<CameraFollow>().target = jumper.transform;
	}

	//Currently I have no idea how to do this:
	public void resetTileMap() {
		tk2dTileMap tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		Debug.Log(tilemap.renderData);
		//tilemap.Build();
		//tilemap.Build();
	}

	public void pause() {

	}
	public void unpause() {
		
	}



}
