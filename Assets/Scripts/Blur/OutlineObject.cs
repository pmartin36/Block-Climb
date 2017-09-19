using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OutlineObject : MonoBehaviour
{
	public Color GlowColor;
	public float LerpFactor = 10;

	private List<Material> _materials = new List<Material>();
	private Color _currentColor;
	private Color _targetColor;

	private static float Angle = 0;

	private static float PercentShine = 1.30f;
	private static float CurrentSpeed = 0f;

	/// <summary>
	/// Cache a child materials so composite object work nicely!
	/// </summary>
	void Awake()
	{
		foreach (var renderer in GetComponentsInChildren<Renderer>())
		{
			_materials.AddRange(renderer.materials);
		}

		for (int i = 0; i < _materials.Count; i++)
		{
			_materials[i].SetColor("_GlowColor", GlowColor);
		}
	}

	private void OnMouseEnter()
	{
		_targetColor = GlowColor;
		enabled = true;

		StartCoroutine("Shine");
	}

	private void OnMouseExit()
	{
		_targetColor = Color.black;

		StopCoroutine("Shine");

		Angle = 0;
		Shader.SetGlobalFloat("_GlowAngle", Angle);
		PercentShine = 1.30f;
		Shader.SetGlobalFloat("_PercentShine", PercentShine);

		enabled = false;
	}

	private void Update()
	{

	}

	private IEnumerator Shine() {
		float startTime = Time.time;
		while(Time.time - startTime <= 1f) {
			Angle = Mathf.Lerp(0,360,(Time.time - startTime)*1);
			Shader.SetGlobalFloat("_GlowAngle", Angle);
			yield return new WaitForEndOfFrame();
		}

		Angle = 360;
		Shader.SetGlobalFloat("_GlowAngle", Angle);

		startTime = Time.time;
		while (Time.time - startTime <= 1.0f){
			PercentShine = Mathf.Lerp(1.30f, -0.30f, Time.time - startTime);
			Shader.SetGlobalFloat("_PercentShine", PercentShine);
			yield return new WaitForEndOfFrame();
		}
	}
}
