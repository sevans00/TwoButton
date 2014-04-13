using UnityEngine;
using System.Collections;

public class UIMenu : MonoBehaviour {

	public delegate void UIMenuAction ();

	public UIMenuAction onShow;
	public UIMenuAction onHide;

	public void Update () {
		if ( Input.GetKeyDown(KeyCode.Escape) ) { //Back
			Hide ();
		}
	}

	public void Show () {
		gameObject.SetActive(false);
		if ( onShow != null ) {
			onShow();
		}
	}

	public void Hide () {
		gameObject.SetActive(false);
		if ( onHide != null ) {
			onHide();
		}
	}
}
