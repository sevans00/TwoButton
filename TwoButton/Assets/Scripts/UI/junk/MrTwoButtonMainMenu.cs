using UnityEngine;
using System.Collections;

public class MrTwoButtonMainMenu : MonoBehaviour {

	public Transform pos1;
	public Transform pos2;
	public Transform posOffScreen;
	public Transform posOffScreen2;

	public Vector3 destination;

	public tk2dSpriteAnimator animator;
	public GameObject mrTwoButton;

	private bool animationFinished = false;

	private int currentThing = -1;

	public delegate void OnAnimationFinished();

	public OnAnimationFinished onAnimationFinished;

	void Start () {
		goUp();
	}

	void goUp() {
		mrTwoButton.transform.position = pos1.position;
		animator.Play("jump");
		animateTo(pos2.position, onPos2);
	}

	void onPos2 () {
		animator.Play("victory");
	}

	void pickRandom() {
		currentThing++;
		if ( currentThing > 4 ) {
			currentThing = 0;
		}
		switch ( currentThing ) {
		case 0:
			//Go down:
			animator.Play("jump");
			animateTo(pos1.position, goUp);
			break;
		case 1:
			//Go down:
			animator.Play("jump");
			animateTo(pos1.position, goUp);
			break;
		case 2:
			//Go offscreen1:
			animator.Play("run");
			animateTo(posOffScreen.position, goUp);
			break;
		case 3:
			//Go offscreen2:
			animator.Play("run");
			animateTo(posOffScreen2.position, goUp);
			break;
		}
	}
	











	void animateTo( Vector3 newDest, OnAnimationFinished newOnAnimationFinished ) {
		animationFinished = false;
		destination = newDest;
		onAnimationFinished = newOnAnimationFinished;
	}


	// Update is called once per frame
	void Update () {
		if ( !animationFinished ) {
			mrTwoButton.transform.position = Vector3.MoveTowards(mrTwoButton.transform.position, destination, Time.deltaTime);
			if ( Vector3.Distance(mrTwoButton.transform.position, destination) <= 0.1f ) {
				animationFinished = true;
				if ( onAnimationFinished != null ) {
					onAnimationFinished();
				}
			}
		}
	}

	void Click () {
		if ( animationFinished ) {
			pickRandom();
		}
	}



}
