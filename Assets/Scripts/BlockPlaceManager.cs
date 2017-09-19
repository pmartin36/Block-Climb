using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockPlaceManager : MonoBehaviour {
	public Dictionary<BlockTypes, int> BlocksAvailable;

	public bool RedrawNeeded { get; private set; }
	private Type _actualBlockType;
	public BlockTypes SelectedBlockType { get; private set; }

	private int selectedBlockCount = 0;
	
	private GameObject blockParent;
	private GameObject blockPrefab;
	private SpriteRenderer halfTrianglePrefab;
	private SpriteRenderer quarterTrianglePrefab;

	private List<Triangle> TrianglesInPlacement;
	private List<Block> BlocksInPlacement;

	public event EventHandler<BlockPlacedEventInfo> BlocksPlacedEventHandler;

	public Vector2 MouseDownLocation {get; private set; }
	public Cursor BlockCursor { get; private set; }

	private OutlineComposite compositeCamera;
	private OutlinePrePass prepassCamera;

	public bool InBlockPlaceMode { get; private set; }

	private const float blockSize = 1f;

	// Use this for initialization
	void Awake () {
		GameObject bc = Instantiate(Resources.Load<GameObject>("Prefabs/Cursor"), Vector3.zero, Quaternion.identity);
		BlockCursor = bc.GetComponent<Cursor>();	

		BlocksAvailable = new Dictionary<BlockTypes, int>();
		BlocksInPlacement = new List<Block>();

		TrianglesInPlacement = new List<Triangle>();

		blockPrefab = Resources.Load<GameObject>("Prefabs/PlacementBlock");
		blockParent = new GameObject("Block Parent");
	}
	
	void Start() {
		//Get Handle to Camera Effect Classes
		compositeCamera = Camera.main.GetComponent<OutlineComposite>();
		prepassCamera = Camera.main.GetComponentInChildren<OutlinePrePass>();
	}

	// Update is called once per frame
	void Update () {
		selectedBlockCount = BlocksAvailable[SelectedBlockType];
		if(RedrawNeeded) {
			SetPlacementBlocks();
		}
	}

	public void SetSelectedBlockType(BlockTypes type) {
		SelectedBlockType = type;		
		RedrawNeeded = true;
		switch (type)
		{
			default:
			case BlockTypes.Dirt:
				_actualBlockType = typeof(BlockDirt);
				break;
			case BlockTypes.Clay:
				_actualBlockType = typeof(BlockClay);
				break;
			case BlockTypes.Steel:
				_actualBlockType = typeof(BlockSteel);
				break;
			case BlockTypes.Cloud:
				_actualBlockType = typeof(BlockCloud);
				break;
		}

		if(halfTrianglePrefab == null) {
			halfTrianglePrefab = Resources.Load<GameObject>("Prefabs/HalfTriangle").GetComponent<SpriteRenderer>();
			quarterTrianglePrefab = Resources.Load<GameObject>("Prefabs/QuarterTriangle").GetComponent<SpriteRenderer>();
		}
		Block.SetTriangleSprites(type, halfTrianglePrefab, quarterTrianglePrefab);
	}

	public void SetBlockCursorPosition(Vector2 location) {
		Vector2 lastPosition = BlockCursor.Position;
		BlockCursor.SetPosition(location);
		if(Mathf.Abs(Vector2.Distance(lastPosition, BlockCursor.Position)) > 0.0001) {
			RedrawNeeded = true;
		}		
	}

	public void SetPlacementBlocks() {
		if (InBlockPlaceMode)
		{
			List<Vector2> bl = new List<Vector2>();
			List<TriangleInfo> tl = new List<TriangleInfo>();

			Vector2 d = (BlockCursor.Position - MouseDownLocation); //get difference
			int x_steps = (int)Mathf.Ceil(Mathf.Abs(d.x));
			int y_steps = (int)Mathf.Ceil(Mathf.Abs(d.y));

			//primary direction should have minimal overlapping
			//secondary direction should have maximum overlap
			//if directions are equal, minimum overlap
			int big = (int)Mathf.Max(x_steps, y_steps);
			if (big == 0)
			{
				//add one point at BlockCursor.Position
				bl.Add(BlockCursor.Position);
			}
			else
			{
				for (int i = 0; i <= big && i < selectedBlockCount; i++)
				{
					//round x/y to nearest 0.5f
					//lerp based off the maximum this way the smaller range only increases when necessary
					float y = Mathf.Round(Mathf.Lerp(MouseDownLocation.y, BlockCursor.Position.y, i / (float)big) * 2f) / 2f;
					float x = Mathf.Round(Mathf.Lerp(MouseDownLocation.x, BlockCursor.Position.x, i / (float)big) * 2f) / 2f;
					Vector2 blockLocation = new Vector2(x,y);

					//Check for if adjoining triangles are needed
					if (i > 0)
					{
						Vector2 last = bl.Last();
						//add triangles if 45° or less
						float angle = Vector2.Angle( new Vector2( x-last.x, y-last.y ), Vector2.up);

						if(angle >= 45 && angle <= 135 && angle != 90) {
							TriangleInfo t = new TriangleInfo();
							t.ConnectingBlockLocation = blockLocation;

							float signdx = Mathf.Sign(x - last.x);

							if (y - last.y < 0) {
								//new block is below old block
								t.Location.y = last.y;
								t.Location.x = x;
								t.DirectionX = -signdx;
							}
							else {
								//new block is above old block
								t.Location.y = y;
								t.Location.x = last.x;
								t.DirectionX = signdx;
							}
							
							if(Mathf.Abs(y-last.y) < .99f) {							
								if(Mathf.Abs(x-last.x) < .99f) {
									//if we're doing a small slope
									t.Scale = new Vector2(0.5f,0.5f);
									t.Location.y += blockSize / 4f;
									t.Location.x += -t.DirectionX * blockSize / 4f;
									t.IsHalfSlope = true;
								}
								else {
									//if we're doing a quarter slope
									t.Location.y += blockSize / 2f;
									t.IsHalfSlope = false;
								}
							}
							else {
								t.IsHalfSlope = true;
							}

							tl.Add(t);
						}					
					}
					bl.Add(blockLocation);
				}
			}

			DeletePlaceModeBlocks();
			CreatePlacementModeBlocksAtLocations(bl, tl);
		}
		RedrawNeeded = false;
	}

	private void DeletePlaceModeBlocks() {
		//delete all existing blocks
		for (int i = BlocksInPlacement.Count - 1; i >= 0; i--)
		{
			Destroy(BlocksInPlacement[i].gameObject);
			BlocksInPlacement.RemoveAt(i);
		}

		for(int i = TrianglesInPlacement.Count - 1; i >= 0; i--) {
			Destroy(TrianglesInPlacement[i].gameObject);
			TrianglesInPlacement.RemoveAt(i);
		}
	}

	private void CreatePlacementModeBlocksAtLocations(List<Vector2> locations, List<TriangleInfo> triangles) {
		int ti = 0;
		for(int i = 0; i < locations.Count && i < selectedBlockCount; i++) {
			if (locations[i].y > Water.DrownLine && LevelBoundsGenerator.LevelBounds.Contains(locations[i])) {
				GameObject placementBlock = Instantiate(blockPrefab, locations[i], Quaternion.identity);
				placementBlock.transform.parent = blockParent.transform;
				Block b = placementBlock.AddComponent(_actualBlockType) as Block;
				b.PlacementModeInit();

				if(triangles != null && triangles.Count > ti && locations[i] == triangles[ti].ConnectingBlockLocation) {
					TriangleInfo tinfo = triangles[ti];
					Triangle placementTriangle = Instantiate(tinfo.IsHalfSlope ? halfTrianglePrefab : quarterTrianglePrefab,
															tinfo.Location,
															Quaternion.identity).GetComponent<Triangle>();
					placementTriangle.TriangleInit(tinfo, b, BlocksInPlacement.Last());
					placementTriangle.transform.parent = blockParent.transform;
					TrianglesInPlacement.Add(placementTriangle);
					ti++;
				}

				BlocksInPlacement.Add(b);
			}
		}
	}

	public void AddAvailableBlocks(BlockTypes type, int numBlocks) {
		if(BlocksAvailable.ContainsKey(type)) {
			BlocksAvailable[type] += numBlocks;
		}
		else {
			BlocksAvailable.Add(type, numBlocks);
			if (BlocksAvailable.Count == 1) {
				//if this was the first entry in the blocks available dictionary, set it to the selected type
				SetSelectedBlockType(type);
			}
		}
	}

	public void EndBlockPlaceMode() {
		InBlockPlaceMode = false;

		for(int i = BlocksInPlacement.Count-1; i >= 0; i--) {
			Block b = BlocksInPlacement[i];
			bool creationSuccess = b.CreateOrDestroyForPlacement();
			if(!creationSuccess) {
				BlocksInPlacement.RemoveAt(i);
			}
		}

		for (int i = TrianglesInPlacement.Count - 1; i >= 0; i--) {
			Triangle t = TrianglesInPlacement[i];
			bool creationSuccess = t.CreateOrDestroyForPlacement();
			if (!creationSuccess) {
				TrianglesInPlacement.RemoveAt(i);
			}
		}

		int numBlocksPlaced = BlocksInPlacement.Count;
		List<Block> pblocks = BlocksInPlacement.ToList();
		if(numBlocksPlaced > 0) {
			EventHandler<BlockPlacedEventInfo> handler = BlocksPlacedEventHandler;
			if (handler != null)
			{
				handler(this, new BlockPlacedEventInfo(numBlocksPlaced, SelectedBlockType, pblocks));
			}

			//perform animation for placing blocks
			StartCoroutine(BlockPlacedEffects(pblocks, TrianglesInPlacement.ToList()));

			//subtract from list of available blocks
			BlocksAvailable[SelectedBlockType] -= numBlocksPlaced;
			BlocksInPlacement.Clear();
			TrianglesInPlacement.Clear();
		}
	}

	public void BeginBlockPlaceMode() {
		MouseDownLocation = BlockCursor.Position;	
		if(BlocksAvailable.ContainsKey(SelectedBlockType)) {
			if(selectedBlockCount > 0) {
				InBlockPlaceMode = true;
				CreatePlacementModeBlocksAtLocations(new List<Vector2>() {MouseDownLocation}, null);
			}
		}
	}

	IEnumerator BlockPlacedEffects(List<Block> blocks, List<Triangle> triangles) {
		float startSpeed = blocks[0].WaitingToPlaceParticleSystem.main.startSpeed.constant;
		float endSpeed = startSpeed * 5;

		float startLifetime = blocks[0].WaitingToPlaceParticleSystem.main.startLifetime.constant;
		float endLifetime = startLifetime * 0.25f;

		float startEmission = blocks[0].WaitingToPlaceParticleSystem.emission.rateOverTime.constant;
		float endEmission = startEmission * 10;

		Color startColor = blocks[0].SpriteColor;
		Color startGlowColor = blocks[0].GlowColor;

		float startTime = Time.time;
		float lerpTime = 1.5f;

		foreach (Block b in blocks) {
			var main = b.WaitingToPlaceParticleSystem.main;
			main.startSpeed = endSpeed;

			var emission = b.WaitingToPlaceParticleSystem.emission;
			emission.rateOverTime = endEmission;
		}

		//lower intensity to 0
		//lower particle lifetime
		while (Time.time - startTime < lerpTime + Time.deltaTime) {
			float tdiff = (Time.time - startTime) / lerpTime;

			foreach (Block b in blocks) {
				b.SpriteColor = Color.Lerp(startColor, Color.white, tdiff);
				b.GlowColor = Color.Lerp(startGlowColor, Color.black, tdiff);

				var main = b.WaitingToPlaceParticleSystem.main;
				main.startLifetime = Mathf.Lerp(startLifetime, endLifetime, tdiff);
			}

			foreach(Triangle tr in triangles) {
				tr.spriteRenderer.color = Color.Lerp(startColor, Color.white, tdiff);
				tr.GlowColor = Color.Lerp(startGlowColor, Color.black, tdiff);
			}

			yield return new WaitForEndOfFrame();
		}

		//flash intensity high
		//set alpha to 1 for each block	
		foreach (Block b in blocks) {
			b.DuringPlaceParticleSystem.gameObject.SetActive(true);
		}

		yield return new WaitForSeconds(blocks[0].DuringPlaceParticleSystem.main.startLifetime.constant);

		//set intensity back to normal
		//set glowcolor to black for each block
		//remove particlerenderer for each block
		foreach (Block b in blocks) {
			b.PlacementModeEnd();
		}
		foreach(Triangle t in triangles) {
			t.PlacementModeEnd();
		}

		//disable outlinepress + cameraComposite
		if (!InBlockPlaceMode) {
			prepassCamera.gameObject.SetActive(false);
			compositeCamera.enabled = false;
		}
	}
}
