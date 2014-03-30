using UnityEngine;
using System.Collections;
using MadLevelManager;

public class StarBlock : InteractiveTile {

	tk2dSprite sprite;
	int starId;

	void Start() {
		sprite = GetComponent<tk2dSprite>();
		StarBlock[] starblocks = GameObject.FindObjectsOfType<StarBlock>();
		for ( int ii = 0; ii < starblocks.Length; ii++ ) {
			if ( starblocks[ii] == this ) {
				starId = ii+1;
			}
		}
		Reset();
	}

	void OnSpriteTriggerEnter () {
		//Record that I got it!
		sprite.renderer.enabled = false;
		MadLevelProfile.SetLevelBoolean(MadLevel.currentLevelName, "star_"+starId, true);
	}

	override public void Reset() {
		//Tint me if i've been gotten
		if ( MadLevelProfile.GetLevelBoolean(MadLevel.currentLevelName, "star_"+starId) ) {
			sprite.color = Color.cyan;
		}
		//For now just make it visible:
		sprite.renderer.enabled = true;
	}
}
