using UnityEngine;
using System.Collections;

//The base physics movement class
//Ideas from: http://blog.svampson.se/post/15966101553/2d-platformer-physics-in-unity3d
[RequireComponent (typeof (BoxCollider2D))]
public class SpritePhysics : MonoBehaviour {

	public Vector2 velocity = new Vector2( 0, 0 );
	public Vector2 maxVelocity = new Vector2( 100f, 100f ); //Maximum velocity this character can travel at (for physics)
	public Vector2 gravity = new Vector2(0, -0.75f); //This is modifiable to different gravities

	public bool onGround = false;

	public bool hitTop = false;
	public bool hitBottom = false;
	public bool hitLeft = false;
	public bool hitRight = false;
	public bool hitElevator = false;
	public ElevatorBlock elevatorHit;

	public int hitTopLayer = -1;
	public int hitBottomLayer = -1;
	public int hitLeftLayer = -1;
	public int hitRightLayer = -1;


	public enum HitDirection { //Relative to this
		Undefined,
		Top,
		Bottom,
		Left,
		Right
	}

	//Gizmos (drawn in the editor window only)
	void OnDrawGizmos() {
		
	}

	//Return a list of points representing the rectangle of the sprite physics:
	public Vector2[] GetColliderBounds () {
		Vector2 position = transform.position;
		BoxCollider2D myCollider2d = GetComponent<BoxCollider2D>();
		Vector2 size = myCollider2d.size;
		Vector2 center = myCollider2d.center + position;
		//The rectangle formed by the collider:
		Rect rect = new Rect( center.x - size.x/2, center.y - size.y/2, size.x, size.y );
		Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
		Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);
		Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
		Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
		return new Vector2[] { bottomLeft, bottomRight, topLeft, topRight };
	}




	//Perform checks: (Done before interactive tiles are updated)
	public void DoDetectCollisions () {
		//Reset flags:
		hitTop = false;
		hitBottom = false;
		hitLeft = false;
		hitRight = false;
		hitElevator = false;
		//Layer flags:
		hitTopLayer = -1;
		hitBottomLayer = -1;
		hitLeftLayer = -1;
		hitRightLayer = -1;
		//Update gravity:
		if ( !onGround ) {
			velocity += gravity;
		}
		//Clip max velocity:
		clipVelocityToMax();
		//Do Collision Detection:
		detectCollisions();
	}

	// Fixed Update is called once per physics step: - Called last
	//Call this function to perform sprite physics during the Fixed Update of a sprite - will perform movement and set flags
	public void DoFixedUpdate () {
		//Update position:
		updatePosition();
	}
	
	//Perform Elevator check:
	public void DoElevatorCheck () {
		//Check for elevators:
		if ( hitElevator ) {
			this.transform.position += elevatorHit.currentDirection * elevatorHit.speed * Time.deltaTime;
		}
	}



	//Detects current and future collisions and clips velocity if velocity will cause overlap
	void detectCollisions() {
		//Ignore triggers:
		bool raycastsHitTriggers = Physics2D.raycastsHitTriggers;
		Physics2D.raycastsHitTriggers = false;

		Vector2 position = transform.position;
		BoxCollider2D myCollider2d = GetComponent<BoxCollider2D>();
		Vector2 size = myCollider2d.size;
		Vector2 center = myCollider2d.center + position;
		//The rectangle formed by the collider:
		Rect rect = new Rect( center.x - size.x/2, center.y - size.y/2, size.x, size.y );

		//We need 8 raycasts: one from each edge in each direction:
		float topPadding = size.y / 4f;
		float bottomPadding = size.y / 3f;
		float sidePadding = size.x / 10f;
		Vector2 bottomLeft = new Vector2(rect.xMin+sidePadding, rect.yMin + bottomPadding);
		Vector2 bottomRight = new Vector2(rect.xMax-sidePadding, rect.yMin + bottomPadding);
		Vector2 topLeft = new Vector2(rect.xMin+sidePadding, rect.yMax - topPadding);
		Vector2 topRight = new Vector2(rect.xMax-sidePadding, rect.yMax - topPadding);
		Vector2 topMiddle = new Vector2(rect.xMax-(size.x/2), rect.yMax-topPadding);
		Vector2 bottomMiddle = new Vector2(rect.xMax-(size.x/2), rect.yMin+bottomPadding);

		int layer = 9; //Player layer (we want to ignore this)
		int layermask = 1 << layer;
		layermask = ~(layermask);


		//-------------- timing --------------------------
		//Hittest timing (to see if I can speed things up:
		/*
		RaycastHit2D testhit;
		float startTime;
		float elapsedTime = 0f;

		startTime = Time.realtimeSinceStartup;
		for ( int kk = 0; kk < 200; kk++ ) {
			testhit = Physics2D.Raycast( bottomLeft, -Vector2.up, bottomPadding+(2f*Time.deltaTime), layermask );
		}
		elapsedTime = Time.realtimeSinceStartup - startTime;
		Debug.Log("Stopwatch time1: "+elapsedTime);

		startTime = Time.realtimeSinceStartup;
		int spriteID;
		tk2dTileMap tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		for ( int kk = 0; kk < 200; kk++ ) {
			spriteID = tilemap.GetTileIdAtPosition(transform.position, 0);
		}
		elapsedTime = Time.realtimeSinceStartup - startTime;
		Debug.Log("Stopwatch time2: "+elapsedTime);
		*/
		//-------------- timing --------------------------


		//-------------- VERTICAL HITTESTS ------------
		RaycastHit2D rayhitDownLeft;
		RaycastHit2D rayhitDownRight;
		RaycastHit2D rayhitUpLeft;
		RaycastHit2D rayhitUpRight;

		//Walking hittest: //NOTE: Stepups now broadcast hits
		if ( velocity.y == 0 ) {
//			Debug.LogWarning("WALKING HITTESTS");
			//float downDistance = Mathf.Abs(gravity.y);
			float downDistance = 2f;
			rayhitDownLeft = Physics2D.Raycast( bottomLeft, -Vector2.up, bottomPadding+(downDistance*Time.deltaTime), layermask );
			rayhitDownRight = Physics2D.Raycast( bottomRight, -Vector2.up, bottomPadding+(downDistance*Time.deltaTime), layermask );
			if ( (rayhitDownLeft.collider == null) && ( rayhitDownRight.collider == null)) {
				onGround = false;
				BroadcastMessage("OnWalkedOffEdge");
			} else {
				//Right step-up:
				if ( velocity.x > 0 ) {
					if ( rayhitDownRight.collider != null ) {
						registerHit(rayhitDownRight, HitDirection.Bottom);
						if ( rayhitDownRight.point.y > transform.position.y ) {
							transform.position = new Vector3(transform.position.x, rayhitDownRight.point.y);
						}
					}
				}
				if ( rayhitDownLeft.collider != null ) {
					registerHit(rayhitDownLeft, HitDirection.Bottom);
				}
				//Left step-up:
				if ( velocity.x < 0 ) {
					if ( rayhitDownLeft.collider != null ) {
						registerHit(rayhitDownLeft, HitDirection.Bottom);
						if ( rayhitDownLeft.point.y > transform.position.y ) {
							transform.position = new Vector3(transform.position.x, rayhitDownLeft.point.y);
						}
					}
				}
				if ( rayhitDownRight.collider != null ) {
					registerHit(rayhitDownRight, HitDirection.Bottom);
				}
			}
		}
		//Non-zero y velocity (IE: jumping or falling)
		//Falling down velocity:
		if ( velocity.y < 0 ) {
			//Debug.LogWarning("DOWN HITTESTS");
			//DOWN Hittests:
			rayhitDownLeft = Physics2D.Raycast( bottomLeft, -Vector2.up, bottomPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			rayhitDownRight = Physics2D.Raycast( bottomRight, -Vector2.up, bottomPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			//Left collision or left collision is higher
			if ( rayhitDownLeft.collider != null && (rayhitDownRight.collider == null || rayhitDownLeft.point.y >= rayhitDownRight.point.y) ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitDownLeft.point.y);
				onGround = true;
				registerHit(rayhitDownLeft, HitDirection.Bottom);
			}
			//Right collision or right collision is higher
			if ( rayhitDownRight.collider != null && (rayhitDownLeft.collider == null || rayhitDownLeft.point.y <= rayhitDownRight.point.y) ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitDownRight.point.y);
				onGround = true;
				registerHit(rayhitDownRight, HitDirection.Bottom);
			}


		}
		//UP Hittests:
		rayhitUpLeft = Physics2D.Raycast( topLeft, Vector2.up, topPadding+(velocity.y*Time.deltaTime), layermask );
		rayhitUpRight = Physics2D.Raycast( topRight, Vector2.up, topPadding+(velocity.y*Time.deltaTime), layermask );
		if ( rayhitUpLeft.collider != null && (rayhitUpRight.collider == null || rayhitUpLeft.point.y <= rayhitUpRight.point.y) ) {
			velocity.y = 0;
			transform.position = new Vector3(transform.position.x, rayhitUpLeft.point.y - size.y);
			registerHit(rayhitUpLeft, HitDirection.Top);
		}
		if ( rayhitUpRight.collider != null && (rayhitUpLeft.collider == null || rayhitUpLeft.point.y >= rayhitUpRight.point.y) ) {
			velocity.y = 0;
			transform.position = new Vector3(transform.position.x, rayhitUpRight.point.y - size.y);
			registerHit(rayhitUpRight, HitDirection.Top);
		}

		
		//-------------- HORIZONTAL HITTESTS ------------
		RaycastHit2D rayhitLeftTop;
		RaycastHit2D rayhitRightTop;
		RaycastHit2D rayhitLeftBottom;
		RaycastHit2D rayhitRightBottom;
		float hwidth = size.x/2f; //Half width

		//Zero movement hittest:
		if ( velocity.x == 0 ) {
			float flexSpace = 0.5f;
			rayhitLeftTop = Physics2D.Raycast( topMiddle, -Vector2.right, hwidth +(flexSpace*Time.deltaTime), layermask );
			rayhitLeftBottom = Physics2D.Raycast( bottomMiddle, -Vector2.right, hwidth +(flexSpace*Time.deltaTime), layermask );
			rayhitRightTop = Physics2D.Raycast( topMiddle, Vector2.right, hwidth +(flexSpace*Time.deltaTime), layermask );
			rayhitRightBottom = Physics2D.Raycast( bottomMiddle, Vector2.right, hwidth + (flexSpace*Time.deltaTime), layermask );
			if ( rayhitLeftTop.collider != null && (rayhitLeftBottom.collider == null || rayhitRightTop.point.x >= rayhitRightBottom.point.x)  ) {
				transform.position = new Vector3(rayhitLeftTop.point.x+hwidth, transform.position.y);
				registerHit(rayhitLeftTop, HitDirection.Left);
			}
			if ( rayhitLeftBottom.collider != null && (rayhitLeftTop.collider == null || rayhitRightTop.point.x <= rayhitRightBottom.point.x)  ) {
				transform.position = new Vector3(rayhitLeftBottom.point.x+hwidth, transform.position.y);
				registerHit(rayhitLeftBottom, HitDirection.Left);
			}
			if ( rayhitRightTop.collider != null && (rayhitRightBottom.collider == null || rayhitRightTop.point.x <= rayhitRightBottom.point.x) ) {
				transform.position = new Vector3(rayhitRightTop.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightTop, HitDirection.Right);
			}
			if ( rayhitRightBottom.collider != null && (rayhitRightTop.collider == null || rayhitRightTop.point.x >= rayhitRightBottom.point.x) ) {
				transform.position = new Vector3(rayhitRightBottom.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightBottom, HitDirection.Right);
			}
		}
		//Right movement hittest:
		if ( velocity.x > 0 ) {
			rayhitRightTop = Physics2D.Raycast( topMiddle, Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			rayhitRightBottom = Physics2D.Raycast( bottomMiddle, Vector2.right, hwidth + (Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitRightTop.collider != null && (rayhitRightBottom.collider == null || rayhitRightTop.point.x <= rayhitRightBottom.point.x) ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitRightTop.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightTop, HitDirection.Right);
			}
			if ( rayhitRightBottom.collider != null && (rayhitRightTop.collider == null || rayhitRightTop.point.x >= rayhitRightBottom.point.x) ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitRightBottom.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightBottom, HitDirection.Right);
			}
		}
		//Left movement hittest:
		if ( velocity.x < 0 ) {
			rayhitLeftTop = Physics2D.Raycast( topMiddle, -Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			rayhitLeftBottom = Physics2D.Raycast( bottomMiddle, -Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitLeftTop.collider != null && (rayhitLeftBottom.collider == null || rayhitRightTop.point.x >= rayhitRightBottom.point.x)  ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitLeftTop.point.x+hwidth, transform.position.y);
				registerHit(rayhitLeftTop, HitDirection.Left);
			}
			if ( rayhitLeftBottom.collider != null && (rayhitLeftTop.collider == null || rayhitRightTop.point.x <= rayhitRightBottom.point.x)  ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitLeftBottom.point.x+hwidth, transform.position.y);
				registerHit(rayhitLeftBottom, HitDirection.Left);
			}
		}



		//Debug drawing code - to see an approximation of the linecasts, change to true:
		if ( true ) {
			//rayhitDownLeft = Physics2D.Raycast( bottomLeft+(Vector2.up*innerPadding), -Vector2.up, innerPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			Debug.DrawLine(bottomLeft, bottomLeft - Vector2.up*(bottomPadding + Mathf.Abs(gravity.y*Time.deltaTime)), Color.green );
			Debug.DrawLine(bottomLeft, bottomLeft - Vector2.up*(bottomPadding + Mathf.Abs(velocity.y*Time.deltaTime)), Color.red );

			//rayhitDownRight = Physics2D.Raycast( bottomRight+(Vector2.up*innerPadding), -Vector2.up, innerPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			Debug.DrawLine(bottomRight, bottomRight - (Vector2.up)*(bottomPadding + Mathf.Abs(gravity.y*Time.deltaTime)), Color.green );
			Debug.DrawLine(bottomRight, bottomRight - (Vector2.up)*(bottomPadding + Mathf.Abs(velocity.y*Time.deltaTime)), Color.red );


			//rayhitUpLeft = Physics2D.Raycast( topLeft-(Vector2.up*innerPadding), Vector2.up, innerPadding+(velocity.y*Time.deltaTime), layermask );
			Debug.DrawLine(topLeft, 
			               topLeft + Vector2.up, Color.red );
			
			//rayhitUpRight = Physics2D.Raycast( topRight-(Vector2.up*innerPadding), Vector2.up, innerPadding+(velocity.y*Time.deltaTime), layermask );
			Debug.DrawLine(topRight, 
			               topRight + Vector2.up, Color.red );
			


			//rayhitRightTop = Physics2D.Raycast( topMiddle-(Vector2.up*topPadding), Vector2.right, innerPadding+(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			Debug.DrawLine(topMiddle-(Vector2.up*topPadding), 
			               topMiddle-(Vector2.up*topPadding) + Vector2.right, Color.red );

			//rayhitRightBottom = Physics2D.Raycast( bottomMiddle+(Vector2.up*bottomPadding), Vector2.right, innerPadding+(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			Debug.DrawLine(bottomMiddle+(Vector2.up*bottomPadding), 
			               bottomMiddle+(Vector2.up*bottomPadding) + Vector2.right, Color.red );

			//rayhitLeftTop = Physics2D.Raycast( topMiddle-(Vector2.up*topPadding), -Vector2.right, innerPadding+(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			Debug.DrawLine(topMiddle-(Vector2.up*topPadding), 
			               topMiddle-(Vector2.up*topPadding) - Vector2.right, Color.red );

			//rayhitLeftBottom = Physics2D.Raycast( bottomMiddle+(Vector2.up*bottomPadding), -Vector2.right, innerPadding+(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			Debug.DrawLine(bottomMiddle+(Vector2.up*bottomPadding), 
			               bottomMiddle+(Vector2.up*bottomPadding) - Vector2.right, Color.red );

		}

		//Restore raycastsHitTriggers:
		Physics2D.raycastsHitTriggers = raycastsHitTriggers;
	}

	//Register a hit
	private void registerHit ( RaycastHit2D hit, HitDirection direction ) {
		if ( hit.collider != null ) { //Hey, a collision happened!
			if ( hit.collider.GetComponent<CrumbleBlock>() != null ) {
				//Debug.Log("Collider!");
				hit.collider.GetComponent<CrumbleBlock>().OnSpritePhysicsCollision(collider2D);
			} else {
				//hit.collider.BroadcastMessage("OnSpritePhysicsCollision", collider2D, SendMessageOptions.DontRequireReceiver);
			}
		}
		if ( hit.collider.gameObject.layer == 8 ) { //Kill layer, ala Discworld
			BroadcastMessage("Kill", SendMessageOptions.DontRequireReceiver); //Kill!
			return;
		}

		int hitLayer = hit.collider.gameObject.layer;
		switch ( direction ) {
		case HitDirection.Top: hitTop = true;
			hitTopLayer = hitLayer;
			break;
		case HitDirection.Bottom: hitBottom = true;
			hitBottomLayer = hitLayer;
			break;
		case HitDirection.Left: hitLeft = true;
			hitLeftLayer = hitLayer;
			break;
		case HitDirection.Right: hitRight = true; 
			hitRightLayer = hitLayer;
			break;
		}
		if ( hit.collider.gameObject.layer == LayerMask.NameToLayer("Elevator") && !hitElevator) {
//			Debug.LogWarning("Hit elevator!");
			hitElevator = true;
			elevatorHit = hit.collider.gameObject.GetComponent<ElevatorBlock>();
			if ( elevatorHit.currentDirection.Equals ( Vector3.down ) && direction == HitDirection.Bottom ) {
				onGround = true;
			}
			if ( elevatorHit.currentDirection.Equals ( Vector3.down ) && direction == HitDirection.Top ) {
				//Set vertical velocity to the elevator's:
				velocity.y = elevatorHit.currentDirection.y * elevatorHit.speed;
			}
		}
	}






	//Test if a given line hits
	private bool hitTest ( Vector2 source, Vector2 dest ) {
		//Debug.DrawLine( new Vector3(source.x, source.y, -1), new Vector3(dest.x, dest.y, -1) );
		RaycastHit2D []hits = new RaycastHit2D[0];
		hits = Physics2D.LinecastAll( source, dest, 9 ); //Layer 9 is the player, ignore this layer
		return hits.Length >= 1;
	}
	



	//Clips the character's velocity to maximum (returns true if clipped)
	bool clipVelocityToMax () {
		bool clipPerformed = false;
		//Clip x:
		if ( velocity.x > maxVelocity.x ) {
			velocity.x = maxVelocity.x;
			clipPerformed = true;
		}
		else if ( velocity.x < -maxVelocity.x ) {
			velocity.x = -maxVelocity.x;
			clipPerformed = true;
		}
		//Clip y:
		if ( velocity.y > maxVelocity.y ) {
			velocity.y = maxVelocity.y;
			clipPerformed = true;
		}
		else if ( velocity.y < -maxVelocity.y ) {
			velocity.y = -maxVelocity.y;
			clipPerformed = true;
		}
		return clipPerformed;
	}

	//Updates the character's position:
	void updatePosition() {
		Vector3 velocity3 = new Vector3( velocity.x ,
		                                 velocity.y  );
		if ( velocity3.sqrMagnitude != 0 ) { //SqrMagnitude is faster
			transform.position = Vector3.MoveTowards(transform.position, transform.position + velocity3, Time.deltaTime*velocity3.magnitude);
		}
	}




}
