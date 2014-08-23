using UnityEngine;
using System.Collections;
using MadLevelManager;

[System.Serializable]
public class ParallaxBGData : ScriptableObject {
	public string MadGroupName = "(default)";
	public string spriteName = "BG_Forest";
	public Color topColor;
	public Color bottomColor;
}
