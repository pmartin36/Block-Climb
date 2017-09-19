using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMarker : MonoBehaviour {

	CollectableBlock MarkingBlock;
	Bounds MarkingBlockBounds;

	Bounds CameraBounds;

	public bool Visible { get; private set; }
	Material m;

	SpriteRenderer spriteRenderer;

	Camera mainCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	public void Init(CollectableBlock mb) {
		mainCamera = Camera.main;
		MarkingBlock = mb;
		
		SpriteRenderer mbSr = mb.gameObject.GetComponent<SpriteRenderer>();
		MarkingBlockBounds = mbSr.bounds;
		m = new Material(mbSr.material);
		m.SetFloat("_DistanceBasedOpacity",1f);

		spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = mbSr.sprite;
		spriteRenderer.material = m;
		spriteRenderer.sortingLayerName = "UI";
		spriteRenderer.sortingOrder = 1;

		transform.localScale = mb.transform.localScale;

		SetPositionAndDisplay();
	}

	// Update is called once per frame
	void LateUpdate () {
		if(MarkingBlock == null || MarkingBlock.markedForDeletion) {
			Destroy(this.gameObject);
		}
		else {
			SetPositionAndDisplay();
			m.SetFloat("_Distance", transform.position.x - MarkingBlockBounds.center.x);
		
			float angle = transform.localRotation.eulerAngles.z;
			angle = angle > 180 ? angle-360 : angle;
			m.SetFloat("_Angle", angle * Mathf.Deg2Rad);
		}
	}

	private void SetPositionAndDisplay() {
		GenerateCameraBounds();

		Visible = !MarkingBlockWithinCameraBounds();

		SetDisplayOptions();
		CalculatePosition();
	}

	bool MarkingBlockWithinCameraBounds() {
		return CameraBounds.Contains(MarkingBlockBounds.center);
	}

	void CalculatePosition() {
		//move marker along the with the camera x axis
		float sign = Mathf.Sign(MarkingBlockBounds.center.x - CameraBounds.center.x);
		transform.localPosition = new Vector3(sign > 0 ? CameraBounds.max.x : CameraBounds.min.x, MarkingBlockBounds.center.y, 0);
		transform.localRotation = MarkingBlock.transform.rotation;
	}

	private void GenerateCameraBounds() {
		//Blocks are at z=-8 which is why the bounds are centered around -8
		CameraBounds = new Bounds(	new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -8),
									new Vector3(mainCamera.orthographicSize*2*mainCamera.aspect, mainCamera.orthographicSize * 2,4));
	}

	void SetDisplayOptions() {
		if(Visible) {
			m.SetColor("_Color", Color.white);
		}
		else {
			m.SetColor("_Color", Color.clear);
		}
	}
}
