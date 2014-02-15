using UnityEngine;
using System.Collections;

public class Puff : Jumper2 {

	// Update is called once per frame
	override protected void FixedUpdate () {
		//Controls:
		flag_jump = false;
		flag_left = false;
		flag_right = false;
		//Reset jumpcount if on ground
		if ( onGround ) {
			jumpCount = 0;
		}
		if ( Game.instance.left && Game.instance.right ) {
			flag_jump = true;
			if ( Game.instance.leftTime > Game.instance.rightTime ) {
				flag_right = true;
			} else {
				flag_left = true;
			}
		} else {
			if ( Game.instance.left ) {
				flag_left = true;
			}
			if ( Game.instance.right ) {
				flag_right = true;
			}
		}



		//Ground / Air Movement:
		if ( flag_left ) {
//			Debug.Log("Move left");
			moveLeft();
		}
		if ( flag_right ) {
			moveRight();
		}
		//Halt movement if on ground and not moving either way:
		if ( onGround && !flag_left && !flag_right ) {
			spritePhysics.velocity = new Vector2(0, spritePhysics.velocity.y);
		}
		//Ground jump:
		if ( onGround && flag_jump ) {
			onGround = false;
			jump();
		}
		//Double jumps:
		if ( !onGround && MaxJumps > jumpCount && flag_jump ) {
			//Do double jump:
			if ( Game.instance.leftTime > last_jump_time || Game.instance.rightTime > last_jump_time ) {
				jump ();
			}
		}

		//Clamp x speed:
		if ( Mathf.Abs( spritePhysics.velocity.x ) > MAX_SPEED ) {
			float sign = 1f;
			if ( spritePhysics.velocity.x < 0 ) {
				sign = -1f;
			}
			spritePhysics.velocity = new Vector2(sign*MAX_SPEED, spritePhysics.velocity.y);
		}

		//Call Sprite physics to do the actual movement and set flags for next update:
		spritePhysics.DoFixedUpdate();
		//Animate the sprite:
		GetComponent<CharacterAnimator>().DoFixedUpdate();
	}

	override public void jump() {
		jumpCount++;
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x, JUMP_SPEED * (1.1f - jumpCount/MaxJumps ) );
		last_jump_time = Time.time;
	}

}
