////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayServicLbCustomGUIExample : MonoBehaviour {


	//examole
	//private const string LEADERBOARD_ID = "REPLACE_WITH_YOUR_ID";
	private const string LEADERBOARD_ID = "CgkIipfs2qcGEAIQAA";


	public int hiScore = 100;


	public GUIStyle headerStyle;
	public GUIStyle boardStyle;


	private GPLeaderBoard loadedLeaderBoard = null;

	private GPCollectionType diplayCollection = GPCollectionType.FRIENDS;

	void Awake() {



		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnAuth);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnAuthFailed);


		GooglePlayManager.instance.addEventListener (GooglePlayManager.SCORE_REQUEST_RECEIVED, OnScoreListLoaded);


		//connecting
		GooglePlayConnection.instance.connect ();
	}
	

	void OnGUI() {

		GUI.Label(new Rect(10, 20, 400, 40), "Custom Leader Board GUI Example", headerStyle);

		if(GUI.Button(new Rect(400, 10, 150, 50), "Load Friends Scores")) {
			GooglePlayManager.instance.loadPlayerCenteredScores(LEADERBOARD_ID, GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS, 10);
		}

		if(GUI.Button(new Rect(600, 10, 150, 50), "Load Global Scores")) {
			GooglePlayManager.instance.loadPlayerCenteredScores(LEADERBOARD_ID, GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL, 10);
		}

		Color defaultColor = GUI.color;

		if(diplayCollection == GPCollectionType.GLOBAL) {
			GUI.color = Color.green;
		}
		if(GUI.Button(new Rect(800, 10, 170, 50), "Displying Global Scores")) {
			diplayCollection = GPCollectionType.GLOBAL;
		}
		GUI.color = defaultColor;



		if(diplayCollection == GPCollectionType.FRIENDS) {
			GUI.color = Color.green;
		}
		if(GUI.Button(new Rect(800, 70, 170, 50), "Displying Friends Scores")) {
			diplayCollection = GPCollectionType.FRIENDS;
		}
		GUI.color = defaultColor;

		GUI.Label(new Rect(10,  90, 100, 40), "rank", boardStyle);
		GUI.Label(new Rect(100, 90, 100, 40), "score", boardStyle);
		GUI.Label(new Rect(200, 90, 100, 40), "playerId", boardStyle);
		GUI.Label(new Rect(400, 90, 100, 40), "name ", boardStyle);
		GUI.Label(new Rect(550, 90, 100, 40), "avatar ", boardStyle);

		if(loadedLeaderBoard != null) {
			for(int i = 1; i < 10; i++) {

				Debug.Log("get");
				GPScore score = loadedLeaderBoard.GetScore(i, GPBoardTimeSpan.ALL_TIME, diplayCollection);
				if(score != null) {
					GUI.Label(new Rect(10,  90 + 70 * i, 100, 40), i.ToString(), boardStyle);
					GUI.Label(new Rect(100, 90 + 70 * i, 100, 40), score.score.ToString() , boardStyle);
					GUI.Label(new Rect(200, 90 + 70 * i, 100, 40), score.playerId, boardStyle);


					GooglePlayerTemplate player = GooglePlayManager.instance.GetPlayerById(score.playerId);
					if(player != null) {
						GUI.Label(new Rect(400, 90 + 70 * i , 100, 40), player.name, boardStyle);
						if(player.icon != null) {
							GUI.DrawTexture(new Rect(550, 75 + 70 * i, 50, 50), player.icon);
						} else  {
							GUI.Label(new Rect(550, 90 + 70 * i, 100, 40), "no photo ", boardStyle);
						}
					}



				}

			}
		}



	}




	private void OnScoreListLoaded() {

		Debug.Log("Scores loaded");

		Debug.Log(LEADERBOARD_ID);
		loadedLeaderBoard = GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID);
		Debug.Log(loadedLeaderBoard);

	}




	private void OnAuth() {
		Debug.Log("Player Authed");
	}
	
	private void OnAuthFailed() {
		Debug.Log("Player auntification failed");
	}
}
