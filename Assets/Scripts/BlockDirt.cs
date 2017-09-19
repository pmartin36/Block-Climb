using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class BlockDirt : Block
{
	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();
		spriteRender.sprite = Block.GetBlockSprite(BlockTypes.Dirt);
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

