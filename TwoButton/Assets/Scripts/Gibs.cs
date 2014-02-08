using UnityEngine;
using System.Collections;

public class Gibs : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("DoGibs");
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
