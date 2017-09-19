using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFader : MonoBehaviour {

	Material mat;

	void Awake() {
		mat = new Material(Shader.Find("Unlit/SolidColor"));

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		Graphics.Blit(src,dst,mat);
	}
}
