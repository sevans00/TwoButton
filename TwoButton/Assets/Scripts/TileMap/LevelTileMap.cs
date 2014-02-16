using UnityEngine;
using System.Collections;

public class LevelTileMap : MonoBehaviour {

	private InteractiveTile [] tiles;

	//Initialization:
	public void Start () {
		tiles = GameObject.FindObjectsOfType<InteractiveTile>();
	}

	//Reset the level by resetting all objects inside it
	public void ResetLevel () {
		foreach ( InteractiveTile tile in tiles ) {
			tile.Reset();
		}
	}
}
