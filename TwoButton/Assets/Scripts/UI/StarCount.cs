using UnityEngine;
using System.Collections;
using MadLevelManager;

public class StarCount : MonoBehaviour {

	public tk2dSprite star1;
	public tk2dSprite star2;
	public tk2dSprite star3;

	public StarBlock starBlock1;
	public StarBlock starBlock2;
	public StarBlock starBlock3;

	// Use this for initialization
	public void Reset () {
		//Find Stars:
		StarBlock [] starBlocks = GameObject.FindObjectsOfType<StarBlock>();
		if ( starBlocks.Length >= 3 ) {
			starBlock1 = starBlocks[0];
			starBlock2 = starBlocks[1];
			starBlock3 = starBlocks[2];
		}
	}
	
	// Update is called once per frame
	public void Update () {
		UpdateStar( star1, starBlock1 );
		UpdateStar( star2, starBlock2 );
		UpdateStar( star3, starBlock3 );
	}

	public void UpdateStar ( tk2dSprite star, StarBlock starBlock ) {
		if ( star != null && starBlock != null ) {
			star.color = Color.white;
			if ( starBlock.got ) {
				star.SetSprite("star");
			} else {
				if ( MadLevelProfile.GetLevelBoolean(MadLevel.currentLevelName, "star_"+starBlock.starId) ) {
					star.SetSprite("star");
					star.color = Color.cyan;
				} else {
					star.SetSprite("star_empty");
				}
			}


		}
	}
}
