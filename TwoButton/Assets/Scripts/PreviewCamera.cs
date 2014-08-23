using UnityEngine;
using System.Collections;
using MadLevelManager;

public class PreviewCamera : MonoBehaviour {

	public static PreviewCamera instance;

	public void Awake () {
		Debug.LogWarning("Preview camera awake");
		PreviewCamera.instance = this;
	}

	public void Update () {
		//Game.instance.pause();
		if ( ( Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Began ) 
		    || (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ) ) {
//			gameObject.SetActive(false);
			Game.instance.unpause();
			//TODO: Zoom in
			valueToExample();
		}
	}

	private void valueToExample()
	{
		Debug.LogWarning("Value to example");
		float fromValue = 0f;
		float toValue = 10f;
		iTween.ValueTo( gameObject, iTween.Hash(
						"from", fromValue, 
		               	"to", toValue, 
		               	"onupdatetarget", gameObject, 
		               	"onupdate", "updateFromValue", 
		               	"time", 1f, 
						"easetype", iTween.EaseType.easeOutExpo ) );
		Debug.LogWarning("Value to example done");
	}
	
	public void updateFromValue( float newValue )
	{
		float value = 0f;
		Debug.LogWarning( "My Value that is tweening: " + newValue );
	}

	public void tweenComplete (  ) {
		Debug.LogWarning("itween Complete!");
	}


}
