using UnityEngine;
using System.Collections;
using MadLevelManager;

public class PreviewCamera : MonoBehaviour {

	public static PreviewCamera instance;

	public Vector3 startPosition;
	public float startOrthographicSize;
	public Camera camera;
	public Camera mainCamera;

	public bool showingLevelInitially = true;

	public bool zoomedOut = true;

	public float zoomTime = 0.4f;

	public void Awake () {
		PreviewCamera.instance = this;
		//startPosition = transform.position;
		//startOrthographicSize = camera.orthographicSize;
	}

	//Reset based on the level:
	public void DoLevelWasLoaded() {
		Vector2 bottomLeft = Vector2.one;
		Vector2 topRight = Vector2.one;
		//Are there camera edges?
		CameraBoundaryBlock [] boundaryBlocks = GameObject.FindObjectsOfType<CameraBoundaryBlock>();
		if ( boundaryBlocks.Length == 2 ) {
			//We've got a boundary:
			if ( boundaryBlocks[0].transform.position.x < boundaryBlocks[1].transform.position.x ) {
				bottomLeft = (Vector2)boundaryBlocks[0].transform.position;
				topRight = (Vector2)boundaryBlocks[1].transform.position;
			} else {
				bottomLeft = (Vector2)boundaryBlocks[1].transform.position;
				topRight = (Vector2)boundaryBlocks[0].transform.position;
			}
		} else {
			//We've got to do this manually:
			tk2dTileMap tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
			int xMin = tilemap.width-1, xMax = 0, yMin = tilemap.height-1, yMax = 0;
			for ( int ii = 0; ii < tilemap.width; ii++ ) {
				for ( int jj = 0; jj < tilemap.height; jj++ ) {
					if ( tilemap.GetTile(ii, jj, 0) != -1 ) {
						if ( ii < xMin ) { xMin = ii; }
						if ( ii > xMax ) { xMax = ii; }
						if ( jj < yMin ) { yMin = jj; }
						if ( jj > yMax ) { yMax = jj; }
					}
				}
			}
			bottomLeft = tilemap.GetTilePosition(xMin, yMin);
			topRight = tilemap.GetTilePosition(xMax, yMax);
		}
		//Adjust:
		bottomLeft += Vector2.one * 0.64f;
		topRight += Vector2.one * 0.64f;
		//Midpoint:
		Vector3 midpoint = (Vector3)Vector2.Lerp(bottomLeft, topRight, 0.5f);
		midpoint.z = -10f;
		transform.position = midpoint;
		//Orthographic size:
		float orthosize = (topRight.y - bottomLeft.y)/2;
		Vector2 dimensions = new Vector2((topRight.x - bottomLeft.x),(topRight.y - bottomLeft.y));
		Vector2 cameraDimensions = new Vector2 ( camera.aspect*dimensions.y, dimensions.y );
		Vector2 cameraDimensionsWidth = new Vector2 ( dimensions.x, dimensions.x/camera.aspect );
//		Debug.LogWarning("Dimensions:"+cameraDimensions);
//		Debug.LogWarning("Dimensions:"+cameraDimensionsWidth);
		//Set the orthosize based on the largest dimension:
		if ( cameraDimensions.y > cameraDimensionsWidth.y ) {
			orthosize = cameraDimensions.y / 2;
		} else {
			orthosize = cameraDimensionsWidth.y / 2;
		}
		camera.orthographicSize = orthosize;

		//Set starting variables:
		startPosition = transform.position;
		startOrthographicSize = camera.orthographicSize;

		//showingLevelInitially = true;
		zoomedOut = true;
	}

	/*
	public void Update () {
		if ( !showingLevelInitially ) {
			return;
		}
		//Game.instance.pause();
		if ( ( Input.touches.Length > 0 && Input.GetTouch(0).phase == TouchPhase.Began ) 
		    || (//Input.GetMouseButtonDown(0) || 
		    Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) ) ) {

			showingLevelInitially = false;
			zoomIn();
		}
	}
	*/

	//Zoom in:
	public void zoomIn() {
		zoomedOut = false;
		iTween.StopByName("zoom_out");
		iTween.StopByName("zoom_out_ortho");
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "zoom_in",
			"position", mainCamera.transform.position,
			"time", zoomTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
		iTween.ValueTo( gameObject, iTween.Hash(
			"name", "zoom_in_ortho",
			"from", camera.orthographicSize, 
			"to", mainCamera.orthographicSize, 
			"onupdatetarget", gameObject, 
			"onupdate", "doCameraZoom", 
			"oncomplete", "zoomInComplete", 
			"time", zoomTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void zoomInComplete() {
		camera.enabled = false;
		mainCamera.enabled = true;
		Game.instance.unpause();
	}

	//Zoom out:
	public void zoomOut() {
		if ( zoomedOut ) {
			return;
		}
		iTween.StopByName("zoom_in");
		iTween.StopByName("zoom_in_ortho");
		camera.enabled = true;
		mainCamera.enabled = false;
		transform.position = mainCamera.transform.position;
		//camera.orthographicSize = mainCamera.orthographicSize;
		Game.instance.pause();
		iTween.MoveTo( gameObject, iTween.Hash(
			"name", "zoom_out",
		    "position", startPosition,
			"time", zoomTime,
			"easetype", iTween.EaseType.easeOutQuad ) );
		iTween.ValueTo( gameObject, iTween.Hash(
			"name", "zoom_out_ortho",
			"from", camera.orthographicSize, 
			"to", startOrthographicSize, 
			"onupdatetarget", gameObject, 
			"onupdate", "doCameraZoom", 
			"oncomplete", "zoomOutComplete", 
			"time", zoomTime, 
			"easetype", iTween.EaseType.easeOutQuad ) );
	}
	public void zoomOutComplete() {
		zoomedOut = true;
	}


	
	



	//Do the orthographic zoom:
	public void doCameraZoom ( float newOrthoSize ) {
		camera.orthographicSize = newOrthoSize;
	}

}
