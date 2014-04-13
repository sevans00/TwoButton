using UnityEngine;
using System.Collections;

public class UIMenuBtnCallbacker : MonoBehaviour {

	public enum Callback_Type {
		SWAP_MENU,
		CLOSE_MENU
	}

	public UIMenu targetMenu;
	public UIMenu swapTarget;
	public Callback_Type type;

	public void Callback() {
		switch ( type ) {
		case Callback_Type.CLOSE_MENU:
			targetMenu.Hide();
			break;
		case Callback_Type.SWAP_MENU:
			targetMenu.Hide();
			swapTarget.Show();
			break;
		}
	}

}
