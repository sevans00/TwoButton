using UnityEngine;
using System.Collections;

//Toggleable lava
public class LavaToggle : ToggleBlock {

	public override void Start ()
	{
		base.Start ();
	}

	override public void Toggle () {
		On = !On;
		if ( On ) {
			renderer.enabled = true;
			boxcollider.enabled = true;
		} else {
			renderer.enabled = false;
			boxcollider.enabled = false;
		}
	}
}
