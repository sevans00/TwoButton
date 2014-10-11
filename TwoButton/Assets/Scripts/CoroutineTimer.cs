using UnityEngine;
using System.Collections;

[System.Serializable]
public class CoroutineTimer
{
	//public fields
	public float TimerDuration, TimerRandomScaleFactor, TimerStartDelay;
	public bool Repeats;
	
	//private fields
	bool _running;
	bool startDelayComplete;
	CoroutineTimerBehaviour timerBehaviour;
	System.Action timerFinishedAction;
	
	public CoroutineTimer(float timerDuration, float timerRandomScaleFactor, 
	                      float timerStartDelay, bool repeats)
	{
		this.TimerDuration = timerDuration;
		this.TimerRandomScaleFactor = timerRandomScaleFactor;
		this.TimerStartDelay = timerStartDelay;
		this.Repeats = repeats;
	}
	
	public CoroutineTimer(float timerDuration) : this(timerDuration, 0, 0, false)
	{
		
	}
	
	public void Start(GameObject targetGameObject, System.Action timerFinishedAction)
	{	
		if(timerBehaviour != null)
		{
			throw new System.InvalidOperationException(
				"This timer has already been started");
		}
		
		timerBehaviour = targetGameObject.AddComponent<CoroutineTimerBehaviour>();
		this.timerFinishedAction = timerFinishedAction;
		
		//if we have a start delay, run a timer for that before starting the real timer
		if(TimerStartDelay != 0.0f && !startDelayComplete)
		{
			timerBehaviour.StartTimer(TimerStartDelay, startTimerFinished);
		}
		else
		{
			doStart();
		}
	}
	
	void doStart()
	{
		if(TimerDuration == 0.0f)
		{
			throw new System.InvalidOperationException("Timer duration cannot be zero");
		}
		if(TimerRandomScaleFactor < 0.0f || TimerRandomScaleFactor > 1.0f)
		{
			throw new System.ArgumentException(
				"Timer scale factor must be between 0 and 1");
		}
		
		float waitSeconds = Random.Range(TimerDuration * (1 - TimerRandomScaleFactor), 
		                                 TimerDuration * (1 + TimerRandomScaleFactor));
		timerBehaviour.StartTimer(waitSeconds, timerFinished);
		_running = true;
	}
	
	public void Stop()
	{
		_running = false;
		
		//stop coroutines, remove timer component and clean up references
		if(timerBehaviour != null)
		{
			timerBehaviour.StopCoroutine(CoroutineTimerBehaviour.TimerCoroutineName);
			GameObject.Destroy(timerBehaviour);
		}
		timerBehaviour = null;
		timerFinishedAction = null;
	}
	
	void startTimerFinished()
	{
		startDelayComplete = true;
		doStart();
	}
	
	void timerFinished()
	{	
		_running = false;
		if(timerFinishedAction != null)
		{
			timerFinishedAction();
		}
		
		//null check to make sure the timer has not been stopped in the timerFinishedDelegate
		if(Repeats && timerBehaviour != null)
		{
			doStart();
		}
		else
		{
			Stop();
		}
	}
	
	#region Property methods
	
	public bool Running
	{
		get
		{
			return _running;
		}
	}
	
	#endregion
}

class CoroutineTimerBehaviour : MonoBehaviour
{
	public static readonly string TimerCoroutineName = "startTimer";
	
	System.Action timerFinishedAction;
	
	public void StartTimer(float waitSeconds, System.Action timerFinishedAction)
	{
		this.timerFinishedAction = timerFinishedAction;
		StartCoroutine(TimerCoroutineName, waitSeconds);
	}
	
	IEnumerator startTimer(float waitSeconds)
	{
		yield return new WaitForSeconds(waitSeconds);
		timerFinishedAction();
	}
}