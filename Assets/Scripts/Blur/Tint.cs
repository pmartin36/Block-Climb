using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tint : MonoBehaviour {

	Material mat;

	void Awake() {
		mat = new Material(Shader.Find("Hidden/Tint"));
		//mat.SetColor("_Tint", new Color(0,0,0.0f,0.5f));
		mat.SetColor("_Tint", Color.green);
	}

	// Update is called once per frame
	void Update() {

	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		Graphics.Blit(src, dst, mat);
	}

}
