using UnityEngine;
using System.Collections;

public class FallingSpikes : InteractiveTile {

	public Vector3 startPosition;
	public Vector3 endPosition;

	public GameObject spikes;
	public BoxCollider2D triggerCollider;

	public bool triggered = false;

	//Trigger the spikes:
	public void TriggerSpikes () {
		if ( triggered ) {
			return;
		}
		triggered = true;
		totalAnimationTime = 0f;
	}
	public override void Reset ()
	{
		Debug.LogWarning("Reset");
		spikes.transform.position = startPosition;
		Debug.LogWarning("Reset pos: "+spikes.transform.position + ", startpos:"+startPosition);
		triggered = false;
		spikePhase = SpikePhase.SHAKING;
	}
	private enum SpikePhase {
		SHAKING,
		DROPPING,
		RESETTING
	};
	private SpikePhase spikePhase = SpikePhase.SHAKING;
	private float totalAnimationTime;
	private float TimeToShake = 0.7f;
	public override void DoFixedUpdate () //Takes into account paused game state
	{
		if ( triggered ) {
			switch (spikePhase) {
			case SpikePhase.SHAKING:
				totalAnimationTime += Time.deltaTime;
				if ( totalAnimationTime >= TimeToShake ) {
					spikes.transform.position = startPosition; //Back to start position before dropping
					Debug.LogWarning("spikespos: "+spikes.transform.position);
					spikePhase = SpikePhase.DROPPING;
					break;
				}
				if ( spikes.transform.localPosition.x < 0 ) {
					spikes.transform.position = startPosition + new Vector3(0.1f, 0f, 0f); 
				} else {
					spikes.transform.position = startPosition - new Vector3(0.1f, 0f, 0f);
				}
				break;
			case SpikePhase.DROPPING:
				Debug.LogWarning("spikespos: "+spikes.transform.position.y+ " <= "+endPosition.y);
				if ( spikes.transform.position.y <= endPosition.y ) {
					totalAnimationTime = 0f;
					spikePhase = SpikePhase.RESETTING;
				}
				spikes.transform.position += Vector3.down * 0.4f;
				break;
			case SpikePhase.RESETTING:
				totalAnimationTime += Time.deltaTime;
				if ( totalAnimationTime < 1.0f ) {
					//Just wait
					break;
				}
				if ( totalAnimationTime < 2.0f ) {
					//Reset the spike back into it's place
					spikes.transform.position = Vector3.Lerp ( startPosition+Vector3.up*1.28f, startPosition, totalAnimationTime-1f);
					break;
				}
				Reset();
				break;
			}
		}
	}





	public void Start () {
		calculateTrigger();
	}

	public void calculateTrigger () {
		Debug.LogWarning("FallingSpikes - Calculating trigger size.");
		startPosition = spikes.transform.position;
		startPosition.z = 1f;
		//Find the end position:
		tk2dTileMap tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		int endY, x, y;
		tilemap.GetTileAtPosition( startPosition, out x, out y );
		endY = y - 1;
		while ( endY != tilemap.height-1 && tilemap.GetTile(x, endY, 0 ) == -1 ) {
			endY--;
		}
		endY++;
		endPosition = tilemap.GetTilePosition( x, endY ) + new Vector3(0.64f, 0, 0);
		Gizmos.DrawWireSphere(startPosition, 0.64f);
		Gizmos.DrawWireSphere(endPosition, 0.64f);
		//Now we've got endY, so modify the trigger:
		Vector3 middlePosition = Vector3.Lerp(startPosition, endPosition, 0.5f);
		triggerCollider.transform.position = middlePosition;
		Vector2 triggerSize = triggerCollider.size;
		triggerSize.x = 3.84f;
		triggerSize.y = Mathf.Abs(endPosition.y - startPosition.y);
		triggerCollider.size = triggerSize;
	}


	void OnSpriteTriggerEnter () {
		TriggerSpikes();
	}
	void OnSpriteTriggerStay () {
		TriggerSpikes();
	}


}
