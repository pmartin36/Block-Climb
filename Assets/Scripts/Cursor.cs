using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cursor : MonoBehaviour {

	public Vector2 CameraPositionAtLastSet { get; set; }
	public Vector2 InputPosition { get; set; }
	public Vector2 Position {
		get {
			return transform.position;
		}
		private set {
			transform.position = value;
		}
	}
	private SpriteRenderer spriteRenderer;

	private int boxSize;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();

		UnityEngine.Cursor.visible = false;

		boxSize = 11;

		//Generate initial texture
		Texture2D ptex = new Texture2D(boxSize, boxSize, TextureFormat.ARGB32, false);
		ptex.wrapMode = TextureWrapMode.Clamp;
		ptex.filterMode = FilterMode.Point;	

		StartCoroutine(ColorizeCursor(ptex));
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Position = InputPosition + GetBlockedPosition((Vector2)Camera.main.transform.position - CameraPositionAtLastSet);
	}

	public void SetPosition(Vector2 location) {
		InputPosition = GetBlockedPosition(location);
		CameraPositionAtLastSet = Camera.main.transform.position;
		Position = InputPosition + GetBlockedPosition((Vector2)Camera.main.transform.position - CameraPositionAtLastSet);
	}

	public static Vector2 GetBlockedPosition(Vector2 unblocked) {
		return new Vector2(
			Mathf.Round(unblocked.x * 2f) * 0.5f,
			Mathf.Round(unblocked.y * 2f) * 0.5f
		);
	}

	IEnumerator ColorizeCursor(Texture2D targetTexture) {
		bool firstIteration = true;
		Color32[] pixelColors = Enumerable.Repeat((Color32)Color.clear, targetTexture.width * targetTexture.height).ToArray();
		//spriteRenderer.color = Color.white;

		List<Color32> startColor;
		List<Color32> targetColor = pixelColors.ToList();	

		while(true) {
			//generate new random colors
			startColor = targetColor.ToList();
			targetColor = targetColor.Select((t,i) => {
				int row = i / boxSize;
				int col = i % boxSize;
				if((row == 2 || row == 3 || row == 7 || row == 8)
					&&  (col > 1 && col < 9)) {
					return (Color32)Color.clear;
				}
				else if((col == 2 || col == 3 || col == 7 || col == 8)
					&& (row > 1 && row < 9)){
					return (Color32)Color.clear;
				}

				Color32 newColor = new Color32();
				newColor.r = (byte)Random.Range(128, 255);
				newColor.g = (byte)Random.Range(128, 255);
				newColor.b = (byte)Random.Range(128, 255);
				newColor.a = 128;
				return newColor;
			}).ToList();

			yield return new WaitForEndOfFrame();

			float startTime = Time.time;
			float colorTransitionTime = 1f;
			while(Time.time - startTime < colorTransitionTime + Time.deltaTime) {

				pixelColors = pixelColors.Select((t,i) => {		
					return Color32.Lerp(startColor[i], targetColor[i], (Time.time - startTime) / colorTransitionTime);
				}).ToArray();

				targetTexture.SetPixels32(pixelColors);
				targetTexture.Apply();

				Sprite sprite = Sprite.Create(targetTexture, new Rect(0, 0, targetTexture.width, targetTexture.height), Vector2.one / 2f, boxSize);
				spriteRenderer.sprite = sprite;

				if(firstIteration) {
					spriteRenderer.color = Color.Lerp(Color.clear, Color.white, (Time.time - startTime) / colorTransitionTime);
				}

				yield return new WaitForEndOfFrame();
			}

			//hold these colors for 1 seconds
			//yield return new WaitForSeconds(1);
			firstIteration = false;					
		}
	}
}
