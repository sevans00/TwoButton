using UnityEngine;
using System.Collections;

public class Peach : Jumper2 {

	public float MAX_FLOAT_TIME = 5f;
	private float currentFloatTime;

	// Update is called once per frame
	override protected void FixedUpdate () {
		//Controls:
		flag_jump = false;
		flag_left = false;
		flag_right = false;
		//Reset jumpcount if on ground
		if ( onGround ) {
			jumpCount = 0;
			currentFloatTime = 0; //Reset floating time
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
//			Debug.Log("Move right");
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
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
			//Slide up/down:
			spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			//Jump:
			if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				jumpDirection(Vector2.up + Vector2.right);
			}
		}
		if ( !onGround && Game.instance.right && spritePhysics.hitRight ) {
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
			//Slide up/down:
			spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			//Jump:
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				jumpDirection(Vector2.up - Vector2.right);
			}
		}


		//Float:
		if ( !onGround && Game.instance.left && Game.instance.right && spritePhysics.velocity.y <= 0 ) {
			if ( currentFloatTime < MAX_FLOAT_TIME ) {
				spritePhysics.velocity.y = -spritePhysics.gravity.y; //Set Velocity negative to counteract spritePhysics
				currentFloatTime += Time.deltaTime;
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
