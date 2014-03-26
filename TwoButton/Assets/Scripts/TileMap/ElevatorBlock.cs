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

	private GameObject[] adjacentElevatorBlocks;

	public void Start () {
		startPosition = transform.position;
		startDirection = Vector3.right; //
		currentDirection = startDirection;
		tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		//Link up with adjacent directions!  Yay!
		int x, y;
		tilemap.GetTileAtPosition(transform.position, out x, out y );
		//Check tiles to the left to find the initial direction to move:
		int tileId_L = tilemap.GetTile(x-1, y, 0);
		int tileId_R = tilemap.GetTile(x+1, y, 0);
		if ( isDirectionTile(tileId_L) ) {
			currentDirection = getDirectionOfTile(tileId_L);
		}
		if ( isDirectionTile(tileId_R) ) {
			currentDirection = getDirectionOfTile(tileId_R);
		}
		startDirection = currentDirection;
	}

	public override void Reset ()
	{
		Debug.Log("Reset elevator!");
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
		tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		//Link up with adjacent elevators!  Yay!
		int x, y;
		tilemap.GetTileAtPosition(transform.position, out x, out y );
		//Check tiles to the left to find the initial direction to move:
		int tileId_L = tilemap.GetTile(x-1, y, 0);
		int tileId_R = tilemap.GetTile(x+1, y, 0);
		if ( !isDirectionTile(tileId_L) && !isDirectionTile(tileId_R) ) {
			return;
		}

		Vector3 newDirection = Vector3.zero;
		if ( isDirectionTile(tileId_L) ) {
			newDirection = getDirectionOfTile(tileId_L);
		}
		if ( isDirectionTile(tileId_R) ) {
			newDirection = getDirectionOfTile(tileId_R);
		}

		float xfrac;
		float yfrac;
		tilemap.GetTileFracAtPosition(transform.position, out xfrac, out yfrac);
		float xfracfrac = xfrac - Mathf.Floor(xfrac);
		float yfracfrac = yfrac - Mathf.Floor(yfrac);

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
