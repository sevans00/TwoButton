using UnityEngine;
using System.Collections;

public class MTB_Death : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("DeathAnim");
	}

	IEnumerator DeathAnim() {
		//tk2dSprite sprite = GetComponent<tk2dSprite>();
		Game.instance.gameOver = true; //Pause the clock
		yield return new WaitForSeconds(0.1f);
		//Move up:
		float gravity = -0.75f; //-0.75f
		Vector3 upPos = transform.position + Vector3.up * 5f;
		float speed = 15f;
		//while ( transform.position.y < upPos.y ) {
		while ( speed > 0f ) {
			transform.position = Vector3.MoveTowards(transform.position, upPos, Time.deltaTime * Mathf.Abs(speed));
			speed += gravity;
			yield return new WaitForFixedUpdate(); //WaitForEndOfFrame
		}
		//yield return new WaitForFixedUpdate();
		//speed = gravity;
		Vector3 bottomOfFrame = new Vector3(transform.position.x, Camera.main.ViewportToWorldPoint(Vector3.zero).y-0.64f, transform.position.z);
		while ( transform.position.y > bottomOfFrame.y ) {
			transform.position = Vector3.MoveTowards(transform.position, bottomOfFrame, Time.deltaTime * Mathf.Abs(speed));
			speed += gravity;
			yield return new WaitForFixedUpdate(); //WaitForEndOfFrame
		}

		Destroy(gameObject);
		Game.instance.GameOver();
	}

}
