﻿////////////////////////////////////////////////////////////////////////////////
//  
// @module Google Ads Unity SDK 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class GoogleMobileAd  {

	public static GoogleMobileAdInterface controller;
	private static bool _IsInited = false ;

	public static void Init(string android_unit_id, string ios_unit_id) {
	
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			controller = IOSAdMobController.instance;
			controller.Init(ios_unit_id);
		} else {
			controller =  AndroidAdMobController.instance;
			controller.Init(android_unit_id);
		}

		_IsInited = true;

	}


	public static void SetBannersUnitID(string android_unit_id, string ios_unit_id) {
		if(!_IsInited) {
			Debug.LogWarning ("ChangeBannersUnitID shoudl be called only after Init function. Call ignored");
			return;
		}

		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			controller.SetBannersUnitID(ios_unit_id);
		} else {
			controller.SetBannersUnitID(android_unit_id);
		}

	}


	public static void SetInterstisialsUnitID(string android_unit_id, string ios_unit_id) {
		if(!_IsInited) {
			Debug.LogWarning ("ChangeInterstisialsUnitID shoudl be called only after Init function. Call ignored");
			return;
		}

		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			controller.SetInterstisialsUnitID(ios_unit_id);
		} else {
			controller.SetInterstisialsUnitID(android_unit_id);
		}

	}



	public static GoogleMobileAdBanner CreateAdBanner(TextAnchor anchor, GADBannerSize size)  {
		if(!_IsInited) {
			Debug.LogWarning ("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}


		return controller.CreateAdBanner(anchor, size);
	}


	public static GoogleMobileAdBanner CreateAdBanner(int x, int y, GADBannerSize size)  {
		if(!_IsInited) {
			Debug.LogWarning ("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}

		return controller.CreateAdBanner(x, y, size);
	}

	

	public static GoogleMobileAdBanner GetBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("GetBanner shoudl be called only after Init function. Call ignored");
			return null;
		}
		
		return controller.GetBanner(id);
	}


	public static void DestroyBanner(int id) {
		if(!_IsInited) {
			Debug.LogWarning ("DestroyCurrentBanner shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.DestroyBanner(id);
	}





	//Add a keyword for targeting purposes.
	public static void AddKeyword(string keyword)  {
		if(!_IsInited) {
			Debug.LogWarning ("AddKeyword shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.AddKeyword(keyword);
		
	}

	public static void SetBirthday(int year, AndroidMonth month, int day)  {
		if(!_IsInited) {
			Debug.LogWarning ("SetBirthday shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.SetBirthday(year, month, day);
		
	}

	public static void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment) {
		if(!_IsInited) {
			Debug.LogWarning ("TagForChildDirectedTreatment shoudl be called only after Init function. Call ignored");
			return;
		}

		controller.TagForChildDirectedTreatment(tagForChildDirectedTreatment);
	}
	
	//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad.
	public static void AddTestDevice(string deviceId) {
		if(!_IsInited) {
			Debug.LogWarning ("AddTestDevice shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.AddTestDevice(deviceId);
	}

	//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad.
	public static void AddTestDevices(params string[] ids) {
		if(!_IsInited) {
			Debug.LogWarning ("AddTestDevice shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.AddTestDevices(ids);
	}
	
	//Set the user's gender for targeting purposes. This should be GADGenger.GENDER_MALE, GADGenger.GENDER_FEMALE, or GADGenger.GENDER_UNKNOWN
	public static void SetGender(GoogleGenger gender) {
		if(!_IsInited) {
			Debug.LogWarning ("SetGender shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.SetGender(gender);
	}
	
	
	


	public static void StartInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("StartInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.StartInterstitialAd();
	}
	
	public static void LoadInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("LoadInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.LoadInterstitialAd();
	}
	
	public static void ShowInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("ShowInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.ShowInterstitialAd();
	}

	public static void RecordInAppResolution(GADInAppResolution resolution) {
		if(!_IsInited) {
			Debug.LogWarning ("RecordInAppResolution shoudl be called only after Init function. Call ignored");
			return;
		}
		
		controller.RecordInAppResolution(resolution);
	}
	


	//--------------------------------------
	//  GET / SET
	//--------------------------------------


	public static bool IsInited {
		get {
			return _IsInited;
		}
	}
	
	public static string BannersUunitId {
		get {
			return controller.BannersUunitId;
		}
	}
	
	public static string InterstisialUnitId {
		get {
			return controller.InterstisialUnitId;
		}
	}

	//--------------------------------------
	// EVENTS Impl
	//--------------------------------------


	public static void addEventListener(string eventName, EventHandlerFunction handler) {
		if(controller == null) {
			return;
		}
		controller.addEventListener(eventName, handler);
	}


	public static void addEventListener(string eventName, DataEventHandlerFunction handler) {
		if(controller == null) {
			return;
		}
		controller.addEventListener(eventName, handler);
	}


	
	public static void removeEventListener(string eventName, 	EventHandlerFunction handler) {
		if(controller == null) {
			return;
		}
		controller.removeEventListener(eventName, handler);
	}


	public static void removeEventListener(string eventName,  DataEventHandlerFunction handler) {
		if(controller == null) {
			return;
		}
		controller.removeEventListener(eventName, handler);
	}
}


