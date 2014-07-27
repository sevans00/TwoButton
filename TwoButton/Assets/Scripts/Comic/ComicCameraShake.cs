using UnityEngine;
using System.Collections;

public class ComicCameraShake : MonoBehaviour {
	
	Vector3 originalPosition;
	Quaternion originalQuaternion;

	public float setShakeIntensity = 1f;
	public float setShakeDecay = 0.001f;

	float shakeIntensity;
	float shakeDecay;

	bool shakeStarted = false;



	//Initialize:
	public CameraPath cameraPath;
	public CameraPathAnimator cameraPathAnimator;
	public void Start () {
		//Link in Animator
		cameraPathAnimator.AnimationFinishedEvent += StartShake;
	}



	//HACK TO TEST:
	public bool doShake = false;
	public void Update () {
		if ( doShake ) {
			shakeStarted = false;
			StartShake();
			doShake = false;
		}
	}





	public void StartShake () {
		/*if ( shakeStarted ) {
			return;
		}
		shakeStarted = true;*/

		shakeIntensity = setShakeIntensity;
		shakeDecay = setShakeDecay;

		//Debug.LogWarning("Start Shake");
		originalPosition = transform.position;
		originalQuaternion = transform.rotation;
		StartCoroutine(DoShake());
	}

	public IEnumerator DoShake () {
		while ( shakeIntensity > 0f ) {
			transform.position = originalPosition + (Vector3)(Random.insideUnitCircle * shakeIntensity);
			transform.rotation = new Quaternion(originalQuaternion.x,// + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
			                                    originalQuaternion.y,// + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
			                                    originalQuaternion.z + Random.Range(-shakeIntensity, shakeIntensity)*.2f,
			                                    originalQuaternion.w + Random.Range(-shakeIntensity, shakeIntensity)*.2f);
			shakeIntensity -= shakeDecay;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(0.5f);
		//Shake is done, animate in button:
		Vector3 nextlevelTarget = new Vector3 (12.74888f, -19.5491f, 0f);
		float buttonSpeed = 10.0f;
		while ( Vector3.Distance( nextLevelButton.transform.position, nextlevelTarget ) > Time.deltaTime * buttonSpeed ) {
			nextLevelButton.transform.position = Vector3.MoveTowards( nextLevelButton.transform.position, nextlevelTarget, Time.deltaTime* buttonSpeed );
			yield return new WaitForEndOfFrame();
		}
	}

	public GameObject nextLevelButton;
	public void NextLevel () {
		Game.instance.NextLevel();
	}


}
