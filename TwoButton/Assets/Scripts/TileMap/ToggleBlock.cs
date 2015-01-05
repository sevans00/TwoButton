using UnityEngine;
using System.Collections;

//A one-way block
public class ToggleBlock : InteractiveTile {

	public bool On = true;
	protected bool starting_On;

	protected string spriteNamePrefix = "toggleblock_blue_";
	protected tk2dSprite sprite;
	protected BoxCollider2D boxcollider;

	public virtual void Start () {
		sprite = GetComponent<tk2dSprite>();
		//Box collider setup:
		boxcollider = GetComponent<BoxCollider2D>();
		starting_On = On;
		On = !On;
		Toggle(); //Setup toggle ;)
	}
	
	virtual public void Toggle () {
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
