using UnityEngine;
using System.Collections;
using MadLevelManager;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	//Backgrounds:
	public GameObject parallaxBG = null;
	public float parallaxDelta = 0.1f;
	public Vector2 deltaMovement;
	public Vector2 previousPosition;
	public bool _prevPosAssigned = false;
	public GameObject parallaxBG_near;
	public float parallaxDelta_near = 0.3f;
	public GameObject parallaxBG_med;
	public float parallaxDelta_med = 0.2f;
	public GameObject parallaxBG_far;
	public float parallaxDelta_far = 0.1f;

	//Boundary
	public int boundaryPoints = 0;
	public Vector2 boundaryPoint1;
	public Vector2 boundaryPoint2;

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

		//Reset parallax bg:
		_prevPosAssigned = false;
		parallaxBG.transform.localPosition = new Vector3 (0,0,20);

		//Choose parallax background:
		//TODO: This
		tk2dSprite bgSprite = parallaxBG.GetComponent<tk2dSprite>();
		switch ( MadLevel.currentGroupName ) {
		case "(default)":
			bgSprite.SetSprite("BG_Forest");
			break;
		case "World1":
			bgSprite.SetSprite("BG_Forest");
			break;
		case "World2":
			bgSprite.SetSprite("BG_Mountains");
			break;
		case "World3":
			bgSprite.SetSprite("BG_LavaMountain");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ( target != null ) {
			if ( !_prevPosAssigned ) {
				_prevPosAssigned = true;
				previousPosition = (Vector2)target.position;
			}

			//Update delta:
			//deltaMovement = (Vector2)this.transform.position - (Vector2)target.position;

			//Update position:
			//this.transform.position = target.position + Vector3.back*10;
			//Boundary check:
			if ( boundaryPoints == 0 ) {
				this.transform.position = target.position + Vector3.back*10;
			} else if ( boundaryPoints == 1 ) {
				//By default, position 1 is lower left:
				this.transform.position = target.position + Vector3.back*10;
				Vector3 cameraZP = Camera.main.ViewportToWorldPoint(Vector3.zero);
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
				Vector3 cameraZP = Camera.main.ViewportToWorldPoint(Vector3.zero);
				Vector3 boundaryDiff = Vector3.zero;
				if ( cameraZP.x < boundaryPoint1.x ) {
					boundaryDiff.x = boundaryPoint1.x - cameraZP.x;
				}
				if ( cameraZP.y < boundaryPoint1.y ) {
					boundaryDiff.y = boundaryPoint1.y - cameraZP.y;
				}
				//Boundary 2 - upper right
				Vector3 cameraOP = Camera.main.ViewportToWorldPoint(Vector3.one);
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
			if ( parallaxBG != null ) {
				parallaxBG.transform.position += (Vector3)deltaMovement*parallaxDelta;
				//TODO: Switch to texture offsets
				//k2dSprite parallaxBG_far_tk2dSprite = parallaxBG_far.GetComponent<tk2dSprite>();
				//parallaxBG_far.renderer.material.GetTexture("_MainTex").wrapMode = TextureWrapMode.Repeat;
				//parallaxBG_far.renderer.material.SetTextureScale( "_MainTex", Vector2.one);
				parallaxBG_far.renderer.material.SetTextureOffset( "_MainTex", parallaxDelta_far * deltaMovement + parallaxBG_far.renderer.material.GetTextureOffset("_MainTex") );

				//parallaxBG.transform.position += (Vector3)deltaMovement*parallaxDelta;

//				parallaxBG_med.renderer.material.SetTextureOffset( "_MainTex", deltaMovement * parallaxDelta_med);
//				parallaxBG_near.renderer.material.SetTextureOffset( "_MainTex", deltaMovement * parallaxDelta_near);

			}
		}
		previousPosition = (Vector2)transform.position;
	}
}
