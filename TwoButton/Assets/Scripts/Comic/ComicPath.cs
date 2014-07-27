using UnityEngine;
using System.Collections;

public class ComicPath : MonoBehaviour {

	void OnDrawGizmos() {
		if ( transform.childCount < 2 ) {
			return;
		}
		Gizmos.color = Color.green;
		Vector3 lastPosition = transform.GetChild(0).position;
		for ( int ii = 1; ii < transform.childCount; ii++) {
			Gizmos.DrawLine( lastPosition, transform.GetChild(ii).position );
			lastPosition = transform.GetChild(ii).position;
		}
	}

	public ComicPathPoint[] getPathPoints () {
		return GetComponentsInChildren<ComicPathPoint>();
	}

}
