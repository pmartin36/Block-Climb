using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaw : MonoBehaviour {

	void OnEnable()
	{
		RenderTexture PrePass = new RenderTexture(Screen.width, Screen.height, 24);
		PrePass.antiAliasing = QualitySettings.antiAliasing;

		var camera = GetComponent<Camera>();
		var glowShader = Shader.Find("Hidden/OutlineReplace");
		camera.targetTexture = PrePass;
		camera.SetReplacementShader(glowShader, "Outlinable");

		Shader.SetGlobalTexture("_OutlinePrePassTex", PrePass);
	}
}
