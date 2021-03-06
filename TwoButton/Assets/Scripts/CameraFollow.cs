﻿using UnityEngine;
using System.Collections;
using MadLevelManager;

public class CameraFollow : MonoBehaviour {

	public static CameraFollow instance;

	public Transform target;

	//Backgrounds:
	public ParallaxBGSection parallaxBGSection;
	public bool _prevPosAssigned;
	public Vector3 previousPosition;
	public Vector3 deltaMovement;
	//World backgrounds:
	public ParallaxBGSection world1_parallaxBGSection;
	public ParallaxBGSection world2_parallaxBGSection;
	public ParallaxBGSection world3_parallaxBGSection;

	//Boundary
	public int boundaryPoints = 0;
	public Vector2 boundaryPoint1;
	public Vector2 boundaryPoint2;

	//Level boundary (allowing out-of-frame hidden areas)
	public int levelBoundaryPoints = 0;
	public Vector2 levelBoundaryPoint1;
	public Vector2 levelBoundaryPoint2;

	//Pre Initialization
	void Awake () {
		CameraFollow.instance = this;
	}

	// Use this for initialization
	void Start () {
		OnLevelWasLoaded();
	}
	
	void OnLevelWasLoaded () {
		//Reset camera boundaries:
		CameraBoundaryBlock[] boundaryBlocks = GameObject.FindObjectsOfType<CameraBoundaryBlock>();
		boundaryPoints = boundaryBlocks.Length;
		if ( boundaryPoints == 0 ) {
			//Do nothing
		} else if ( boundaryPoints == 1 ) { //Boundary point 1 is bottom left - always
			boundaryBlocks[0].Setup();
			boundaryPoint1 = (Vector2) boundaryBlocks[0].transform.position;
		} else {
			boundaryBlocks[0].Setup();
			boundaryBlocks[1].Setup();
			//Bound in a rectangle: (lower left to upper right)
			Vector2 bound1 = (Vector2) boundaryBlocks[0].transform.position;
			Vector2 bound2 = (Vector2) boundaryBlocks[1].transform.position;
			float tempXY;
			if ( bound1.x > bound2.x ) {
				tempXY = bound2.x;
				bound2.x = bound1.x;
				bound1.x = tempXY;
			}
			if ( bound1.y > bound2.y ) {
				tempXY = bound2.y;
				bound2.y = bound1.y;
				bound1.y = tempXY;
			}
			boundaryPoint1 = bound1;
			boundaryPoint2 = bound2;
		}
		//Reset the level boundaries (these should be BIGGER than the camera boundaries)
		LevelBoundaryBlock[] levelBoundaryBlocks = GameObject.FindObjectsOfType<LevelBoundaryBlock>();
		levelBoundaryPoints = levelBoundaryBlocks.Length;
		if ( levelBoundaryPoints == 0 ) {
			//Do nothing
		} else if ( levelBoundaryPoints == 1 ) { //Boundary point 1 is bottom left - always
			levelBoundaryBlocks[0].Setup();
			levelBoundaryPoint1 = (Vector2) levelBoundaryBlocks[0].transform.position;
			boundaryPoint1 = levelBoundaryPoint1; //FOR NOW
		} else {
			levelBoundaryBlocks[0].Setup();
			levelBoundaryBlocks[1].Setup();
			//Bound in a rectangle: (lower left to upper right)
			Vector2 lbound1 = (Vector2) levelBoundaryBlocks[0].transform.position;
			Vector2 lbound2 = (Vector2) levelBoundaryBlocks[1].transform.position;
			float ltempXY;
			if ( lbound1.x > lbound2.x ) {
				ltempXY = lbound2.x;
				lbound2.x = lbound1.x;
				lbound1.x = ltempXY;
			}
			if ( lbound1.y > lbound2.y ) {
				ltempXY = lbound2.y;
				lbound2.y = lbound1.y;
				lbound1.y = ltempXY;
			}
			levelBoundaryPoint1 = lbound1;
			levelBoundaryPoint2 = lbound2;
			boundaryPoint1 = levelBoundaryPoint1; //FOR NOW
			boundaryPoint2 = levelBoundaryPoint2; //FOR NOW
		}

		//Initialize parallax bg:
		world1_parallaxBGSection.gameObject.SetActive(false);
		world2_parallaxBGSection.gameObject.SetActive(false);
		world3_parallaxBGSection.gameObject.SetActive(false);
		if ( MadLevel.currentGroupName == null ) {
			world1_parallaxBGSection.gameObject.SetActive(true);
			parallaxBGSection = world1_parallaxBGSection;
		} else {
			if ( MadLevel.currentGroupName == "World2" ) {
				world2_parallaxBGSection.gameObject.SetActive(true);
				parallaxBGSection = world2_parallaxBGSection;
			} else {
				if ( MadLevel.currentGroupName == "World3" ) {
					world3_parallaxBGSection.gameObject.SetActive(true);
					parallaxBGSection = world3_parallaxBGSection;
				} else {
					world1_parallaxBGSection.gameObject.SetActive(true);
					parallaxBGSection = world1_parallaxBGSection;
				}
			}
		}
		//For test sake, let's make world 3 always on:
//		world1_parallaxBGSection.gameObject.SetActive(false);
//		world2_parallaxBGSection.gameObject.SetActive(false);
//		world3_parallaxBGSection.gameObject.SetActive(true);
//		parallaxBGSection = world3_parallaxBGSection;
		//Reset parallax bg:
		_prevPosAssigned = false;
		parallaxBGSection.reset();
	}
	
	// Update is called once per frame
	void Update () {
		if ( target != null ) {
			if ( !_prevPosAssigned ) {
				_prevPosAssigned = true;
				previousPosition = (Vector2)target.position;
			}

			//Update delta:
			deltaMovement = (Vector2)this.transform.position - (Vector2)target.position;

			//Update position:
			//this.transform.position = target.position + Vector3.back*10;
			//Boundary check:
			if ( boundaryPoints == 0 ) {
				this.transform.position = target.position + Vector3.back*10;
			} else if ( boundaryPoints == 1 ) {
				//By default, position 1 is lower left:
				this.transform.position = target.position + Vector3.back*10;
				Vector3 cameraZP = camera.ViewportToWorldPoint(Vector3.zero);
				Vector3 boundaryDiff = Vector3.zero;
				if ( cameraZP.x < boundaryPoint1.x ) {
					boundaryDiff.x = boundaryPoint1.x - cameraZP.x;
				}
				if ( cameraZP.y < boundaryPoint1.y ) {
					boundaryDiff.y = boundaryPoint1.y - cameraZP.y;
				}
				this.transform.position += boundaryDiff;
			} else {
				//Boundary 1 - bottom left
				this.transform.position = target.position + Vector3.back*10;
				Vector3 cameraZP = camera.ViewportToWorldPoint(Vector3.zero);
				Vector3 boundaryDiff = Vector3.zero;
				if ( cameraZP.x < boundaryPoint1.x ) {
					boundaryDiff.x = boundaryPoint1.x - cameraZP.x;
				}
				if ( cameraZP.y < boundaryPoint1.y ) {
					boundaryDiff.y = boundaryPoint1.y - cameraZP.y;
				}
				//Boundary 2 - upper right
				Vector3 cameraOP = camera.ViewportToWorldPoint(Vector3.one);
				if ( cameraOP.x > boundaryPoint2.x ) {
					boundaryDiff.x = boundaryPoint2.x - cameraOP.x;
				}
				if ( cameraOP.y > boundaryPoint2.y ) {
					boundaryDiff.y = boundaryPoint2.y - cameraOP.y;
				}
				this.transform.position += boundaryDiff;
			}



			//Parallax thingy:
			//Update delta:
			deltaMovement = (Vector2)previousPosition - (Vector2)transform.position;
			if ( parallaxBGSection != null ) {
				parallaxBGSection.setPosition(deltaMovement);
			}
		}
		previousPosition = (Vector2)transform.position;
	}
}
