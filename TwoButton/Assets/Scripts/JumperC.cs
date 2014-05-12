using UnityEngine;
using System.Collections;

public class JumperC : Jumper2 {
	
	private bool onGroundLastFrame = false;
	private float lastLandingTime = 0f;
	
	private bool inputRight = false;
	private bool inputLeft = false;
	private float rightTime = 0f;
	private float leftTime = 0f;
	private float rightTimeUp = 0f;
	private float leftTimeUp = 0f;
	
	private bool rightJump = false;
	private bool leftJump = false;
	private bool rightJumpLastFrame = false;
	private bool leftJumpLastFrame = false;
	private bool controllableHeightJumps = false;
	
	private bool inputJump = false;
	private float jumpTime = 0f;
	
	private float timeHeldRightAwayFromWall = 0f;
	private float timeHeldLeftAwayFromWall = 0f;
	private float totalStickToWallTime = 0.5f; //CONSTANT
	
	private bool stickingToLeftWall = false;
	private bool stickingToRightWall = false;
	
	private bool doing_walljump_stickyfriction = false;
	private float walljump_stickyfriction_time = 0f;
	private float total_walljump_stickyfriction_time = 0.15f; //The little "mini-animation" of character preparing to launch
	private float max_walljump_betweenbutton_time = 0.0f; //MaxTime between alternate button presses (make it easier to walljump)
	
	protected override void Start ()
	{
		//		walkAccelleration = 1.0f;
		//		airAccelleration  = 2.0f;
		//		airTurnMultiplier = 2.0f;
		//		turnMultiplier    = 2.2f;
		base.Start ();
	}
	
	
	delegate void WalljumpDelegate ();
	WalljumpDelegate walljumpDelegate;
	
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
		
		
		
		
		
		//Reset ground jumping overrides:
		if ( (!rightnew || !leftnew) && rightJump) {
			rightJump = false;
		}
		if ( (!leftnew || !rightnew) && leftJump ) {
			leftJump = false;
		}
		//Do ground jumping overrides: //But if we release the other button mid jump, we've got our input back
		if ( rightnew && rightJump && leftnew ) {
			rightnew = false;
		}
		if ( leftnew && leftJump && rightnew ) {
			leftnew = false;
		}
		
		
		
		
		
		//Actually set left and right:
		if ( !inputRight && rightnew ) { //This is the frame where right went down
			rightTime = Time.time;
		}
		if ( !inputLeft && leftnew ) { //This is the frame where left went down
			leftTime = Time.time;
		}
		if ( inputRight && !rightnew ) { //This is the frame where right went up
			rightTimeUp = Time.time;
		}
		if ( inputLeft && !leftnew ) { //This is the frame where left went up
			leftTimeUp = Time.time;
		}
		inputRight = rightnew;
		inputLeft = leftnew;
		
		
		
		//Reset jumpcount if on ground
		if ( onGround ) {
			jumpCount = 0;
		}
		
		
		
		
		
		
		
		
		
		
		
		
		if ( !onGroundLastFrame && onGround ) {
			lastLandingTime = Time.time;
		}
		
		//Ground jump:
		/*if ( onGround && ( (inputLeft && inputRight) || jumpnew ) ) {
		//if ( onGround && jumpnew ) {
			onGround = false;
			jump();
		}*/
		if ( onGround && ( (inputLeft && inputRight && (rightTime >= lastLandingTime || leftTime >= lastLandingTime)) || jumpnew ) ) {
			//if ( onGround && jumpnew ) {
			if ( leftTime > rightTime ) {
				leftJump = true;
				inputLeft = false;
			}
			if ( leftTime < rightTime ) {
				rightJump = true;
				inputRight = false;
			}
			onGround = false;
			jump();
		}
		if ( controllableHeightJumps ) {
			//Ground Jump Height Control:
			if ( !onGround && !leftJump && leftJumpLastFrame && spritePhysics.velocity.y > 0 ) {
				spritePhysics.velocity.y = -spritePhysics.gravity.y;
			}
			if ( !onGround && !rightJump && rightJumpLastFrame && spritePhysics.velocity.y > 0 ) {
				spritePhysics.velocity.y = -spritePhysics.gravity.y;
			}
		}
		leftJumpLastFrame = leftJump;
		rightJumpLastFrame = rightJump;
		
		
		
		
		//Wall slide:
		bool wallSlide_right = false;
		bool wallSlide_left = false;
		if ( !onGround && spritePhysics.hitLeft ) { //inputLeft && 
			wallSlide_left = true;
			lastWallSlideTime = Time.time;
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( inputLeft && spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
		} else {
			timeHeldRightAwayFromWall = 0f;
		}
		if ( !onGround && spritePhysics.hitRight ) { //inputRight && 
			wallSlide_right = true;
			lastWallSlideTime = Time.time;
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( inputRight && spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
		} else {
			timeHeldLeftAwayFromWall = 0f;
		}
		//Wall slide stickyness:
		stickingToLeftWall = true;
		stickingToRightWall = true;
		if ( inputRight && wallSlide_left && !inputLeft ) {
			timeHeldRightAwayFromWall += Time.deltaTime;
			if ( timeHeldRightAwayFromWall > totalStickToWallTime ) {
				stickingToLeftWall = false;
			}
			if ( Time.time - leftTimeUp <= max_walljump_betweenbutton_time ) {//Time-delayed walljump here:
				//doLeftWalljump();
			}
		}
		if ( inputLeft && wallSlide_right && !inputRight ) {
			timeHeldLeftAwayFromWall += Time.deltaTime;
			if ( timeHeldLeftAwayFromWall > totalStickToWallTime ) { //Slow descent
				stickingToRightWall = false;
			}
			if ( Time.time - rightTimeUp <= max_walljump_betweenbutton_time ) { //Time-delayed walljump here:
				//doRightWalljump();
			}
		}
		//Ground / Air Horizontal Movement:
		if ( inputLeft ) {
			//			Debug.Log("Move left");
			if ( wallSlide_right ) {
				if ( !stickingToRightWall ) {
					moveLeft();
				}
			} else {
				moveLeft();
			}
		}
		if ( inputRight ) {
			//			Debug.Log("Move right");
			if ( wallSlide_left ) {
				if ( !stickingToLeftWall ) {
					moveRight();
					wallSlide_right = false; //
				}
			} else {
				moveRight();
				wallSlide_left = false; //
			}
		}
		
		
		//Halt horizontal movement if on ground and not moving either way:
		if ( onGround && !inputLeft && !inputRight ) {
			if ( spritePhysics.hitBottomLayer == LayerMask.NameToLayer("Ice") ) { //ICE
				//No slowdown!
			} else {
				spritePhysics.velocity = new Vector2(0, spritePhysics.velocity.y);
			}
		}
		
		
		
		
		
		
		//Wall Jump:
		if ( wallSlide_right ) {
			if ( inputRight && rightTime >= lastWallSlideTime ) {
				jumpDirection(Vector2.up - Vector2.right);
			}
			if ( inputLeft && leftTime >= lastWallSlideTime ) {
				if ( inputRight ) {
					leftJump = true;
				}
				jumpDirection(Vector2.up - Vector2.right);
			}
		}
		if ( wallSlide_left ) {
			if ( inputLeft && leftTime >= lastWallSlideTime ) {
				jumpDirection(Vector2.up + Vector2.right);
			}
			//if ( false && inputRight && rightTime > leftTime && last_jump_time < rightTime && rightTime >= lastWallSlideTime ) {
			//rightJump = true;
			if ( inputRight && rightTime >= lastWallSlideTime ) {
				if ( inputLeft ) {
					rightJump = true;
				}
				jumpDirection(Vector2.up + Vector2.right);
			}
		}
		
		if ( controllableHeightJumps ) {
			//Not implemented for wallslide yet
		}
		
		
		
		
		
		
		
		
		
		onGroundLastFrame = onGround;
		
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
	
	
	//WALL JUMPING:
	public void doRightWalljump () {
		spritePhysics.velocity.y = 0f;
		jumpDirection(new Vector2( -1f, 1f ), JUMP_SPEED);
	}
	public void doLeftWalljump () {
		spritePhysics.velocity.y = 0f;
		jumpDirection(new Vector2( 1f, 1f ), JUMP_SPEED);
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
	
	protected override void Kill () {
		base.Kill();
	}
	
}
