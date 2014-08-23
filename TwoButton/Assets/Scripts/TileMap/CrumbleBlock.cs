using UnityEngine;
using System.Collections;

public class CrumbleBlock : InteractiveTile {

	tk2dSpriteAnimator animator;
	public void Start () {
		animator = GetComponent<tk2dSpriteAnimator>();
		animator.AnimationCompleted = animationCompleted;
		animator.Stop();
	}

	//Pause code:
	public void Update () {
		if ( Game.instance.paused && triggered && animator.IsPlaying( animator.CurrentClip ) ) {
			animator.Pause();
		}
		if ( !Game.instance.paused && triggered && animator.Paused ) {
			animator.Resume();
		}
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
