using UnityEngine;
using System.Collections;
using MadLevelManager;

public class WorldSelectScreen : MonoBehaviour {


	public MadLevelConfiguration config1;
	public MadLevelConfiguration config2;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.C) ) {
			Call ();
		}
	}

	public void Call () {
		Debug.Log("CALL!");


	}

}
