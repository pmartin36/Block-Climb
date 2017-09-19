using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BlockGatheredEventInfo : EventArgs{
	public BlockTypes Type { get; set; }
	public int Count { get; set; }

	public BlockGatheredEventInfo(BlockTypes type, int count) {
		Type = type;
		Count = count;
	}
}
