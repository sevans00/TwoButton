using UnityEngine;
using System.Collections;
using MadLevelManager;

public class ComicCamera : MonoBehaviour {

	public void Skip () {
		MadLevelProfile.SetCompleted(MadLevel.currentLevelName, true);
		Game.instance.NextLevel();
	}


}
