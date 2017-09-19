using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectableBlock : MonoBehaviour {

	public BlockTypes type;
	public int count;

	public Quaternion targetRotation;
	SpriteRenderer spriteRenderer;

	Material m;

	public static GameObject BlockMarkerContainer;
	public bool markedForDeletion = false;

	// Use this for initialization
	void Start () {
		targetRotation = Quaternion.Euler(new Vector3(0,0,60));

		spriteRenderer = GetComponent<SpriteRenderer>();

		//Convert Sprite to Texture2D 
		Sprite sprite = Block.GetBlockSprite(type);
		Texture2D newText = Block.TextureFromSprite(sprite);

		Texture2D borderTex = Resources.Load<Texture2D>("Sprites/CollectableBlockBorder");

		// Set Material properties
		m = new Material(Shader.Find("Unlit/CollectableBlock"));
		m.SetColor("_BorderColor", Color.black);
		m.SetTexture("_Block", newText);
		m.SetTexture("_Border", borderTex);

		spriteRenderer.material = m;

		StartCoroutine(Rock());

		if(BlockMarkerContainer == null) {
			BlockMarkerContainer = new GameObject("Block Markers");
			BlockMarkerContainer.transform.position = new Vector3(0,0,-8.1f);
		}

		//Create Block Marker
		GameObject marker = new GameObject("Marker");
		marker.transform.parent = BlockMarkerContainer.transform;
		marker.AddComponent<CollectableMarker>().Init(this);
	}

	public void Collect() {
		StopCoroutine(Rock());
		StartCoroutine(StartDestroy());
	}

	void Update() {
		
	}

	IEnumerator StartDestroy() {
		//will be destroyed soon -- despawn the marker
		markedForDeletion = true;

		gameObject.GetComponent<BoxCollider2D>().enabled = false;

		float startTime = Time.time;
		float jtime = 1f + Time.deltaTime;

		Vector2 startPosition = transform.localPosition;
		Vector2 endPosition = startPosition + Vector2.up * 1f;

		m.SetColor("_BorderColor", Color.white);

		while (Time.time - startTime < jtime) {
			float ptime = (Time.time - startTime) / jtime;
			transform.localPosition = Vector2.Lerp(startPosition, endPosition, ptime);

			m.SetColor("_Color", Color.Lerp(Color.white, Color.clear, ptime));

			transform.Rotate(new Vector3(0,0,1));
			spriteRenderer.material = m;

			yield return new WaitForEndOfFrame();
		}
	
		Destroy(this.gameObject);

		yield return null;
	}

	IEnumerator Rock() {
		yield return new WaitForEndOfFrame();

		float startTime;
		Quaternion startRotation;
		float jtime = 1.5f + Time.deltaTime;

		while(true) {
			startTime = Time.time;
			startRotation = targetRotation;

			targetRotation = Quaternion.Euler(new Vector3(0, 0, -startRotation.eulerAngles.z));

			while (Time.time - startTime < jtime) {
				float ptime = (Time.time - startTime) / jtime;

				transform.rotation = Quaternion.Slerp(startRotation, targetRotation, ptime);

				yield return new WaitForEndOfFrame();
			}
			
		}
	}
}
