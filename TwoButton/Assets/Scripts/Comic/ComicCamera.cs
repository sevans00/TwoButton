using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ComicCamera : MonoBehaviour {

	public float waitTime = 0.5f;

	public ComicPath path;
	public Camera comicCamera;

	ComicPathPoint [] pathPoints;
	int currentPathPointIndex;

	ComicPathPoint currentPathPoint;
	ComicPathPoint nextPathPoint;

	float cameraSpeed = 5f;

	// Use this for initialization
	void Start () {
		pathPoints = path.getPathPoints();
		if ( pathPoints.Length == 0 ) {
			Debug.LogWarning("Not enough path points");
		}
		transform.position = pathPoints[0].transform.position;
		comicCamera.orthographicSize = pathPoints[0].cameraOrthographicSize;
		currentPathPointIndex = 0;
		currentPathPoint = pathPoints[0];
		nextPathPoint = pathPoints[1];
	}

	// Update is called once per frame
	void Update () {
		if ( Vector3.Distance( transform.position, nextPathPoint.transform.position ) <= cameraSpeed*Time.deltaTime ) {
			if ( currentPathPointIndex >= pathPoints.Length-1 ) {
				transform.position = nextPathPoint.transform.position;
				return;
			}
			currentPathPoint = nextPathPoint;
			currentPathPointIndex++;
			nextPathPoint = pathPoints[currentPathPointIndex];
		}
		Vector3 newPosition;
		float totalDistance = Vector3.Distance(currentPathPoint.transform.position, nextPathPoint.transform.position);
		float currentDistance = Vector3.Distance(currentPathPoint.transform.position, transform.position);
		float frac = currentDistance / totalDistance;
		
		newPosition = Vector3.MoveTowards(transform.position, nextPathPoint.transform.position, cameraSpeed*Time.deltaTime);

		transform.position = newPosition;


	}


}
