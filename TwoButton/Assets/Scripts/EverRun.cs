using UnityEngine;
using System.Collections;


//Everrun - will always move in the direction of last motion
public class EverRun : Jumper2 {

	// FixedUpdate is called once per physics step
	override protected void FixedUpdate () {
		//This big if section is to do changing direction at the peak of a jump when jumping in one direction and releasing the button in the same direction as the jump.
		if ( flag_jump && (!Game.instance.left || !Game.instance.right) ) {
			if ( Game.instance.leftTime > Game.instance.rightTime ) {
				if ( Game.instance.right ) {
					Game.instance.leftTime = 0f;
				}
			} else { 
				if ( Game.instance.left ) {
					Game.instance.rightTime = 0f;
				}
			}
		}
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
			if ( Game.instance.leftTime < Game.instance.rightTime ) {
				flag_left = true;
			} else {
				flag_right = true;
			}
		} else {
			if ( Game.instance.leftTime > Game.instance.rightTime ) {
				flag_left = true;
			} else {
				flag_right = true;
			}
		}

		
		
		//Ground / Air Movement:
		if ( flag_left ) {
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
				Debug.Log("Double Jump!");
				jump ();
			}
		}
		
		//Do wall slide and/or jump:
		if ( !onGround && Game.instance.left && spritePhysics.hitLeft ) {
			//			Debug.Log("Slide left");
			//			sprite.SetSprite("jumper_slide");
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
			//Slide up/down:
			spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			//Jump:
			if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				//				Debug.Log("Jump from left");
				//Game.instance.leftTime = Time.time; //We're going right now!
				//Game.instance.LeftUp();
				jumpDirection(Vector2.up + Vector2.right);
			}
		}
		if ( !onGround && Game.instance.right && spritePhysics.hitRight ) {
			//			Debug.Log("Slide right");
			//			sprite.SetSprite("jumper_slide");
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
			//Slide up/down:
			spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			//Jump:
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				//				Debug.Log("Jump from right");
				//Game.instance.rightTime = Time.time; //We're going left now!
				//Game.instance.RightUp();
				jumpDirection(Vector2.up - Vector2.right);
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

}
