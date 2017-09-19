using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

	public float BoxSize;
	public static float DrownLine;
	public BoxCollider2D box;

	GameObject SplashPrefab;

	// Use this for initialization
	void Start () {
		BoxSize = 4f;

		ParticleSystem ps = GetComponentInChildren<ParticleSystem>();

		box = gameObject.AddComponent<BoxCollider2D>();
		box.offset = Vector2.zero;
		box.size = ps.shape.box + new Vector3(0,ps.main.startSize.constant/4f);
		box.isTrigger = true;

		DrownLine = transform.position.y + box.size.y/2f;

		SplashPrefab = Resources.Load<GameObject>("Prefabs/Splash");

		LevelBoundsGenerator.Create(box.bounds);
	}
	
	// Update is called once per frame
	void Update () {
		//transform.position = new Vector2(Camera.main.transform.position.x, transform.position.y);
	}

	public void GenerateSplash(Vector2 location, float momentum) {
		GameObject splash = Instantiate(SplashPrefab, new Vector2(location.x, DrownLine), SplashPrefab.transform.rotation, this.transform);
		ParticleSystem p = splash.GetComponent<ParticleSystem>();

		var main = p.main;
		var ss = p.main.startSpeed;
		//set speed and # of particles based on incoming object's momentum
		main.startSpeedMultiplier *= Math.Min(momentum, 5);

		var shape = p.shape;
		shape.angle = Mathf.Clamp(Mathf.Pow(momentum, 2) * shape.angle, 12, 45);

		int emissionCount = (int)(50 * Mathf.Pow(momentum,2));
		p.Emit(emissionCount);

		Debug.Log(p.main.startSpeed.constantMax);
	}
}
