using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

	public string idle_animationName = "idle";
	public string jump_animationName = "jump";
	public string slide_animationName = "wallslide";
	public string run_animationName = "run";
	public string victory_animationName = "victory";

	private tk2dSprite sprite;
	public tk2dSpriteAnimator spriteAnimator;
	private SpritePhysics spritePhysics;

	private bool facingRight = true;

	public void Start() {
		sprite = this.GetComponent<tk2dSprite>();
		spritePhysics = this.GetComponent<SpritePhysics>();
		spriteAnimator = this.GetComponent<tk2dSpriteAnimator>();
	}

	public void SetSprite( string spriteName ) {
		sprite.SetSprite(spriteName);
	}

	public void DoFixedUpdate() {
		if ( sprite == null ) {
			return;
		}
		//Ground or jump:
		if ( spritePhysics.onGround ) {
			if ( spritePhysics.velocity.x != 0 || Game.instance.left || Game.instance.right ) {
				spriteAnimator.Play(run_animationName);
			} else {
				spriteAnimator.Play(idle_animationName);
			}
		} else {
			spriteAnimator.Play(jump_animationName);
		}

		//Set facing:
		if ( spritePhysics.velocity.x > 0 ) {
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
		}
		if ( spritePhysics.velocity.x < 0 ) {
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
		}

		//Wallslide:
		if ( !spritePhysics.onGround && (spritePhysics.hitRight) ) { //Game.instance.right && 
			spriteAnimator.Play(slide_animationName);
		}
		if ( !spritePhysics.onGround && (spritePhysics.hitLeft) ) { //Game.instance.left && 
			spriteAnimator.Play(slide_animationName);
		}
	}




}
