using UnityEngine;
using System.Collections;

public class FaceMrTwoButton : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if ( Game.instance.jumper != null ) {
			if ( Game.instance.jumper.transform.position.x > this.transform.position.x ) {
				transform.localScale = new Vector3(-1f, 1f, 1f);
			} else {
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}
}
