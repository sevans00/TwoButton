using UnityEngine;
using System.Collections;

public class CrumbleBlock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool triggered = false;
	public void OnSpritePhysicsCollision ( Collider2D other ) {
		if ( triggered ) {
			return;
		}
		triggered = true;
		Debug.Log("CrumbleBlock Collision!");
		GetComponent<tk2dSpriteAnimator>().Play();
		GetComponent<tk2dSpriteAnimator>().AnimationCompleted = animationCompleted;
	}

	void animationCompleted ( tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip ) {
		Debug.Log("Animation Completed!");
		if ( gameObject != null ) {
			Destroy(gameObject);
		}
	}

}
