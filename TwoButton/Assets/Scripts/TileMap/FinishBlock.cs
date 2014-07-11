using UnityEngine;
using System.Collections;

public class FinishBlock : InteractiveTile {

	tk2dSpriteAnimator animator;

	void Start () {
		animator = GetComponent<tk2dSpriteAnimator>();
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
