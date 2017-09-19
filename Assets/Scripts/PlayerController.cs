using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public event EventHandler<BlockGatheredEventInfo> BlockGatheredEventHandler;
	public Player Player { get; set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Collectable") {
			CollectableBlock cb = other.GetComponent<CollectableBlock>();
			EventHandler<BlockGatheredEventInfo> handler = BlockGatheredEventHandler;
			if (handler != null) {
				handler(this, new BlockGatheredEventInfo(cb.type, cb.count));
			}
			cb.Collect();
		}
		else if(other.tag == "Ocean") {
			other.gameObject.GetComponent<Water>().GenerateSplash(transform.position, Math.Abs(Player.Velocity.y));
			StartCoroutine(BobInSea(3f));
			StartCoroutine(EndLevelAfterTime(3f));
		}
	}

	private IEnumerator EndLevelAfterTime(float v) {
		yield return new WaitForSeconds(v);

		float startTime = Time.time;
		float fadeTime = 1f;
		while(Time.time - startTime < fadeTime + Time.deltaTime) {
			Shader.SetGlobalColor("_ColorAdd", Color.Lerp(Color.white, Color.black, (Time.time - startTime) / fadeTime));
			yield return new WaitForEndOfFrame();
		}
		GlobalManager.Instance.LevelManager.ResetLevel();
	}

	private IEnumerator BobInSea(float timeToBeginFade) {
		float incomingVelocity = Mathf.Clamp(Player.Velocity.y*2, -5, -1);
		float targetDepth = Water.DrownLine + incomingVelocity;
		float currentDepth = transform.position.y;

		float maxPlayerHeight = Water.DrownLine + Player.Physics.box.size.y * 0.33f;
		float minPlayerHeight = Water.DrownLine - Player.Physics.box.size.y * 0.33f;

		float startTime = Time.time;
		float journeyTime = 0.2f;

		Player.PlayerInWater();

		//shoot down
		while (Time.time - startTime < journeyTime + Time.deltaTime) {
			transform.position = new Vector2(transform.position.x, 
				Mathf.Lerp(currentDepth, targetDepth, (Time.time - startTime) / journeyTime));
			yield return new WaitForEndOfFrame();
		}
		
		//resurface
		journeyTime = Mathf.Abs(maxPlayerHeight - targetDepth);
		currentDepth = transform.position.y;
		while (Time.time - startTime < journeyTime + Time.deltaTime) {
			transform.position = new Vector2(transform.position.x,
				Mathf.Lerp(currentDepth, maxPlayerHeight, (Time.time - startTime) / journeyTime));
			yield return new WaitForEndOfFrame();
		}

		//bob until level ends
		targetDepth = minPlayerHeight;
		currentDepth = maxPlayerHeight;
		journeyTime = (maxPlayerHeight - minPlayerHeight)*2f;
		while(true) {
			startTime = Time.time;
			while(Time.time - startTime < journeyTime + Time.deltaTime) {
				transform.position = new Vector2(transform.position.x,
					Mathf.Lerp(currentDepth, targetDepth, (Time.time - startTime) / journeyTime));
				yield return new WaitForEndOfFrame();
			}

			float temp = targetDepth;
			targetDepth = currentDepth;
			currentDepth = temp;

			//yield return new WaitForSeconds(0.2f);
		}
	}
}
