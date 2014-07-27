using UnityEngine;
using System.Collections;

public class FinishBlock : InteractiveTile {

	public tk2dSpriteAnimator animator;

	void Start () {

	}

	void OnSpriteTriggerEnter () {
		Game.instance.EndLevel(gameObject);
	}

	public void PlayFlag() { 
		animator.Play("FinishBlockFlagSwap");
	}

	public override void Reset ()
	{
		base.Reset ();
		animator.Play("FinishBlockIdle");
	}
}
