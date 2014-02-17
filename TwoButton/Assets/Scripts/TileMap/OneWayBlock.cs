using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
//A one-way block
public class OneWayBlock : MonoBehaviour {

	private BoxCollider2D boxcollider;
	private float maxY;
	public void Start () {
		boxcollider = GetComponent<BoxCollider2D>();
		maxY = transform.position.y + boxcollider.center.y + boxcollider.size.y/2;
	}

	public void FixedUpdate () {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if ( player == null ) {
			return;
		}
		//For now, just check vertical:  //Add-on check for if the position will be further down next step
		if ( player.transform.position.y >= maxY + player.GetComponent<SpritePhysics>().velocity.y) { 
			boxcollider.isTrigger = false;
		} else {
			boxcollider.isTrigger = true;
		}

	}

}
