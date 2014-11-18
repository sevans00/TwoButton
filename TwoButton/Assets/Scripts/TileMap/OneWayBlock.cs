using UnityEngine;
using System.Collections;

//A one-way block
public class OneWayBlock : InteractiveTile {

	private BoxCollider2D boxcollider;
	private float maxY;
	public void Start () {
		boxcollider = GetComponent<BoxCollider2D>();
		maxY = transform.position.y + boxcollider.center.y + boxcollider.size.y/2 - 0.0f;
	}

	override public void Reset () {
		boxcollider.isTrigger = false; //Make a collider again
	}

	override public void DoFixedUpdate () {
		if ( Game.instance.jumper == null ) {
			return;
		}
		//For now, just check vertical:
		if ( Game.instance.jumper.transform.position.y >= maxY ) { 
			boxcollider.isTrigger = false;
		} else {
			boxcollider.isTrigger = true;
		}

	}

}
