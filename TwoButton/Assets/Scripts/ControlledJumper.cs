using UnityEngine;
using System.Collections;

//Control the height of the jump jumper:
public class ControlledJumper : Jumper2 {

	protected override void FixedUpdate ()
	{
		if ( !onGround && spritePhysics.velocity.y > 0 && (!Game.instance.left && !Game.instance.right) ) {
			spritePhysics.velocity.y = 0;
		}
		base.FixedUpdate ();
	}

}
