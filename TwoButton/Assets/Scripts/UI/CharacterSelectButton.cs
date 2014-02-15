using UnityEngine;
using System.Collections;

public class CharacterSelectButton : MonoBehaviour {

	public GameObject prefab;

	public void Select() {
		CharacterSelectScreen.instance.SelectNewCharacter(prefab);
	}

}
