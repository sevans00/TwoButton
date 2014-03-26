using UnityEngine;
using System.Collections;

public class CrumbleBlock : InteractiveTile {

	tk2dSpriteAnimator animator;
	public void Start () {
		animator = GetComponent<tk2dSpriteAnimator>();
		animator.AnimationCompleted = animationCompleted;
		animator.Stop();
	}

	public bool triggered = false;
	public void OnSpritePhysicsCollision ( Collider2D other ) {
		if ( triggered ) {
			return;
		}
		triggered = true;
		animator.Play();
	}

	void animationCompleted ( tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip ) {
		if ( gameObject != null ) {
			gameObject.SetActive(false);
		}
	}

	public override void Reset ()
	{
		gameObject.SetActive(true); 
		animator.Stop();
		animator.SetFrame(0);
		triggered = false;
		base.Reset ();
	}

}
