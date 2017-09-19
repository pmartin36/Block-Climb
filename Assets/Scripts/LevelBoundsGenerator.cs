using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelBoundsGenerator{

	public static Bounds LevelBounds;

	public static void Create(Bounds bounds) {
		bounds.size = new Vector2(bounds.size.x, 1000f);
		bounds.center = (Vector2)bounds.center;
		LevelBounds = bounds;

		GameObject leftBound = new GameObject("Left Bound");
		leftBound.AddComponent<LevelBounds>().Init(bounds.min.x - 0.5f);

		GameObject rightBound = new GameObject("Right Bound");
		rightBound.AddComponent<LevelBounds>().Init(bounds.max.x + 0.5f);
	}
}
