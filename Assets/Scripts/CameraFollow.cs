using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Player target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector2 focusAreaSize;

	FocusArea focusArea;

	Vector2 PreviousTargetVelocity;

	float screenShakeOffset = 0f;
	float horizontalOffset = 0f;

	private Camera mainCamera;
	public float CameraWidth { get; set; }

	void Start() {
		PreviousTargetVelocity = Vector2.zero;
		mainCamera = GetComponent<Camera>();
		CameraWidth = mainCamera.orthographicSize * mainCamera.aspect;
	}

	public void InitFocusArea(Player p, Vector2 focusSize) {
		target = p;
		transform.position = target.Physics.box.bounds.center;
		focusArea = new FocusArea(target.Physics.box.bounds, focusSize);
	}

	void LateUpdate() {
		float vdiff = PreviousTargetVelocity.y - target.Velocity.y;
		focusArea.Update(target.Physics.box.bounds);
		if(vdiff <= -0.5f && target.Velocity.y >= -0.1f && !target.Drowning) {
			//start screen shake coroutine
			StopCoroutine("ScreenShake");
			StartCoroutine(ScreenShake(vdiff));
		}

		horizontalOffset = 0;
		horizontalOffset -= Mathf.Min(0, (focusArea.center.x - CameraWidth) - LevelBoundsGenerator.LevelBounds.min.x);
		horizontalOffset -= Mathf.Max(0, (focusArea.center.x + CameraWidth) - LevelBoundsGenerator.LevelBounds.max.x);

		Vector2 focusPosition = focusArea.center + new Vector2(horizontalOffset, verticalOffset + screenShakeOffset);

		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
		PreviousTargetVelocity = target.Velocity;
	}

	void OnDrawGizmos() {
		if(focusArea != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(focusArea.center, focusArea.GetSize());
		}
	}

	IEnumerator ScreenShake(float vdiff) {
		float startTime = Time.time - Time.deltaTime;
		float overallStart = startTime;
		
		float divisor = 1f;
		float targetShake = vdiff / divisor;

		//start at middle -> go to extrema -> go back to middle -> half the amplitude -> go to other extrema -> go back to middle -> half -> repeat until 1s has past or amplitude < 0.1
		while (Mathf.Abs(targetShake) > 0.1f) {
			float jtime = (Time.time - startTime) * (4f + Mathf.Abs(targetShake));

			if(jtime <= 0.5f) {
				screenShakeOffset = Mathf.Lerp(0,targetShake, jtime*2);
			}
			else if(jtime <= 1f) {
				screenShakeOffset = Mathf.Lerp(targetShake, 0, (jtime-0.5f)*2);
			}
			else {
				if (Time.time - overallStart > 1) {
					targetShake = 0f;
				}
				else {
					screenShakeOffset = 0f;
					float odivisor = divisor;
					divisor *= 2;
					targetShake *= (-odivisor / divisor);
					startTime = Time.time;
				}
			}

			yield return new WaitForEndOfFrame();
		}
		screenShakeOffset = 0;
	}

	class FocusArea {
		public Vector2 center;
		public Vector2 velocity;
		float left, right;
		float top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			center = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}

			top += shiftY;
			bottom += shiftY;
			center = new Vector2((left + right) / 2, (top + bottom) / 2);

			velocity = new Vector2(shiftX, shiftY);
		}

		public Vector2 GetSize() {
			return new Vector2( right-left, top-bottom );
		}
	}

}

