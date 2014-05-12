using UnityEngine;
using System.Collections;

public class PlayServicFridnsLoadExample : MonoBehaviour {
	

	public GUIStyle headerStyle;
	public GUIStyle boardStyle;
	


	private bool renderFriendsList = false;
	
	void Awake() {

		
		
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnAuth);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnAuthFailed);
		
		
		GooglePlayManager.instance.addEventListener (GooglePlayManager.PLAYERS_LOADED, OnFriendListLoaded);
		
		
		//connecting
		GooglePlayConnection.instance.connect ();
	}




	void OnGUI() {
		
		GUI.Label(new Rect(10, 20, 400, 40), "Friend List Load Example", headerStyle);
		
		if(GUI.Button(new Rect(300, 10, 150, 50), "Load Friends")) {
			GooglePlayManager.instance.loadConnectedPlayers();
		}


		if(!renderFriendsList) {
			return;
		}

	
		

		GUI.Label(new Rect(10,  90, 100, 40), "id", boardStyle);
		GUI.Label(new Rect(150, 90, 100, 40), "name", boardStyle);;
		GUI.Label(new Rect(300, 90, 100, 40), "avatar ", boardStyle);

		int i = 1;
		foreach(string FriendId in GooglePlayManager.instance.friendsList) {

			GooglePlayerTemplate player = GooglePlayManager.instance.GetPlayerById(FriendId);
			if(player != null) {
				GUI.Label(new Rect(10,  90 + 70 * i, 100, 40), player.playerId, boardStyle);
				GUI.Label(new Rect(150, 90 + 70 * i , 100, 40), player.name, boardStyle);
				if(player.icon != null) {
					GUI.DrawTexture(new Rect(300, 75 + 70 * i, 50, 50), player.icon);
				} else  {
					GUI.Label(new Rect(300, 90 + 70 * i, 100, 40), "no photo ", boardStyle);
				}



				i++;
			}

		}


	}

	private void OnFriendListLoaded() {
		renderFriendsList = true;
	}
	
	private void OnAuth() {
		Debug.Log("Player Authed");


	}
	
	private void OnAuthFailed() {
		Debug.Log(" Player auntification failed");
	}

}
