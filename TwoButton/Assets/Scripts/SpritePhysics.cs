using UnityEngine;
using System.Collections;

//The base physics movement class
//Modified from: http://blog.svampson.se/post/15966101553/2d-platformer-physics-in-unity3d
public class SpritePhysics : MonoBehaviour {

	public Vector2 velocity = new Vector2( 0, 0 );
	public Vector2 maxVelocity = new Vector2( 100f, 100f ); //Maximum velocity this character can travel at (for physics)
	public Vector2 gravity = new Vector2(0, -0.75f); //This is modifiable to different gravities

	public bool onGround = false;

	public bool hitTop = false;
	public bool hitBottom = false;
	public bool hitLeft = false;
	public bool hitRight = false;

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

	// Fixed Update is called once per physics step:
	//Call this function to perform sprite physics during the Fixed Update of a sprite - will perform movement and set flags
	public void DoFixedUpdate () {
		//Reset flags:
		hitTop = false;
		hitBottom = false;
		hitLeft = false;
		hitRight = false;
		//Update gravity:
		if ( !onGround ) {
			velocity += gravity; //TODO: should this be only if not on ground?
		}
		//Clip max velocity:
		clipVelocityToMax();
		//Do Collision Detection:
		detectCollisions();
		//Update position:
		updatePosition();
	}


	//Detects current and future collisions and clips velocity if velocity will cause overlap
	void detectCollisions() {
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

		
		//-------------- VERTICAL HITTESTS ------------
		RaycastHit2D rayhitDownLeft;
		RaycastHit2D rayhitDownRight;
		RaycastHit2D rayhitUpLeft;
		RaycastHit2D rayhitUpRight;

		//Walking hittest: //NOTE: Stepups currently do not broadcast hits
		if ( velocity.y == 0 ) {
			rayhitDownLeft = Physics2D.Raycast( bottomLeft, -Vector2.up, bottomPadding+(Mathf.Abs(gravity.y)*Time.deltaTime), layermask );
			rayhitDownRight = Physics2D.Raycast( bottomRight, -Vector2.up, bottomPadding+(Mathf.Abs(gravity.y)*Time.deltaTime), layermask );
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
				//Left step-up:
				if ( velocity.x < 0 ) {
					if ( rayhitDownLeft.collider != null ) {
						registerHit(rayhitDownLeft, HitDirection.Bottom);
						if ( rayhitDownLeft.point.y > transform.position.y ) {
							transform.position = new Vector3(transform.position.x, rayhitDownLeft.point.y);
						}
					}
				}
			}
		}
		//Falling hittest:
		if ( velocity.y < 0 ) {
			rayhitDownLeft = Physics2D.Raycast( bottomLeft, -Vector2.up, bottomPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			if ( rayhitDownLeft.collider != null ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitDownLeft.point.y);
				onGround = true;
				registerHit(rayhitDownLeft, HitDirection.Bottom);
			}
			rayhitDownRight = Physics2D.Raycast( bottomRight, -Vector2.up, bottomPadding+(Mathf.Abs(velocity.y)*Time.deltaTime), layermask );
			if ( rayhitDownRight.collider != null ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitDownRight.point.y);
				onGround = true;
				registerHit(rayhitDownRight, HitDirection.Bottom);
			}
		}
		//Jumping hittest:
		if ( velocity.y > 0 ) {
			rayhitUpLeft = Physics2D.Raycast( topLeft, Vector2.up, topPadding+(velocity.y*Time.deltaTime), layermask );
			if ( rayhitUpLeft.collider != null ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitUpLeft.point.y - size.y);
				registerHit(rayhitUpLeft, HitDirection.Top);
			}
			rayhitUpRight = Physics2D.Raycast( topRight, Vector2.up, topPadding+(velocity.y*Time.deltaTime), layermask );
			if ( rayhitUpRight.collider != null ) {
				velocity.y = 0;
				transform.position = new Vector3(transform.position.x, rayhitUpRight.point.y - size.y);
				registerHit(rayhitUpRight, HitDirection.Top);
			}
		}

		
		//-------------- HORIZONTAL HITTESTS ------------
		RaycastHit2D rayhitLeftTop;
		RaycastHit2D rayhitRightTop;
		RaycastHit2D rayhitLeftBottom;
		RaycastHit2D rayhitRightBottom;
		float hwidth = size.x/2f; //Half width
		//Right movement hittest:
		if ( velocity.x > 0 ) {
			rayhitRightTop = Physics2D.Raycast( topMiddle, Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitRightTop.collider != null ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitRightTop.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightTop, HitDirection.Right);
			}
			rayhitRightBottom = Physics2D.Raycast( bottomMiddle, Vector2.right, hwidth + (Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitRightBottom.collider != null ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitRightBottom.point.x-hwidth, transform.position.y);
				registerHit(rayhitRightBottom, HitDirection.Right);
			}
		}
		//Left movement hittest:
		if ( velocity.x < 0 ) {
			rayhitLeftTop = Physics2D.Raycast( topMiddle, -Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitLeftTop.collider != null ) {
				velocity.x = 0;
				transform.position = new Vector3(rayhitLeftTop.point.x+hwidth, transform.position.y);
				registerHit(rayhitLeftTop, HitDirection.Left);
			}
			rayhitLeftBottom = Physics2D.Raycast( bottomMiddle, -Vector2.right, hwidth +(Mathf.Abs(velocity.x)*Time.deltaTime), layermask );
			if ( rayhitLeftBottom.collider != null ) {
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
	}
	private void registerHit ( RaycastHit2D hit, HitDirection direction ) {
		if ( hit.collider != null ) { //Hey, a collision happened!
			hit.collider.gameObject.BroadcastMessage("OnSpritePhysicsCollision", collider2D, SendMessageOptions.DontRequireReceiver);
		}
		if ( hit.collider.gameObject.layer == 8 ) {
			BroadcastMessage("Kill", SendMessageOptions.DontRequireReceiver); //Kill!
			return;
		}
		switch ( direction ) {
		case HitDirection.Top: hitTop = true; break;
		case HitDirection.Bottom: hitBottom = true; break;
		case HitDirection.Left: hitLeft = true; break;
		case HitDirection.Right: hitRight = true; break;
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
		if ( velocity3.sqrMagnitude != 0 ) {
			transform.position = Vector3.MoveTowards(transform.position, transform.position + velocity3, Time.deltaTime*velocity3.magnitude);
		}
	}




}
