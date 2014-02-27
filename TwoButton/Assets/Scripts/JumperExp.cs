using UnityEngine;
using System.Collections;

public class JumperExp : Jumper2 {

	private bool inputRight = false;
	private bool inputLeft = false;
	private float rightTime = 0f;
	private float leftTime = 0f;

	public float wall_pushoff_percent = 0.5f;

	// Update is called once per frame
	override protected void FixedUpdate () {
		//SET INPUT FLAGS:
		//Keys:
		bool rightnew = false;
		bool leftnew = false;
		bool jumpnew = false;
		if ( Input.touchCount > 0 ) {
			//Multitouch:
			foreach ( Touch touch in Input.touches ) {
				//Jumping check:
				if ( touch.position.y < Screen.height - Screen.height / 3 ) {
					if ( touch.position.x > Screen.width / 2 ) {
						rightnew = true;
					}
					if ( touch.position.x < Screen.width / 2 ) {
						leftnew = true;
					}
				}
				//Jumping:
				if ( touch.position.y > Screen.height - Screen.height / 3 ) {
					jumpnew = true;
				}
			}
		}
		//Keyboard for debugging:
		if ( Input.GetKey(KeyCode.A) ) {
			leftnew = true;
		}
		if ( Input.GetKey(KeyCode.D) ) {
			rightnew = true;
		}
		if ( Input.GetKey(KeyCode.W) ) {
			jumpnew = true;
		}
		//Actually set left and right:
		if ( !inputRight && rightnew ) {
			rightTime = Time.time;
		}
		if ( !inputLeft && leftnew ) {
			leftTime = Time.time;
		}
		inputRight = rightnew;
		inputLeft = leftnew;

		//Reset jumpcount if on ground
		if ( onGround ) {
			jumpCount = 0;
		}

		//Wall slide:
		bool wallSlide_right = false;
		bool wallSlide_left = false;
		if ( !onGround && spritePhysics.hitLeft ) { //Game.instance.left && 
			wallSlide_left = true;
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
		}
		if ( !onGround && spritePhysics.hitRight ) { //Game.instance.right && 
			wallSlide_right = true;
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
		}


		



		//Ground jump:
		if ( onGround && inputLeft && inputRight ) {
			onGround = false;
			jump();
		}


		//Ground / Air Horizontal Movement:
		if ( inputLeft ) {
//			Debug.Log("Move left");
			moveLeft();
		}
		if ( inputRight ) {
//			Debug.Log("Move right");
			moveRight();
		}


		//Halt movement if on ground and not moving either way:
		if ( onGround && !inputLeft && !inputRight ) {
			if ( spritePhysics.hitBottomLayer == LayerMask.NameToLayer("Ice") ) { //ICE
				//No slowdown!
			} else {
				spritePhysics.velocity = new Vector2(0, spritePhysics.velocity.y);
			}
		}





		//Wall Jump:
		if ( wallSlide_right ) {
			if ( inputLeft && last_jump_time < leftTime ) {
				if ( inputRight ) {
					jumpDirection(new Vector2( -wall_pushoff_percent, 1f ), JUMP_SPEED * 0.7f);
				} else {
					jumpDirection(new Vector2( 0f, 1f ), JUMP_SPEED * 0.7f);
				}
				//jumpDirection(Vector2.up - Vector2.right, JUMP_SPEED * 0.7f);
				//jump2(JUMP_SPEED * 0.7f);
			}
		}
		if ( wallSlide_left ) {
			if ( inputRight && last_jump_time < rightTime ) {
				if ( inputLeft ) {
					jumpDirection(new Vector2( wall_pushoff_percent, 1f ), JUMP_SPEED * 0.7f);
				} else {
					jumpDirection(new Vector2( 0f, 1f ), JUMP_SPEED * 0.7f);
				}
				//jumpDirection(Vector2.up + Vector2.right, JUMP_SPEED * 0.7f);
				//jump2(JUMP_SPEED * 0.7f);
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

	public void jump2( float speed ) {
		jumpCount++;
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x, speed );
		last_jump_time = Time.time;
	}
	public void jumpDirection( Vector2 direction, float speed ) {
//		jumpCount = 1; //Uncomment to make walljumps reset double jump
		direction.Normalize();
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x + speed*direction.x, speed*direction.y );
		last_jump_time = Time.time;
	}

	//Wall slide:
	override public void doWallSlide() {
		//Set wallslide time:
		if ( !onGround && spritePhysics.hitLeft ) { //Game.instance.left && 
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			lastWallSlideTime = Time.time;
		}
		if ( !onGround && spritePhysics.hitRight ) { //Game.instance.right && 
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			lastWallSlideTime = Time.time;
		}

		//Do jumping:
		if ( lastWallSlideTime > Time.time - 0.1f ) {
			//Jump:
			if ( !onGround && spritePhysics.hitRight && Game.instance.right ) //Game.instance.right && 
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				//jumpDirection(Vector2.up - Vector2.right);
				jump2(JUMP_SPEED * 0.7f);
			}
			//Jump:
			if ( !onGround && spritePhysics.hitLeft && Game.instance.left ) //Game.instance.left && 
				if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				//jumpDirection(Vector2.up + Vector2.right);
				jump2(JUMP_SPEED * 0.7f);
			}
		}
	}

}
