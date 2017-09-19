using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class BlockSteel : Block
{
	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();
		spriteRender.sprite = Block.GetBlockSprite(BlockTypes.Steel);
	}

	// Update is called once per frame
	protected override void Update()
	{

	}

	public override void PlacementModeInit()
	{
		base.PlacementModeInit();
	}

	public override void PlacementModeEnd()
	{
		base.PlacementModeEnd();
	}
}

