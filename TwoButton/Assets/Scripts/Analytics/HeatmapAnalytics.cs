using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using MadLevelManager;

//Using GameAnalytics and http://support.gameanalytics.com/hc/en-us/articles/200841566-Downloading-Heatmap-Data
//To download heatmaps
public class HeatmapAnalytics : MonoBehaviour {

	public Color gizmoColor = Color.red;
	private string _baseURL = "http://data-api.gameanalytics.com";

	public Hashtable returnParam;

	public void LoadHeatmapData () {
		Debug.Log("Start fn:");
		doLoad();
	}

	public void Start () {
		//doLoad();
	}

	public void doLoad () {
		Debug.Log("Loading heatmap data");
		GA_Request request = new GA_Request();
		//request.RequestGameInfo(gameInfoSuccess, gameInfoError);

		List<string> list = new List<string>();
		list.Add("Death");
		request.RequestHeatmapData(list, MadLevel.currentLevelName, "0.30", heatmapSuccess, gameInfoError);

	}
	public void heatmapSuccess(GA_Request.RequestType requestType, Hashtable returnParam, GA_Request.SubmitErrorHandler errorEvent)
	{
		Debug.Log("Success");
		Debug.Log("Hash:"+returnParam.ToString());
		Debug.Log("x:"+(returnParam["x"] as ArrayList).Count);
		Debug.Log("y:"+(returnParam["y"] as ArrayList).Count);
		this.returnParam = returnParam;
		// Get all values with an enumeration of the keys
		foreach ( string key in returnParam.Keys )
		{
			Debug.Log(key+ ": "+ returnParam[key]);
			Debug.Log(key+ ": "+ (returnParam[key] as ArrayList));
			ArrayList array = (returnParam[key] as ArrayList);
			for ( var ii = 0; ii < array.Count; ii++ ) {
				Debug.Log(" - "+array[ii]);
			}
		}
	}




	public void gameInfoSuccess(GA_Request.RequestType requestType, Hashtable returnParam, GA_Request.SubmitErrorHandler errorEvent)
	{
		Debug.Log("Success");
		Debug.Log("Hash:"+returnParam.ToString());
		this.returnParam = returnParam;
		// Get all values with an enumeration of the keys
		foreach ( string key in returnParam.Keys )
		{
			Debug.Log(key+ ": "+ returnParam[key]);
			ArrayList array = (returnParam[key] as ArrayList);
			for ( var ii = 0; ii < array.Count; ii++ ) {
				Debug.Log(" - "+array[ii]);
			}
		}

	}
	public void gameInfoError(string message)
	{
		Debug.Log("Failure");
	}




	public void OnDrawGizmos () {



		DrawSphereHere(Vector3.zero);
		DrawSphereHere(Vector3.one);
		DrawSphereHere(Vector3.one*0.1f);
		DrawSphereHere(Vector3.one*0.2f);

	}

	public void DrawSphereHere ( Vector3 position ) {
		Gizmos.color = gizmoColor;
		Gizmos.DrawSphere(position, 1);
	}

}
