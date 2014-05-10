using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeatmapAnalytics))]
public class HeatmapAnalyticsEditor : Editor
{

	
	void OnEnable() {
		HeatmapAnalytics heatmapAnalytics = (HeatmapAnalytics) target;

	}
	
	
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		HeatmapAnalytics heatmapAnalytics = (HeatmapAnalytics) target;

		if ( GUILayout.Button("Load Heatmap Data") ) {
			heatmapAnalytics.LoadHeatmapData();
		}

	}
}