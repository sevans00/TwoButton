using UnityEngine;
using System.Collections;

public class FinishBlock : MonoBehaviour {

	void OnSpriteTriggerEnter () {
		Game.instance.EndLevel();
	}
}
