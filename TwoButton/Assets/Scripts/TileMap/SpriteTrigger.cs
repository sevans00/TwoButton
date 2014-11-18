using UnityEngine;
using System.Collections;

//Used to detect the presence of the player inside this trigger
[RequireComponent (typeof (BoxCollider2D))]
public class SpriteTrigger : InteractiveTile {

	public GameObject target = null;
	private BoxCollider2D boxcollider;
	private bool wasTriggeredLastUpdate = false;

	public void Start () {
		boxcollider = GetComponent<BoxCollider2D>();
		boxcollider.isTrigger = true;
	}

	//Detect if the player is in the trigger
	public void FixedUpdate() {
		//TODO: Cache this in Game perhaps.  Can't cache it here, because it'll be destroyed.
		SpritePhysics spritePhysics = GameObject.FindObjectOfType<SpritePhysics>();
		if ( spritePhysics != null ) {
			Vector2 []bounds = spritePhysics.GetColliderBounds();
			foreach ( Vector2 boundPoint in bounds) {
				if ( boxcollider.OverlapPoint(boundPoint) ) {
					if ( !wasTriggeredLastUpdate ) {
						wasTriggeredLastUpdate = true;
						Broadcast("OnSpriteTriggerEnter", SendMessageOptions.DontRequireReceiver);
					} else {
						Broadcast("OnSpriteTriggerStay", SendMessageOptions.DontRequireReceiver);
					}
					return;
				}
			}
		}
		if ( wasTriggeredLastUpdate ) {
			wasTriggeredLastUpdate = false;
			Broadcast("OnSpriteTriggerExit", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Broadcast(string message, SendMessageOptions options) {
		if ( target == null ) {
			BroadcastMessage(message, options);
		} else {
			target.BroadcastMessage(message, options);
		}
	}

}
