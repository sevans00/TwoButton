////////////////////////////////////////////////////////////////////////////////
//  
// @module Google Ads Unity SDK 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//Attach the script to the empty gameobject on your sceneS
public class AdMobBannerInterstitial : MonoBehaviour {


	public string AndroidInterstitialUnityId;
	public string IOSInterstitialUnityId;


	// --------------------------------------
	// Unity Events
	// --------------------------------------
	
	void Awake() {


		if(GoogleMobileAd.IsInited) {

			string unit_id = AndroidInterstitialUnityId;
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
				unit_id = IOSInterstitialUnityId;
			}


			if(!GoogleMobileAd.InterstisialUnitId.Equals(unit_id)) {
				GoogleMobileAd.SetInterstisialsUnitID(AndroidInterstitialUnityId, IOSInterstitialUnityId);
			}


		} else {
			GoogleMobileAd.Init(AndroidInterstitialUnityId, IOSInterstitialUnityId);
		}



	}

	void Start() {
		ShowBanner();
	}




	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		GoogleMobileAd.StartInterstitialAd();
	}



	// --------------------------------------
	// GET / SET
	// --------------------------------------



	public string sceneBannerId {
		get {
			return Application.loadedLevelName + "_" + this.gameObject.name;
		}
	}

	
}
