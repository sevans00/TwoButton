using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ParallaxBGSection : MonoBehaviour {

	private ParallaxBGData data;
	public ParallaxBGData[] parallaxBGData;

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

		Bounds bounds = bgSprite.GetBounds();
		Vector3 spriteTopRight = bgSprite.transform.position + bounds.extents;
		Vector3 spriteBottomLeft = bgSprite.transform.position - bounds.extents;

		Vector3 cameraBottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
		Vector3 cameraTopRight = Camera.main.ViewportToWorldPoint(Vector3.one);

		if ( data == null ) {
			return;
		}
		//Set the camera render texture:
		if ( cameraTopRight.y > spriteTopRight.y ) {
			Camera.main.backgroundColor = data.topColor;
		}
		if ( cameraBottomLeft.y < spriteBottomLeft.y ) {
			Camera.main.backgroundColor = data.bottomColor;
		}

		//Check horizontal: //You know what?  Don't.  Because the tilemaps are only big enough to fit three, so let's just make it three.  Srsly.
	}


	//Reset the parallax:
	public void reset () {
		//See if we can find a BG_StartPoint in the level:
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

		//Do Data:
		data = null;
		for ( int ii = 0; ii < parallaxBGData.Length; ii++ ) {
			if ( MadLevel.currentGroupName == parallaxBGData[ii].MadGroupName ) {
				data = parallaxBGData[ii];
				break;
			}
		}
		if ( data == null ) {
			data = new ParallaxBGData();
		}

//		bgSprite.SetSprite(data.spriteName);

		/*
		return;
		//Choose parallax background:
		//TODO: This
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
		}*/
	}

}
