using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	public GameObject parallaxBG = null;
	public float parallaxDelta = 0.1f;
	public Vector2 deltaMovement;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( target != null ) {
			//Update delta:
			deltaMovement = (Vector2)this.transform.position - (Vector2)target.position;
			
			//Update position:
			this.transform.position = target.position + Vector3.back*10;
			//Parallax thingy:
			if ( parallaxBG != null ) {
				parallaxBG.transform.position += (Vector3)deltaMovement*parallaxDelta;
			}
		}
	}
}
