using UnityEngine;
using System.Collections;

public class ScrollCloudTexture : MonoBehaviour {

	public float scrollSpeed = 0.5F;

	void Start () {
		Material mat = renderer.material;
		Shader shader = mat.shader;
	}

	// Update is called once per frame
	void Update () {
		float offset = Time.time * scrollSpeed;
		renderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
	}
}
