using UnityEngine;
using System.Collections;

public class ImmersiveMode : Singletone<ImmersiveMode> {


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void EnableImmersiveMode()  {
		AndroidNative.enableImmersiveMode();
	}

}
