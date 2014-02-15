using UnityEngine;
using System.Collections;

public class Jumper : MonoBehaviour {

	public GameObject gibsPrefab;

	public bool onGround = false;

	private bool isDead = false;

	public tk2dSprite sprite;

	private float JUMP_SPEED = 10.0f;

	private float walkAccelleration = 0.9f;
	private float turnMultiplier = 1.2f;
	private float MAX_SPEED = 10.0f;

	public bool flag_left = false;
	public bool flag_right = false;
	public bool flag_jump = false;



	// Use this for initialization
	void Start () {
		sprite = GetComponent<tk2dSprite>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Animation:
		//Debug.Log("OnGround:"+onGround);
		if ( onGround ) {
			sprite.SetSprite("jumper_idle");
		} else {
			sprite.SetSprite("jumper_jump");
		}
		if ( rigidbody2D.velocity.x > 0) {
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z );
		}
		if ( rigidbody2D.velocity.x < 0 ) {
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z );
		}

		//Controls:
		flag_jump = false;
		flag_left = false;
		flag_right = false;
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

		//Movement:
		if ( flag_left ) {
			moveLeft();
		}
		if ( flag_right ) {
			moveRight();
		}
		//Halt movement if on ground and not moving either way:
		if ( onGround && !flag_left && !flag_right ) {
			rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
		}
		if ( flag_jump ) {
			jump();
		}
		if ( Input.GetKeyUp(KeyCode.W) ) {
			stopJump();
		}
		if ( Mathf.Abs( rigidbody2D.velocity.x ) > MAX_SPEED ) {
			float sign = 1f;
			if ( rigidbody2D.velocity.x < 0 ) {
				sign = -1f;
			}
			rigidbody2D.velocity = new Vector2(sign*MAX_SPEED, rigidbody2D.velocity.y);
		}
	}

	
	



	public void moveLeft() {
		move (-Vector2.right);
	}
	public void moveRight() {
		move ( Vector2.right);
	}
	private void move( Vector2 direction ) {
		float sign = signOf(direction.x);
		float currentSign = signOf(rigidbody2D.velocity.x);
		float v = walkAccelleration;
		if ( currentSign != 0 && currentSign != sign ) {
			v *= turnMultiplier;
		}
		rigidbody2D.velocity += direction * v;
	}
	private float signOf ( float number ) {
		if ( number == 0 ) {
			return 0;
		}
		if ( number < 0 ) {
			return -1;
		}
		return 1;
	}

	public void jump() {
		if ( onGround ) {
			onGround = false;
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, JUMP_SPEED );
		}
	}
	public void stopJump() {
		if ( rigidbody2D.velocity.y > 0 ) {
			rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0 );
		}
	}


	void OnCollisionEnter2D ( Collision2D collision ) {
		//Debug.Log("Collision enter!");
		foreach (ContactPoint2D contact in collision.contacts) {
			if ( contact.collider.gameObject.layer == 8) {
				Kill ();
			}
			if ( contact.normal.y > 0.1f ) {
				onGround = true;
			}
		}
	}

	void OnCollisionStay2D ( Collision2D collision ) {
		//Debug.Log("Collision enter!");
		foreach (ContactPoint2D contact in collision.contacts) {
			Debug.DrawLine( new Vector3(contact.point.x, contact.point.y, -1), new Vector3(contact.normal.x + contact.point.x, contact.normal.y+contact.point.y, -1) );

			if ( contact.normal.y > 0.1f ) {
				onGround = true;
				//break;
			}
		}
	}
	/*
	void OnCollisionExit2D ( Collision2D collision ) {
		//Debug.Log("Collision enter!");
		foreach (ContactPoint2D contact in collision.contacts) {
			if ( contact.normal.y > 0 ) {
				//onGround = false;
				break;
			}
		}
	}
	*/


	//Kill this jumper
	void Kill () {
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
