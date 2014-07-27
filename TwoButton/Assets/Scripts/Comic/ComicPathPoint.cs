using UnityEngine;
using System.Collections;

public class ComicPathPoint : MonoBehaviour {

	public float cameraOrthographicSize = 1f;

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 1f);
		Gizmos.color = Color.yellow;
		float cameraSize = cameraOrthographicSize * 3.2f;
		float cameraHeight = cameraOrthographicSize * 5f / 2.5f;
		Vector3 cameraSquare = new Vector3( cameraSize, cameraHeight, 0f  );
		Gizmos.DrawWireCube(transform.position, cameraSquare);
	}

}
