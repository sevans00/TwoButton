using UnityEngine;
using System.Collections;

public class RisingLava : InteractiveTile {

	public Vector3 startPosition;
	public Vector3 endPosition;

	public float speed = 0.1f;

	public void Awake () {
		startPosition = transform.position;
	}

	public override void Reset ()
	{
		transform.position = startPosition;
	}

	public override void DoFixedUpdate () //Takes into account paused game state
	{
		transform.position += Vector3.up * speed;
	}

}
