﻿using UnityEngine;
using System.Collections;

public class GameInstance : MonoBehaviour {
	public GameObject GamePrefab;
	public void Start() {
		if ( Game.instance == null && GameObject.FindObjectOfType<Game>() == null ) {
			GameObject.Instantiate(GamePrefab, Vector3.zero, Quaternion.identity);
		}
		if ( GameObject.FindObjectsOfType<Game>().Length == 2 ) {
			Destroy ( GameObject.FindObjectOfType<Game>().gameObject );
		}
	}
}
