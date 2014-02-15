﻿using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

	public string idle_spriteName = "jumper_idle";
	public string jump_spriteName = "jumper_jump";
	public string slide_spriteName = "jumper_slide";

	private tk2dSprite sprite;
	private SpritePhysics spritePhysics;
	
	public void Start() {
		sprite = this.GetComponent<tk2dSprite>();
		spritePhysics = this.GetComponent<SpritePhysics>();
	}

	public void SetSprite( string spriteName ) {
		sprite.SetSprite(spriteName);
	}

	public void DoFixedUpdate() {
		//Ground or jump:
		if ( spritePhysics.onGround ) {
			sprite.SetSprite(idle_spriteName);
		} else {
			sprite.SetSprite(jump_spriteName);
		}

		//Set facing:
		if ( spritePhysics.velocity.x > 0) {
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
		}
		if ( spritePhysics.velocity.x < 0 ) {
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
		}


		//Wallslide:
		if ( !spritePhysics.onGround && (Game.instance.right && spritePhysics.hitRight) ) {
			SetSprite(slide_spriteName);
		}
		if ( !spritePhysics.onGround && (Game.instance.left && spritePhysics.hitLeft) ) {
			SetSprite(slide_spriteName);
		}
	}




}
