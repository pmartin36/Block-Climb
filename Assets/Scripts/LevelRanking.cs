using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum Rank
{
	Captain,
	FirstMate,
	Quartermaster,
	Cook
}

public class LevelRanking
{
	public Rank BlocksUsed { get; set; }
	public Rank CompletionTime { get; set; }
	public Rank BlocksCollected { get; set; }

	public LevelRanking(Rank blocksUsed, Rank completionTime, Rank blocksCollected)
	{
		BlocksUsed = blocksUsed;
		CompletionTime = completionTime;
		BlocksCollected = blocksCollected;
	}

	public LevelRanking() : this(Rank.Cook, Rank.Cook, Rank.Cook){
	}
}