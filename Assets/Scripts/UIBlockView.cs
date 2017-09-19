using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBlockView : MonoBehaviour, IBlockPlacedReceiver {

	GameObject uiBlockPrefab;
	List<UIBlock> Blocks;

	void Awake() {
		Blocks = new List<UIBlock>();
		uiBlockPrefab = Resources.Load<GameObject>("Prefabs/UIBlock");

		(GlobalManager.Instance.LevelManager as LevelManagerInLevel).RegisterBlocksAvailableUI(this);
		
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {	
		if(Blocks.First().PositionQueue.Count > 0 && 
			Blocks.Count( p => !p.Moving ) == Blocks.Count) {
			//if blocks need to move and all are stopped
			//if one block needs to move, they all need to move..that's why blocks.first is used
			foreach (UIBlock b in Blocks) {
				b.MoveBlock();
			}
		}
	}

	public void RotateBlocksLeft() {
		foreach(UIBlock b in Blocks) {
			b.RotateLeft(Blocks.Count);
		}
	}

	public void RotateBlocksRight() {
		foreach (UIBlock b in Blocks)
		{
			b.RotateRight(Blocks.Count);
		}
	}

	public BlockTypes ActiveBlockType() {
		//get the block that is at the bottom of the UI (the active block type)
		return Blocks.First(b => b.ActiveBlock).BlockType;
	} 

	public void AddBlocks(BlockTypes type, int count) {
		UIBlock b = Blocks.FirstOrDefault( u => u.BlockType == type);
		//does a block of this type already exist in the list
		if(b != null) {
			b.Count += count;
		}
		else {
			//if not, create a new block and initialize it
			GameObject o = Instantiate(uiBlockPrefab, this.transform);
			UIBlock u = o.GetComponent<UIBlock>();
			UIBlockPositions p;
			switch (Blocks.Count)
			{
				case 0:
					p = UIBlockPositions.Bottom;
					break;
				case 1:
					p = UIBlockPositions.Left;
					break;
				case 2:
					p = UIBlockPositions.Right;
					break;
				default:
				case 3:
					p = UIBlockPositions.Top;
					break;
			}
			u.Init(type, p, count);
			Blocks.Add(u);
		}
	}

	public void c_newBlockPlacedEvent(object sender, BlockPlacedEventInfo e)
	{
		//subtract blocks
		UIBlock b = Blocks.FirstOrDefault(u => u.BlockType == e.TypePlaced);
		if (b != null)
		{
			b.Count -= e.NumPlaced;
		}
	}
}
