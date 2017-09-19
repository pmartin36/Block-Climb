using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsTest : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		

		LevelStats pb = new LevelStats(12, 100, 30);
		pb.BlocksCollected.Collected = 90;

		LevelStats newpb = pb.Clone() as LevelStats;

		newpb.Time = pb.Time - 1;
		FasterTime(pb, newpb);

		newpb.Time = pb.Time;
		newpb.BlocksUsed = pb.BlocksUsed - 1;
		LessUsed(pb, newpb);

		newpb.BlocksUsed = pb.BlocksUsed;
		newpb.BlocksCollected.Collected = pb.BlocksCollected.Collected + 1;
		MoreCollected(pb, newpb);

		newpb.Time = pb.Time - 1;
		MoreCollectedLessTime(pb, newpb);

		GlobalManager.Instance.GenerateTestCompletionStats();
		List<Dictionary<string, LevelStats>> ds = Serializer < List < Dictionary < string, LevelStats>>>.Deserialize("LevelStats.bin");

		var statDictionary = GlobalManager.Instance.LevelCompletionStats[0];
		WriteDictionaryRanks(statDictionary);
		GetRankings(newpb, statDictionary);

		newpb.Time = 1;
		newpb.BlocksCollected.Collected = 100;
		newpb.BlocksUsed = 1;

		GetRankings(newpb, statDictionary);

		//LevelRanking pbRanking = pb.GetLevelRanking(GlobalManager.Instance.LevelCompletionStats[LevelNumber]);
	}

	public void FasterTime(LevelStats l1, LevelStats l2) {
		Debug.Log("l2 faster than l1");

		LevelStats comparer = new LevelStats();
		LevelStats newpb = LevelStats.GetLowest(l1, l2, ref comparer);

		if(comparer.Time > 0 && comparer.BlocksUsed < 0 && comparer.BlocksCollected.Collected < 0) {
			Debug.Log("Comparer results correct");
		} 
		else {
			Debug.Log("Comparer results correct");
		}

		if(newpb.Time == l2.Time && newpb.BlocksUsed == l1.BlocksUsed && newpb.BlocksCollected.Collected == l1.BlocksCollected.Collected) {
			Debug.Log("Properties match");
		}
		else {
			Debug.Log("Property mismatch");
		}
	}

	public void LessUsed(LevelStats l1, LevelStats l2)
	{
		Debug.Log("l2 used less blocks than l1");

		LevelStats comparer = new LevelStats();
		LevelStats newpb = LevelStats.GetLowest(l1, l2, ref comparer);

		if (comparer.Time < 0 && comparer.BlocksUsed > 0 && comparer.BlocksCollected.Collected < 0)
		{
			Debug.Log("Comparer results correct");
		}
		else
		{
			Debug.Log("Comparer results correct");
		}

		if (newpb.Time == l1.Time && newpb.BlocksUsed == l2.BlocksUsed && newpb.BlocksCollected.Collected == l1.BlocksCollected.Collected)
		{
			Debug.Log("Properties match");
		}
		else
		{
			Debug.Log("Property mismatch");
		}
	}

	public void MoreCollected(LevelStats l1, LevelStats l2)
	{
		Debug.Log("l2 collected more blocks than l1");

		LevelStats comparer = new LevelStats();
		LevelStats newpb = LevelStats.GetLowest(l1, l2, ref comparer);

		if (comparer.Time < 0 && comparer.BlocksUsed < 0 && comparer.BlocksCollected.Collected > 0)
		{
			Debug.Log("Comparer results correct");
		}
		else
		{
			Debug.Log("Comparer results correct");
		}

		if (newpb.Time == l1.Time && newpb.BlocksUsed == l1.BlocksUsed && newpb.BlocksCollected.Collected == l2.BlocksCollected.Collected)
		{
			Debug.Log("Properties match");
		}
		else
		{
			Debug.Log("Property mismatch");
		}
	}

	public void MoreCollectedLessTime(LevelStats l1, LevelStats l2)
	{
		Debug.Log("l2 collected more blocks than l1 AND was faster");

		LevelStats comparer = new LevelStats();
		LevelStats newpb = LevelStats.GetLowest(l1, l2, ref comparer);

		if (comparer.Time > 0 && comparer.BlocksUsed < 0 && comparer.BlocksCollected.Collected > 0)
		{
			Debug.Log("Comparer results correct");
		}
		else
		{
			Debug.Log("Comparer results correct");
		}

		if (newpb.Time == l2.Time && newpb.BlocksUsed == l1.BlocksUsed && newpb.BlocksCollected.Collected == l2.BlocksCollected.Collected)
		{
			Debug.Log("Properties match");
		}
		else
		{
			Debug.Log("Property mismatch");
		}
	}

	public void WriteDictionaryRanks(Dictionary<string, LevelStats> dic) {
		Debug.Log("Captain: " + dic["Captain"]);
		Debug.Log("FirstMate: " + dic["FirstMate"]);
		Debug.Log("Quartermaster: " + dic["Quartermaster"]);
		Debug.Log("Cook: " + dic["Cook"]);
	}

	public void GetRankings( LevelStats l1, Dictionary<string, LevelStats> dic ) {
		LevelRanking pbRanking = l1.GetLevelRanking(dic);
		Debug.Log(l1);
		Debug.Log(string.Format("Time: {0}, Blocks Used: {1}, Blocks Collected: {2}",pbRanking.CompletionTime, pbRanking.BlocksUsed, pbRanking.BlocksCollected));
	}
}
