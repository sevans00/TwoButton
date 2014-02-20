using UnityEngine;
using System.Collections;

public class Puff : Jumper2 {

	override public void jump() {
		jumpCount++;
		spritePhysics.velocity = new Vector2(spritePhysics.velocity.x, JUMP_SPEED * (1.0f - (jumpCount)/(MaxJumps+1) ) );
		last_jump_time = Time.time;
	}
}
