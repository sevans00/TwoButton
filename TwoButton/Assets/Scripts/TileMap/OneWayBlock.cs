using UnityEngine;
using System.Collections;

//A one-way block
public class OneWayBlock : MonoBehaviour {

	private BoxCollider2D boxcollider;
	private float maxY;
	public void Start () {
		boxcollider = GetComponent<BoxCollider2D>();
		maxY = transform.position.y + boxcollider.center.y + boxcollider.size.y/2 - 0.0f;
	}

	public void FixedUpdate () {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if ( player == null ) {
			return;
		}
		//For now, just check vertical:
		if ( player.transform.position.y >= maxY ) { 
			boxcollider.isTrigger = false;
		} else {
			boxcollider.isTrigger = true;
		}

	}

}
