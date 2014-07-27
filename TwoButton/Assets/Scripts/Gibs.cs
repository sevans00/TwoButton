using UnityEngine;
using System.Collections;

public class Gibs : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("DoGibs");
		//TestBlood();
	}
	void TestBlood () {
		tk2dTileMap tilemap = GameObject.FindObjectOfType<tk2dTileMap>();
		int x,y;
		bool getTileResult;
		int tileid;
		getTileResult = tilemap.GetTileAtPosition(transform.position, out x, out y);
		tileid = tilemap.GetTile(x, y, 0);
		Debug.LogWarning("0"+tileid);
		tileid = tilemap.GetTile(x, y, 1);
		Debug.LogWarning("1"+tileid);
		tileid = tilemap.GetTile(x, y, 2);
		Debug.LogWarning("2"+tileid);
		int layer;
		GameObject instance;
		tilemap.GetTilePrefabsListItem(0, out x, out y, out layer, out instance);
		Debug.LogWarning("instance:"+instance);

		tk2dSprite sprite = instance.GetComponent<tk2dSprite>();
		sprite.color = Color.blue;




	}

	IEnumerator DoGibs() {
		tk2dSprite sprite = GetComponent<tk2dSprite>();
		sprite.scale = Vector3.one * Random.Range(0.8f, 1.2f);
		gameObject.transform.RotateAround(transform.position,Vector3.forward, Random.Range(0f, 89f));
		Color spritecolor = sprite.color;
		spritecolor.a = 0.6f;
		sprite.color = spritecolor;
		return null;
	}

}
