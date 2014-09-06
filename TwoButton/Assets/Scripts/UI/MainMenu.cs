﻿using MadLevelManager;
using UnityEngine;
using System.Collections;

public class MainMenu : UIMenu {

	public GameObject quitGameDialog;
	public GameObject rootMenu;
	public tk2dUIItem playButton;

	// Use this for initialization
	void Start () {
		HideQuitGame();

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown ( KeyCode.Escape ) ) {
			if ( quitGameDialog.activeSelf ) {
				HideQuitGame();
			} else {
				ShowQuitGame();
			}
		}
	}


	public void ShowQuitGame() {
		quitGameDialog.SetActive(true);
		rootMenu.SetActive(false);
	}
	
	public void HideQuitGame() {
		quitGameDialog.SetActive(false);
		rootMenu.SetActive(true);
	}

	public void QuitGame() {
		Application.Quit();
	}
	
	public void PlayGame() {
		if ( PlayerPrefs.GetInt("First Time", 0) == 0 ) {
			MadLevel.LoadLevelByName("0_IntroComic");
			PlayerPrefs.SetInt("First Time", 1);
		} else {
			MadLevel.LoadLevelByName("World Select Screen");
		}
	}



}

