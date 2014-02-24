using UnityEngine;
using System.Collections;

//A one-way block
public class ToggleBlock : InteractiveTile {

	public bool On = true;
	private bool starting_On;

	private string spriteNamePrefix = "toggleblock_blue_";
	private tk2dSprite sprite;
	private BoxCollider2D boxcollider;

	public void Start () {
		sprite = GetComponent<tk2dSprite>();
		//Box collider setup:
		boxcollider = GetComponent<BoxCollider2D>();
		starting_On = On;
		On = !On;
		Toggle(); //Setup toggle ;)
	}
	
	public void Toggle () {
		On = !On;
		Color c = new Color(1f,1f,1f);
		if ( On ) {
			sprite.SetSprite( spriteNamePrefix + "on" );
			boxcollider.enabled = true;
			c.a = 1.0f;

		} else {
			sprite.SetSprite( spriteNamePrefix + "off" );
			boxcollider.enabled = false;
			c.a = 0.6f;
		}
		sprite.color = c;
	}

	override public void Reset () {
		On = !starting_On;
		Toggle();
		base.Reset();
	}


}
