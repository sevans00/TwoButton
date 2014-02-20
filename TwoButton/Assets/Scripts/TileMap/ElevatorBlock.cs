using UnityEngine;
using System.Collections;

public class ElevatorBlock : InteractiveTile {

	public Vector3 startPosition;
	public Vector3 startDirection;

	public Vector3 currentDirection;
	public float speed = 1.0f;

	public tk2dTileMap tilemap;

	private const int TILE_UP = 13;
	private const int TILE_DOWN = 15;
	private const int TILE_LEFT = 14;
	private const int TILE_RIGHT = 12;

	public void Start () {
		startPosition = transform.position;
		startDirection = Vector3.right; //
		currentDirection = startDirection;
		tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
	}

	public override void Reset ()
	{
		transform.position = startPosition;
		currentDirection = startDirection;
	}

	public void FixedUpdate () {
		//Check for turns:
		ChangeDirectionCheck();
		//Update position:
		transform.position += currentDirection*speed*Time.deltaTime;

		//GetComponent<BoxCollider2D>()
		//Vector3 velocity3 = currentDirection*speed*Time.deltaTime;
		//transform.position = Vector3.MoveTowards(transform.position, transform.position + velocity3, Time.deltaTime*velocity3.magnitude);
	}

	public void ChangeDirectionCheck () {
		//Get the block currently under me:
		int tileId = tilemap.GetTileIdAtPosition(transform.position, 0);
		if ( !isDirectionTile(tileId)) {
			return;
		}

		float xfrac;
		float yfrac;
		tilemap.GetTileFracAtPosition(transform.position, out xfrac, out yfrac);
		float xfracfrac = xfrac - Mathf.Floor(xfrac);
		float yfracfrac = yfrac - Mathf.Floor(yfrac);

		Vector3 newDirection = getDirectionOfTile(tileId);
		if ( newDirection.Equals ( currentDirection ) ) {
			return;
		}

		//Vector3 blockPosition = tilemap.GetTilePosition( Mathf.FloorToInt(xfrac), Mathf.FloorToInt(yfrac) );

		if ( currentDirection.x > 0 && xfracfrac >= 0.5f ) {
			currentDirection = newDirection;
			//Position in the middle of the new block: //TODO: this
			//transform.position = blockPosition;
			return;
		}
		if ( currentDirection.x < 0 && xfracfrac <= 0.5f ) {
			currentDirection = newDirection;
			//Position in the middle of the new block: //TODO: this
			//transform.position = blockPosition;
			return;
		}
		if ( currentDirection.y > 0 && yfracfrac >= 0.5f ) {
			currentDirection = newDirection;
			//Position in the middle of the new block: //TODO: this
			//transform.position = blockPosition;
			return;
		}
		if ( currentDirection.y < 0 && yfracfrac <= 0.5f ) {
			currentDirection = newDirection;
			//Position in the middle of the new block: //TODO: this
			//transform.position = blockPosition;
			return;
		}



	}

	public bool isDirectionTile ( int tileId ) {
		return ( tileId == TILE_UP || tileId == TILE_DOWN || tileId == TILE_LEFT || tileId == TILE_RIGHT );
	}
	public Vector3 getDirectionOfTile ( int tileId ) {
		switch ( tileId ) {
		case TILE_UP: return Vector3.up;
		case TILE_DOWN: return Vector3.down;
		case TILE_LEFT: return Vector3.left;
		case TILE_RIGHT: return Vector3.right;
		}
		return Vector3.zero;
	}

}
