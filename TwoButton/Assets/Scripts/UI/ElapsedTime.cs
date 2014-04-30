using UnityEngine;
using System.Collections;

public class ElapsedTime : MonoBehaviour {

	public tk2dTextMesh textMesh;

	public void Update () {
		float timeElapsed = Time.time - Game.instance.spawnTime;
		timeElapsed = (Mathf.Ceil(timeElapsed*100)/100);
		textMesh.text = string.Format("{0:0.00}", timeElapsed);
		textMesh.Commit();
	}

}
