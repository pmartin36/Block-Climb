using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour {

	Camera mainCamera;

	public void Init(float x) {
		mainCamera = Camera.main;
		transform.position = new Vector2(x, mainCamera.transform.position.y);
		transform.localScale = new Vector2(1f, mainCamera.orthographicSize*2f);

		gameObject.layer = LayerMask.NameToLayer("Block");
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2(transform.position.x, mainCamera.transform.position.y);
	}
}
