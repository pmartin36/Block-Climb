using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class LevelStats : ICloneable
{
	public CollectedBlockStats BlocksCollected { get; set; }
	public int BlocksUsed { get; set; }
	public int Time { get; set; }

	//Used only for the ref block in GetLowest
	public LevelStats() {
		BlocksCollected = new CollectedBlockStats();
		Time = -1;
		BlocksUsed = -1;
	}
	//normal constructor
	public LevelStats(int levelBlockCount) {
		BlocksCollected = new CollectedBlockStats(0,levelBlockCount);
		Time = 0;
		BlocksUsed = 0;
	}
	//may not be used?
	public LevelStats(int time, int levelBlockCount, int used) {
		BlocksCollected = new CollectedBlockStats(0, levelBlockCount);
		Time = time;
		BlocksUsed = used;
	}

	/// <summary>
	/// Compares completion time, blocked used, and blocks collected for l1 and l2
	/// </summary>
	/// <param name="l1"></param>
	/// <param name="l2"></param>
	/// <param name="compareResult">+1 is returned for each parameter that l2 was better</param>
	/// <returns></returns>
	public static LevelStats GetLowest(LevelStats l1, LevelStats l2, ref LevelStats compareResult) {
		LevelStats lsr = l1.Clone() as LevelStats;

		//if l2 was completed faster than l1, make l2 new lowest
		if(l2.Time < lsr.Time) {
			lsr.Time = l2.Time;
			compareResult.Time = 1;
		}
		//if l2 used less blocks than l1, make l2 new lowest
		if(l2.BlocksUsed < lsr.BlocksUsed) {
			lsr.BlocksUsed = l2.BlocksUsed;
			compareResult.BlocksUsed = 1;
		}
		//if l2 collected more blocks than l1, make l2 new highest
		if(l2.BlocksCollected.Collected > lsr.BlocksCollected.Collected) {
			lsr.BlocksCollected.Collected = l2.BlocksCollected.Collected;
			compareResult.BlocksCollected.Collected = 1;
		}

		return lsr;
	}

	public LevelRanking GetLevelRanking (Dictionary<string,LevelStats> rankings) {
		LevelRanking lr = new LevelRanking();

		LevelStats captain = rankings["Captain"];
		LevelStats firstmate = rankings["FirstMate"];
		LevelStats quartermaster = rankings["Quartermaster"];

		//check ranking for completion time
		if (this.Time <= captain.Time) {
			lr.CompletionTime = Rank.Captain;
		}
		else if(this.Time < firstmate.Time) {
			lr.CompletionTime = Rank.FirstMate;
		}
		else if(this.Time < quartermaster.Time) {
			lr.CompletionTime = Rank.Quartermaster;
		}

		//check ranking for blocked used
		if (this.BlocksUsed <= captain.BlocksUsed)
		{
			lr.BlocksUsed = Rank.Captain;
		}
		else if (this.BlocksUsed < firstmate.BlocksUsed)
		{
			lr.BlocksUsed = Rank.FirstMate;
		}
		else if (this.BlocksUsed < quartermaster.BlocksUsed)
		{
			lr.BlocksUsed = Rank.Quartermaster;
		}

		//check ranking for blocked collected
		if (this.BlocksCollected.Collected >= captain.BlocksCollected.Collected)
		{
			lr.BlocksCollected = Rank.Captain;
		}
		else if (this.BlocksCollected.Collected > firstmate.BlocksCollected.Collected)
		{
			lr.BlocksCollected = Rank.FirstMate;
		}
		else if (this.BlocksCollected.Collected > quartermaster.BlocksCollected.Collected)
		{
			lr.BlocksCollected = Rank.Quartermaster;
		}

		return lr;
	}

	public override string ToString()
	{
		return string.Format("Time: {0}, Used: {1}, Collected {2}",Time,BlocksUsed,BlocksCollected);
	}

	public object Clone()
	{
		return new LevelStats() {
			BlocksCollected = this.BlocksCollected.Clone() as CollectedBlockStats,
			Time = this.Time,
			BlocksUsed = this.BlocksUsed
		};
	}
}
