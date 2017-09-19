using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BlockPlacedEventInfo : EventArgs
{
	public int NumPlaced {get; private set; }
	public BlockTypes TypePlaced {get; private set; }
	public List<Block> BlocksPlaced {get; private set; }

	public BlockPlacedEventInfo(int numPlaced, BlockTypes type, List<Block> blocks) {
		NumPlaced = numPlaced;
		TypePlaced = type;
		BlocksPlaced = blocks;
	}
}

