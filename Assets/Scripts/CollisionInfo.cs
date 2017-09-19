using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CollisionInfo {
	public bool above, below;
	public bool left, right;

	public bool climbingSlope;
	public bool descendingSlope;
	public bool slidingDownMaxSlope;

	public float slopeAngle, slopeAngleOld;
	public Vector2 slopeNormal;
	public Vector2 moveAmountOld;
	public int faceDir;
	public bool fallingThroughPlatform;

	public void Reset() {
		above = below = false;
		left = right = false;
		climbingSlope = false;
		descendingSlope = false;
		slidingDownMaxSlope = false;
		slopeNormal = Vector2.zero;

		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}

	public CollisionInfo() {
		Reset();
		faceDir = 1;
	}
}
