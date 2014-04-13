using UnityEngine;
using System.Collections;

public class UICharacterSelectButton : MonoBehaviour {

	public GameObject prefab;

	public void Select() {
		UICharacterSelectScreen.instance.SelectNewCharacter(prefab);
	}

}
