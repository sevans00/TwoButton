﻿////////////////////////////////////////////////////////////////////////////////
//  
// @module Google Ads Unity SDK 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class GoogleAdsExample : MonoBehaviour {



	
	//replace with your ids


	//Android
	private const string ANDROID_INTERSTITIAL_AD_UNIT_ID 	= "ca-app-pub-6101605888755494/3301497967";
	private const string ANDROID_BANNERS_AD_UNIT_ID 		= "ca-app-pub-6101605888755494/1824764765";
	
	//IOS
	private const string IOS_INTERSTITIAL_AD_UNIT_ID 	= "ca-app-pub-6101605888755494/3329373962";
	private const string IOS_BANNERS_AD_UNIT_ID 		= "ca-app-pub-6101605888755494/1852640761";



	private GUIStyle style;
	private GUIStyle style2;

	private GoogleMobileAdBanner banner1;
	private GoogleMobileAdBanner banner2;

	private bool IsInterstisialsAdReady = false;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Start() {

		//Required
		GoogleMobileAd.Init(ANDROID_BANNERS_AD_UNIT_ID, IOS_BANNERS_AD_UNIT_ID);
		GoogleMobileAd.SetInterstisialsUnitID(ANDROID_INTERSTITIAL_AD_UNIT_ID, IOS_INTERSTITIAL_AD_UNIT_ID);


		
		//Optional, add data for better ad targeting
		GoogleMobileAd.SetGender(GoogleGenger.Male);
		GoogleMobileAd.AddKeyword("game");
		GoogleMobileAd.SetBirthday(1989, AndroidMonth.MARCH, 18);
		GoogleMobileAd.TagForChildDirectedTreatment(false);
		
		//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad
		GoogleMobileAd.AddTestDevice("6B9FA8031AEFDC4758B7D8987F77A5A6");



		//More eventts ot explore under GoogleMobileAdEvents class
		GoogleMobileAd.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, OnInterstisialsLoaded);
		GoogleMobileAd.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);

		//listening for InApp Event
		//You will only receive in-app purchase (IAP) ads if you specifically configure an IAP ad campaign in the AdMob front end.
		GoogleMobileAd.addEventListener(GoogleMobileAdEvents.ON_AD_IN_APP_REQUEST, OnInAppRequest);

		InitStyles();

	}
	
	
	private void InitStyles () {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
		
		style2 =  new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {

		float StartY = 20;
		float StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Interstisal Example", style);

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Start Interstitial Ad")) {
			GoogleMobileAd.StartInterstitialAd ();
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Load Interstitial Ad")) {
			GoogleMobileAd.LoadInterstitialAd ();
		}


		StartX += 170;
		GUI.enabled = IsInterstisialsAdReady;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Show Interstitial Ad")) {
			GoogleMobileAd.ShowInterstitialAd ();
		}
		GUI.enabled  = true;


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Banners Example", style);

		GUI.enabled = false;
		if(banner1 == null) {
			GUI.enabled  = true;
		}

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Custom Pos")) {
			banner1 = GoogleMobileAd.CreateAdBanner(300, 100, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Left")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.UpperLeft, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Center")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Top Right")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.UpperRight, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Left")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Center")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Banner Bottom Right")) {
			banner1 = GoogleMobileAd.CreateAdBanner(TextAnchor.LowerRight, GADBannerSize.BANNER);
		}



		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded) {
				GUI.enabled  = true;
			}
		}


		StartY+= 80;
		StartX = 10;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Refresh")) {
			banner1.Refresh();
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Move To Center")) {
			banner1.SetBannerPosition(TextAnchor.MiddleCenter);
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "To Radom Coords")) {
			banner1.SetBannerPosition(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
		}




		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Hide")) {
			banner1.Hide();
		}


		GUI.enabled  = false;
		if(banner1 != null) {
			if(banner1.IsLoaded && !banner1.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Show")) {
			banner1.Show();
		}





		GUI.enabled  = false;
		if(banner1 != null) {
			GUI.enabled  = true;
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Destroy")) {
			GoogleMobileAd.DestroyBanner(banner1.id);
			banner1 = null;

		}
		GUI.enabled  = true;


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Banner 2", style);

		GUI.enabled = false;
		if(banner2 == null) {
			GUI.enabled  = true;
		}

		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Smart Banner")) {
			banner2 =GoogleMobileAd.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.SMART_BANNER);
		}



		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded) {
				GUI.enabled  = true;
			}
		}

		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Refresh")) {
			banner2.Refresh();
		}

		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded && banner2.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Hide")) {
			banner2.Hide();
		}


		GUI.enabled  = false;
		if(banner2 != null) {
			if(banner2.IsLoaded && !banner2.IsOnScreen) {
				GUI.enabled  = true;
			}
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Show")) {
			banner2.Show();
		}

		GUI.enabled  = false;
		if(banner2 != null) {
			GUI.enabled  = true;
		}
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Destroy")) {
			GoogleMobileAd.DestroyBanner(banner2.id);
			banner2 = null;
			
		}

		GUI.enabled  = true;

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnInterstisialsLoaded() {
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen() {
		IsInterstisialsAdReady = false;
	}

	private void OnInAppRequest(CEvent e) {
		//getting product id
		string productId = (string) e.data;
		Debug.Log ("In App Request for product Id: " + productId + " received");
		
		
		//Then you should perfrom purchase  for this product id, using this or another game billing plugin
		//Once the purchase is complete, you should call RecordInAppResolution with one of the constants defined in GADInAppResolution:
		
		GoogleMobileAd.RecordInAppResolution(GADInAppResolution.RESOLUTION_SUCCESS);
		
	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
