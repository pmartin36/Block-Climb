using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Triangle : MonoBehaviour {

	[HideInInspector]
	public TriangleInfo info;

	[HideInInspector]
	public Block ConnectingBlock1, ConnectingBlock2;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;
	public Color GlowColor {
		get
		{
			return spriteRenderer.material.GetColor("_GlowColor");
		}
		set
		{
			spriteRenderer.material.SetColor("_GlowColor", value);
		}
	}

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void TriangleInit(TriangleInfo ti, Block b1, Block b2) {
		info = ti;
		ConnectingBlock1 = b1;
		ConnectingBlock2 = b2;
		transform.localScale = new Vector2( ti.Scale.x * ti.DirectionX, ti.Scale.y);
		GlowColor = Color.white;
	}

	public bool CreateOrDestroyForPlacement() {
		if(ConnectingBlock1 == null || ConnectingBlock1.Destroyed ||
			ConnectingBlock2 == null || ConnectingBlock2.Destroyed) {
			Destroy(this.gameObject);
			return false;
		}
		
		float angle = info.IsHalfSlope ? 45f : 27.5f;
		angle *= -info.DirectionX;

		RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, GetComponent<PolygonCollider2D>().bounds.size, angle, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Player"));
		if (hit) {
			Destroy(this.gameObject);
			return false;
		}

		gameObject.layer = LayerMask.NameToLayer("Block");

		return true;
	}

	public void PlacementModeEnd() {
		spriteRenderer.color = Color.white;
		spriteRenderer.sortingOrder = -1000;
	}
}

class TriangleInfo {

	public float DirectionX { get; set; }
	public Vector2 Location;
	public bool IsHalfSlope { get; set; }
	public Vector2 Scale;

	//these are the two blocks the triangle is joining
	[HideInInspector]
	public Vector2 ConnectingBlockLocation;

	public TriangleInfo() : this(1, Vector2.zero, true, Vector2.one) {}
	public TriangleInfo(float directionX, Vector2 location, bool isHalfSlope, Vector2 scale) {
		DirectionX = directionX;
		Location = location;
		IsHalfSlope = isHalfSlope;
		Scale = scale;
	}
}
