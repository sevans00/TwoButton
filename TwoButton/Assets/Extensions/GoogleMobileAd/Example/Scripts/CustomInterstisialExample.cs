using UnityEngine;
using System.Collections;

public class CustomInterstisialExample : MonoBehaviour {

	//Android
	private const string ANDROID_INTERSTITIAL_AD_UNIT_ID 	= "ca-app-pub-6101605888755494/3301497967";
	private const string ANDROID_BANNERS_AD_UNIT_ID 		= "ca-app-pub-6101605888755494/1824764765";
	
	//IOS
	private const string IOS_INTERSTITIAL_AD_UNIT_ID 	= "ca-app-pub-6101605888755494/3329373962";
	private const string IOS_BANNERS_AD_UNIT_ID 		= "ca-app-pub-6101605888755494/1852640761";




	void Start () {
		GoogleMobileAd.Init(ANDROID_BANNERS_AD_UNIT_ID, IOS_BANNERS_AD_UNIT_ID);
		GoogleMobileAd.SetInterstisialsUnitID(ANDROID_INTERSTITIAL_AD_UNIT_ID, IOS_INTERSTITIAL_AD_UNIT_ID);

		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, OnInterstisialsLoaded);
		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);

		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_CLOSED, OnInterstisialsClosed);

		//loadin ad:
		GoogleMobileAd.LoadInterstitialAd ();
	}

	private void OnInterstisialsLoaded() {
		//ad loaded, strting ad
		GoogleMobileAd.ShowInterstitialAd ();

	}

	private void OnInterstisialsOpen() {
		//pausing the game
	}

	private void OnInterstisialsClosed() {
		//un-pausing the game
	}

}
