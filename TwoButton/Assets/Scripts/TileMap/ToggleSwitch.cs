using UnityEngine;
using System.Collections;

//A one-way block
public class ToggleSwitch : InteractiveTile {


	public bool Pressed = false;
	private bool starting_Pressed;

	private string spriteNamePrefix = "switch_";
	private tk2dSprite sprite;
	private ToggleBlock[] blocks;

	public void Start () {
		starting_Pressed = Pressed;
		sprite = GetComponent<tk2dSprite>();
		blocks = GameObject.FindObjectsOfType<ToggleBlock>();
	}
	
	public void Toggle () {
		Pressed = !Pressed;
		if ( !Pressed ) {
			sprite.SetSprite( spriteNamePrefix + "on" );
		} else {
			sprite.SetSprite( spriteNamePrefix + "off" );
		}
		foreach ( ToggleBlock block in blocks ) {
			block.Toggle();
		}
	}

	void OnSpriteTriggerEnter () {
		Toggle();
	}

	
	override public void Reset () {
		Pressed = starting_Pressed;
		if ( !Pressed ) {
			sprite.SetSprite( spriteNamePrefix + "on" );
		} else {
			sprite.SetSprite( spriteNamePrefix + "off" );
		}
		base.Reset();
	}

}
