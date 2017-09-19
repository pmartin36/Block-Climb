using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour {

	protected SpriteRenderer spriteRender;
	public Sprite Sprite{
		get {
			return spriteRender.sprite;
		}
	}
	public Color SpriteColor {
		get {
			return spriteRender.color;
		}
		set {
			spriteRender.color = value;
		}
	}
	public Color GlowColor
	{
		get {
			return material.GetColor("_GlowColor");
		}
		set {
			material.SetColor("_GlowColor", value);
		}
	}

	public bool InPlacementMode { get; set; }
	public bool ValidPlacement { get; set; }
	public static int OrderNumber = -1000;
	protected Material material;

	public ParticleSystem WaitingToPlaceParticleSystem { get;set; }
	public ParticleSystem DuringPlaceParticleSystem { get; set; }

	BoxCollider2D box;

	static Sprite []triangleSprites;
	static Sprite []blockSprites;

	public bool Destroyed { get; set; }

	// Use this for initialization
	protected virtual void Awake () {
		spriteRender = GetComponent<SpriteRenderer>();
		material = GetComponent<Renderer>().material;

		GetParticleSystems();
		box = GetComponent<BoxCollider2D>();
	}

	protected virtual void Start() {

	}

	// Update is called once per frame
	protected virtual void Update () {
		
	}

	public virtual void GetParticleSystems() {
		foreach (Transform child in transform)
		{
			ParticleSystem p = child.GetComponent<ParticleSystem>();
			if (p != null)
			{
				if (p.gameObject.activeSelf)
				{
					WaitingToPlaceParticleSystem = p;
				}
				else
				{
					DuringPlaceParticleSystem = p;
				}
			}
		}
	}

	public virtual bool CreateOrDestroyForPlacement() {
		RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, box.bounds.size, 0, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Player"));
		if(hit) {
			//player is in box with block, destroy block
			Destroyed = true;
			Destroy(this.gameObject);
			return false;
		}
		else {
			//create the block
			gameObject.layer = LayerMask.NameToLayer("Block");
			return true;
		}
	}

	public virtual void PlacementModeInit() {
		InPlacementMode = true;
		spriteRender.color = new Color(1,1,1,0.5f);

		spriteRender.sortingOrder = 2;

		GlowColor = Color.white;
	}

	public virtual void PlacementModeEnd() {
		InPlacementMode = false;
		spriteRender.color = Color.white;

		spriteRender.sortingOrder = OrderNumber++;

		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem p in ps) {
			Destroy(p.gameObject);
		}
	}

	public static void SetTriangleSprites(BlockTypes t, SpriteRenderer half, SpriteRenderer quarter) {
		if(triangleSprites == null) {
			triangleSprites = Resources.LoadAll<Sprite>("Sprites/Triangles");
		}
		switch (t) {
			case BlockTypes.Dirt:
				half.sprite = triangleSprites[6];
				quarter.sprite = triangleSprites[7];
				break;
			case BlockTypes.Clay:
				half.sprite = triangleSprites[4];
				quarter.sprite = triangleSprites[5];
				break;
			case BlockTypes.Steel:
				half.sprite = triangleSprites[0];
				quarter.sprite = triangleSprites[1];
				break;
			case BlockTypes.Cloud:
				half.sprite = triangleSprites[2];
				quarter.sprite = triangleSprites[3];
				break;
			default:
				break;
		}
	}

	public static Sprite GetBlockSprite(BlockTypes t) {
		if(blockSprites == null) {
			blockSprites = Resources.LoadAll<Sprite>("Sprites/Blocks");
		}
		switch (t) {
			default:
			case BlockTypes.Dirt:
				return blockSprites[2];
			case BlockTypes.Clay:
				return blockSprites[1];
			case BlockTypes.Steel:
				return blockSprites[0];
			case BlockTypes.Cloud:
				return blockSprites[3];
		}
	}

	public static Texture2D TextureFromSprite(Sprite sprite) {
		Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
		Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
													 (int)sprite.textureRect.y,
													 (int)sprite.textureRect.width,
													 (int)sprite.textureRect.height);
		newText.SetPixels(newColors);
		newText.Apply();
		return newText;
	}
}
