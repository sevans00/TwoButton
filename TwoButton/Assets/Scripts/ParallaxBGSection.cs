using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ParallaxBGSection : MonoBehaviour {

	public float bgParallaxDelta = 0.1f;
	public float mgParallaxDelta = 0.2f;
	public float fgParallaxDelta = 0.3f;

	public tk2dTiledSprite bgSprite;
	private Vector3 bgStartPosition;
	public tk2dTiledSprite mgSprite;
	private Vector3 mgStartPosition;
	public tk2dTiledSprite fgSprite;
	private Vector3 fgStartPosition;

	private Vector3 startPosition;

	//Preinit init
	public void Awake () {
		startPosition = new Vector3(0, 20, 40);
		bgStartPosition = bgSprite.transform.localPosition;
		mgStartPosition = mgSprite.transform.localPosition;
		fgStartPosition = fgSprite.transform.localPosition;
	}



	public void setPosition ( Vector2 deltaMovement ) {
//		Debug.LogWarning ( " Delta movement! "+deltaMovement );
		bgSprite.transform.localPosition += (Vector3)(deltaMovement*bgParallaxDelta);
		mgSprite.transform.localPosition += (Vector3)(deltaMovement*mgParallaxDelta);
		fgSprite.transform.localPosition += (Vector3)(deltaMovement*fgParallaxDelta);
	}


	//Reset the parallax:
	public void reset () {
		//See if we can find a BG_StartPoint in the level: (DOESN'T WORK YET)
		BG_StartPoint bg_StartPoint = GameObject.FindObjectOfType<BG_StartPoint>();
		if ( bg_StartPoint == null ) {
			transform.localPosition = startPosition;
		} else {
			transform.position = new Vector3 (bg_StartPoint.transform.position.x, bg_StartPoint.transform.position.y, transform.position.y);
		}

		//Reset parallax bg positions:
		bgSprite.transform.localPosition = bgStartPosition;
		mgSprite.transform.localPosition = mgStartPosition;
		fgSprite.transform.localPosition = fgStartPosition;
	}

}
