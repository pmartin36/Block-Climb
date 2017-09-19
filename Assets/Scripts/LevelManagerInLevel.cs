using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(BlockPlaceManager))]
public class LevelManagerInLevel : LevelManager, IBlockPlacedReceiver, IBlockGatheredReceiver {

	
	public LevelStats currentStats;
	public float StartTime { get; set; }
	public float GetCurrentTime {
		get {
			return Time.time - StartTime;
		}
	}
	public int LevelNumber { get; set; }
	public bool LevelComplete = false;

	public BlockPlaceManager BlockPlacer { get; set; }
	public UIBlockView BlocksUI { get; private set; }
	public Player Player { get; private set; }

	private OutlineComposite compositeCamera;
	private OutlinePrePass prepassCamera;

	protected override void Awake () {
		base.Awake();

		LevelNumber = SceneManager.GetActiveScene().buildIndex;	

		//set currentStats all to zero
		currentStats = new LevelStats(1);

		//check if there is an entry in the master level list in globalmanager
		//if there is, populate currentstats AvailableBlocks 
		//if there isn't, create an entry in the master level list
		if(GlobalManager.Instance.LevelCompletionStats.Count > LevelNumber) {
			int blocksavailable = GlobalManager.Instance.LevelCompletionStats[LevelNumber]["Captain"].BlocksCollected.Available;
			currentStats.BlocksCollected.Available = blocksavailable;
		}
		else {
			int blocksavailable = GameObject.FindObjectsOfType<CollectableBlock>().Length;
			currentStats.BlocksCollected.Available = blocksavailable;
			//set master level list
			Dictionary<string, LevelStats> entry = new Dictionary<string, LevelStats>();

			//set captain for this level
			entry.Add("Captain", new LevelStats() {
				Time = 1,
				BlocksUsed = 1,
				BlocksCollected = new CollectedBlockStats(100, 100)
			});		

			//set firstmate
			entry.Add("FirstMate", new LevelStats()
			{
				Time = 5,
				BlocksUsed = 5,
				BlocksCollected = new CollectedBlockStats(90, 100)
			});

			//set quartermaster
			entry.Add("Quartermaster", new LevelStats()
			{
				Time = 10,
				BlocksUsed = 10,
				BlocksCollected = new CollectedBlockStats(80, 100)
			});

			//set cook
			entry.Add("Cook", new LevelStats()
			{
				Time = 20,
				BlocksUsed = 20,
				BlocksCollected = new CollectedBlockStats(70, 100)
			});

			//set pb
			entry.Add("PB", new LevelStats() {
				Time = int.MaxValue,
				BlocksUsed = int.MaxValue,
				BlocksCollected = new CollectedBlockStats(int.MinValue, blocksavailable)
			});

			//add the global list
			GlobalManager.Instance.LevelCompletionStats.Add(entry);
		} 
	}

	public void Start() {
		StartTime = Time.time;

		//Get Handle to Camera Effect Classes
		compositeCamera = Camera.main.GetComponent<OutlineComposite>();
		prepassCamera = Camera.main.GetComponentInChildren<OutlinePrePass>();

		//Fade in for beginning of level
		StartCoroutine(FadeViewIn());

		//Get Handle to BlockPlacer
		BlockPlacer = GetComponent<BlockPlaceManager>();

		BlockPlacer.BlocksPlacedEventHandler += BlocksUI.c_newBlockPlacedEvent;
		BlockPlacer.BlocksPlacedEventHandler += this.c_newBlockPlacedEvent;

		//set BlockPlacer Starting Block Counts
		BlockPlacer.AddAvailableBlocks(BlockTypes.Clay, 5);
		BlockPlacer.AddAvailableBlocks(BlockTypes.Dirt, 5);
		BlockPlacer.AddAvailableBlocks(BlockTypes.Steel, 5);
		BlockPlacer.AddAvailableBlocks(BlockTypes.Clay, 20);
		BlockPlacer.AddAvailableBlocks(BlockTypes.Cloud, 1);

		//set Block UI Starting Block Counts
		//these ones are for testing...make sure to remove
		BlocksUI.AddBlocks(BlockTypes.Clay, 5);
		BlocksUI.AddBlocks(BlockTypes.Dirt, 5);
		BlocksUI.AddBlocks(BlockTypes.Steel, 5);
		BlocksUI.AddBlocks(BlockTypes.Clay, 20);
		BlocksUI.AddBlocks(BlockTypes.Cloud, 1);
	}

	protected override void Update () {
		if(!LevelComplete) {
			currentStats.Time = (int)GetCurrentTime;
		}
		base.Update();
	}

	public void RegisterBlocksAvailableUI(UIBlockView ui) {
		BlocksUI = ui;
	}

	public void RegisterPlayer(Player player){
		Player = player;

		//pass player to Camera follow script
		Camera.main.GetComponent<CameraFollow>().InitFocusArea(player, new Vector2(5,5));

		//get handler for block pickups
		player.Controller.BlockGatheredEventHandler += c_newBlockGatheredEvent;
	}

	public override void ProcessPlayingInputs(float horizontal, float vertical, float switchblock, bool placeblock, bool jump, Vector3 courseAdjust, Vector3 fineAdjust, Vector3 mousePosition)
	{
		if (switchblock >= 0.1f)
		{
			BlocksUI.RotateBlocksRight();

			//set active block type in block place manager to match what's showing in the UI
			BlockTypes active = BlocksUI.ActiveBlockType();
			BlockPlacer.SetSelectedBlockType(active);
		}
		else if (switchblock <= -0.1f)
		{
			BlocksUI.RotateBlocksLeft();

			//set active block type in block place manager to match what's showing in the UI
			BlockTypes active = BlocksUI.ActiveBlockType();
			BlockPlacer.SetSelectedBlockType(active);
		}

		//z = 0 when that input type is being used
		if(mousePosition.z == 0){
			Vector2 world = Camera.main.ScreenToWorldPoint(mousePosition);
			BlockPlacer.SetBlockCursorPosition(world);
		}
		else {
			if (courseAdjust.z == 0) {

			}
			else if (fineAdjust.z == 0) {

			}
		}

		if(placeblock && !BlockPlacer.InBlockPlaceMode) {
			compositeCamera.enabled = true;
			prepassCamera.gameObject.SetActive(true);
			BlockPlacer.BeginBlockPlaceMode();
		}
		else if(!placeblock && BlockPlacer.InBlockPlaceMode) {
			BlockPlacer.EndBlockPlaceMode();
		}

		//player movement
		Player.InputDirection = new Vector2(horizontal, vertical);
		Player.ProcessJumpInput(jump);
	}

	public override void ProcessPauseInputs(float horizontal, float vertical, bool menuconfirm, bool menuback)
	{
		base.ProcessPauseInputs(horizontal, vertical, menuconfirm, menuback);
	}

	public override void ResetLevel()
	{
		SceneManager.LoadScene(LevelNumber);
	}

	public override void AdvanceLevel()
	{

	}

	public override void QuitLevel()
	{

	}

	public void CompleteLevel() {
		//set new PBs
		LevelStats pb;
		LevelStats comparer = new LevelStats();

		//set the global PB to be the new PB if better
		pb = GlobalManager.Instance.LevelCompletionStats[LevelNumber]["PB"];
		pb = LevelStats.GetLowest(pb, currentStats, ref comparer);

		//play animation for improved pb categories
		if(comparer.Time > 0) {

		}
		if(comparer.BlocksCollected.Collected > 0) {

		}
		if(comparer.BlocksUsed > 0) {

		}

		//check the rankings of the new PB
		LevelRanking pbRanking = pb.GetLevelRanking(GlobalManager.Instance.LevelCompletionStats[LevelNumber]);

		//display animations for rankings

		//Save Game
		GlobalManager.Instance.SaveGame();
	}

	public void c_newBlockPlacedEvent(object sender, BlockPlacedEventInfo e)
	{
		currentStats.BlocksUsed += e.NumPlaced;
	}

	public void c_newBlockGatheredEvent(object sender, BlockGatheredEventInfo e) {
		//update UI
		BlocksUI.AddBlocks(e.Type, e.Count);

		//Update Block Placement Manager
		BlockPlacer.AddAvailableBlocks(e.Type, e.Count);

		//Update LevelStats
		currentStats.BlocksCollected += e.Count;

		//Verify UI and Block Placement Manager are in sync??

	}

	private IEnumerator FadeViewIn() {
		float startTime = Time.time;
		float fadeTime = 2f;
		while (Time.time - startTime < fadeTime + Time.deltaTime) {
			Shader.SetGlobalColor("_ColorAdd", Color.Lerp(Color.black, Color.white, (Time.time - startTime) / fadeTime));
			yield return new WaitForEndOfFrame();
		}
	}
}
