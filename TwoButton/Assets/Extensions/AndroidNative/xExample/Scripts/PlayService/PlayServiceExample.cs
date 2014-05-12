////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public class PlayServiceExample : MonoBehaviour {

	private string playerLabel = "Player disconnected";

	private int score = 100;


	//example
	//private const string LEADERBOARD_NAME = "leaderboard_easy";
	private const string LEADERBOARD_NAME = "REPLACE_WITH_YOUR_NAME";


	//example
	//private const string LEADERBOARD_ID = "CgkI3rzhk6QZEAIQBw";
	private const string LEADERBOARD_ID = "REPLACE_WITH_YOUR_ID";



	void Start() {
		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);


		//listen for GooglePlayManager events
		GooglePlayManager.instance.addEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
		GooglePlayManager.instance.addEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);


		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 

	}


	void OnGUI() {

		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			if(GooglePlayManager.instance.player.icon != null) {
				GUI.DrawTexture(new Rect(20, 20, GooglePlayManager.instance.player.icon.width, GooglePlayManager.instance.player.icon.height), GooglePlayManager.instance.player.icon);
			}
		} 
		GUI.Label (new Rect(145, 25, 150, 50), playerLabel);


		string title = "GooglePlay Connect";

		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			title = "GooglePlay Disconnect";
		}


		Color c = GUI.color;


		if(GUI.Button(new Rect(10, 130, 150, 50), title)) {
			Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.state.ToString());
			if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
				GUI.color = Color.red;
				GooglePlayConnection.instance.disconnect ();
			} else {
				GUI.color = Color.green;
				GooglePlayConnection.instance.connect ();
			}

		}

		GUI.color = c;

		if(GUI.Button(new Rect(10, 190, 150, 50), "Achivments UI")) {
			GooglePlayManager.instance.showAchivmentsUI ();
		}

		if(GUI.Button(new Rect(10, 250, 150, 50), "Leader Boards UI")) {
			GooglePlayManager.instance.showLeaderBoardsUI ();
		}



		/*************************/




		if(GUI.Button(new Rect(170, 130, 150, 50), "Load LeaderBoards")) {
			GooglePlayManager.instance.addEventListener (GooglePlayManager.LEADERBOARDS_LOEADED, OnLeaderBoardsLoaded);
			GooglePlayManager.instance.loadLeaderBoards ();

		}

		if(GUI.Button(new Rect(170, 190, 150, 50), "SubmitScore")) {
			score++;
			GooglePlayManager.instance.submitScore (LEADERBOARD_NAME, score);

		}

		if(GUI.Button(new Rect(170, 250, 150, 50), "L-Board UI by Name")) {
			GooglePlayManager.instance.showLeaderBoard (LEADERBOARD_NAME);
		}




		/*************************/

		if(GUI.Button(new Rect(330, 130, 150, 50), "Report Achivment")) {
			GooglePlayManager.instance.reportAchievement ("achievement_prime");
		}

		if(GUI.Button(new Rect(330, 190, 150, 50), "Increment Achivment")) {
			GooglePlayManager.instance.incrementAchievement ("achievement_bored", 2);
		}

		if(GUI.Button(new Rect(330, 250, 150, 50), "Reveal Achivment")) {
			GooglePlayManager.instance.revealAchievement ("achievement_humble");
		}

		if(GUI.Button(new Rect(330, 310, 150, 50), "Load Achivments")) {
			GooglePlayManager.instance.addEventListener (GooglePlayManager.ACHIEVEMENTS_LOADED, OnAchivmentsLoaded);
			GooglePlayManager.instance.loadAchievements ();
		}


		/*************************/


		Color cc = GUI.color;
		GUI.color = Color.green;
		if(GUI.Button(new Rect(10, 310, 150, 50), "Back")) {
			
			Application.LoadLevel ("Preview");
			
		}
		
		GUI.color = cc;



	}

	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnAchivmentsLoaded(CEvent e) {
		GooglePlayManager.instance.removeEventListener (GooglePlayManager.ACHIEVEMENTS_LOADED, OnAchivmentsLoaded);
		GooglePlayResult result = e.data as GooglePlayResult;
		if(result.isSuccess) {
			AndroidNative.showMessage ("OnAchivmentsLoaded", "Total Achivments: " + GooglePlayManager.instance.achievements.Count.ToString());
		} else {
			AndroidNative.showMessage ("OnAchivmentsLoaded error: ", result.message);
		}

	}

	private void OnAchivmentUpdated(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		AndroidNative.showMessage ("OnAchivmentUpdated ", "Id: " + result.achievementId + "\n status: " + result.message);
	}


	private void OnLeaderBoardsLoaded(CEvent e) {
		GooglePlayManager.instance.removeEventListener (GooglePlayManager.LEADERBOARDS_LOEADED, OnLeaderBoardsLoaded);

		GooglePlayResult result = e.data as GooglePlayResult;
		if(result.isSuccess) {
			if( GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID) == null) {
				AndroidNative.showMessage("Leader boards loaded", LEADERBOARD_ID + " not found in leader boards list");
				return;
			}


			AndroidNative.showMessage (LEADERBOARD_NAME + "  score",  GooglePlayManager.instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL).score.ToString());
		} else {
			AndroidNative.showMessage ("OnLeaderBoardsLoaded error: ", result.message);
		}
	}

	private void OnScoreSubmited(CEvent e) {
		GooglePlayResult result = e.data as GooglePlayResult;
		AndroidNative.showMessage ("OnScoreSubmited", result.message);
	}



	private void OnPlayerDisconnected() {
		playerLabel = "Player disconnected";
	}

	private void OnPlayerConnected() {
		playerLabel = GooglePlayManager.instance.player.name;
	}


	void OnDestroy() {
		if(!GooglePlayConnection.IsDestroyed) {
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);

		}

		if(!GooglePlayManager.IsDestroyed) {
			GooglePlayManager.instance.removeEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
			GooglePlayManager.instance.removeEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);
		}

	}


}
