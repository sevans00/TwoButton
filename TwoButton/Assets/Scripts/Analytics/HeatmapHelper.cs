using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using MadLevelManager;

//Using GameAnalytics and http://support.gameanalytics.com/hc/en-us/articles/200841566-Downloading-Heatmap-Data
//To download heatmaps
public class HeatmapHelper : MonoBehaviour {

	GA_HeatMapDataFilter dataFilter;

	public void Start() {
		dataFilter = GetComponent<GA_HeatMapDataFilter>();
		
	}

}
