using UnityEngine;
using System.Collections;

public class Jumper2 : MonoBehaviour {

	public GameObject gibsPrefab;

	//Getter for onGround:
	public bool onGround {
		get{
			if ( spritePhysics != null) {
				return spritePhysics.onGround;
			}
			return false; }
		set{
			spritePhysics.onGround = value;
		}
	}


	public tk2dSprite sprite;
	public SpritePhysics spritePhysics;

	public float JUMP_SPEED = 25.0f;

	public float walkAccelleration = 0.9f;
	public float airAccelleration = 2.0f; //
	public float turnMultiplier = 1.2f;
	public float airTurnMultiplier = 1.2f;
	public float MAX_SPEED = 20.0f;

	public float WALL_SLIDE_SPEED = 5f;

	protected bool flag_left = false;
	protected bool flag_right = false;
	protected bool flag_jump = false;

	//For wall-jumping
	protected float last_jump_time = 0f;

	//Double-jumping:
	public int MaxJumps = 1;
	protected int jumpCount = 0;

	//Avoid triggering death too many times:
	protected bool isDead = false;
	
	// Use this for initialization
	virtual protected void Start () {
		sprite = GetComponent<tk2dSprite>();
		spritePhysics = GetComponent<SpritePhysics>();
	}
	
	// Update is called once per frame
	virtual protected void FixedUpdate () {
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
		if ( Game.instance.jump ) { //Jump flag
			flag_jump = true;
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
			if ( spritePhysics.hitBottomLayer == LayerMask.NameToLayer("Ice") ) { //ICE
				//No slowdown!
			} else {
				spritePhysics.velocity = new Vector2(0, spritePhysics.velocity.y);
			}
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

		doWallSlide();

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

	public void OnWalkedOffEdge() {
		jumpCount++; //Walking off the edge counts as a jump
	}

	public void moveLeft() {
		move (-Vector2.right);
	}
	public void moveRight() {
		move ( Vector2.right);
	}
	protected void move( Vector2 direction ) {
		float sign = signOf(direction.x);
		float currentSign = signOf(spritePhysics.velocity.x);
		float v = walkAccelleration;
		if ( !onGround ) {
			v = airAccelleration;
		}
		if ( currentSign != 0 && currentSign != sign ) {
			if ( spritePhysics.hitBottomLayer != LayerMask.NameToLayer("Ice") ) { //Turning is harder on ice
				if ( onGround ) {
					v *= turnMultiplier;
				} else { 
					v *= airTurnMultiplier;
				}
			}
		}
		spritePhysics.velocity += direction * v;
	}
	protected float signOf ( float number ) {
		if ( number == 0 ) {
			return 0;
		}
		if ( number < 0 ) {
			return -1;
		}
		return 1;
	}

	virtual public void jump() {
		jumpCount++;
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x, JUMP_SPEED );
		last_jump_time = Time.time;
	}
	virtual public void jumpDirection( Vector2 direction ) {
//		jumpCount = 1; //Uncomment to make walljumps reset double jump
		direction.Normalize();
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x + JUMP_SPEED*direction.x, JUMP_SPEED*direction.y );
		last_jump_time = Time.time;
	}
	public void stopJump() {
		if ( spritePhysics.velocity.y > 0 ) {
			spritePhysics.velocity = new Vector2(spritePhysics.velocity.x, 0 );
		}
	}

	//Wall slide:
	protected float lastWallSlideTime = 0f;
	virtual public void doWallSlide() {
		
		if ( lastWallSlideTime > Time.time - 0.1f ) {
			//Jump:
			if ( !onGround && spritePhysics.hitRight ) //Game.instance.right && 
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				jumpDirection(Vector2.up - Vector2.right);
			}
			//Jump:
			if ( !onGround && spritePhysics.hitLeft ) //Game.instance.left && 
				if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				jumpDirection(Vector2.up + Vector2.right);
			}
		}

		//Do wall slide and/or jump:
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
	}
	//First wall slide code:
	virtual public void doEasyWallSlide() {
		//Do wall slide and/or jump:
		if ( !onGround && spritePhysics.hitLeft ) { //Game.instance.left && 
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			//Jump:
			if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				jumpDirection(Vector2.up + Vector2.right);
			}
		}
		if ( !onGround && spritePhysics.hitRight ) { //Game.instance.right && 
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			//Jump:
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				jumpDirection(Vector2.up - Vector2.right);
			}
		}
	}







	//Kill this jumper
	void Kill () {
		Debug.Log("Kill recieved!");
		if ( isDead ) {
			return;
		}
		isDead = true;
		Debug.Log("Gameover1");
		//Gibs!
		if ( gibsPrefab != null ) {
			Instantiate(gibsPrefab, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
		Debug.Log("Gameover2");
		if ( Game.instance != null ) {
			Game.instance.GameOver();
		}
	}

}
