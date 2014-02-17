using UnityEngine;
using System.Collections;

public class TriggerTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		BoxCollider2D bcollider = GetComponent<BoxCollider2D>();

	}

	void OnTriggerEnter2D ( Collider2D other ) {
		Debug.Log("TRIGGER ENTERED!");
	}

}
