using UnityEngine;
using System.Collections;

//Classic controls jumper:
public class JumperC : Jumper2 {

	//Original non-stick Wall slide:
	override public void doWallSlide() {
		//Do wall slide and/or jump:
		if ( !onGround && Game.instance.left && spritePhysics.hitLeft ) { //
			sprite.scale = new Vector3( 1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			//Jump:
			if ( Game.instance.right && Game.instance.rightTime > Game.instance.leftTime && last_jump_time < Game.instance.rightTime ) {
				jumpDirection(Vector2.up + Vector2.right);
			}
		}
		if ( !onGround && Game.instance.right && spritePhysics.hitRight ) { //Game.instance.right && 
			sprite.scale = new Vector3( -1, sprite.scale.y, sprite.scale.z ); //Face away from wall
			//Slide up/down:
			if ( spritePhysics.hitLeftLayer != LayerMask.NameToLayer("Ice") ) {
				spritePhysics.velocity.y = Mathf.Max(spritePhysics.velocity.y, -WALL_SLIDE_SPEED);
			}
			//Jump:
			if ( Game.instance.left && Game.instance.leftTime > Game.instance.rightTime && last_jump_time < Game.instance.leftTime ) {
				jumpDirection(Vector2.up - Vector2.right);
			}
		}
	}







	//Kill this jumper
	void Kill () {
		Debug.Log("Kill recieved!");
		if ( isDead ) {
			return;
		}
		isDead = true;
		Debug.Log("Gameover1");
		//Gibs!
		if ( gibsPrefab != null ) {
			Instantiate(gibsPrefab, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
		Debug.Log("Gameover2");
		if ( Game.instance != null ) {
			Game.instance.GameOver();
		}
	}

}
