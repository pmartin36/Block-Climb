using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class GlobalManager : Singleton<GlobalManager> {

	public List<LevelRanking> CompletedLevelRankings;
	public int CompletedLevels {
		get {
			return CompletedLevelRankings.Count;
		}
	}
	public LevelManager LevelManager { get; set; }
	public List<Dictionary<string, LevelStats>> LevelCompletionStats;

	void Awake(){
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		//check serialized save file
		//if it doesn't exist, this is players first time launching so set everything to default
		LevelCompletionStats = Serializer<List<Dictionary<string, LevelStats>>>.Deserialize("LevelStats.bin");
		if(LevelCompletionStats == null) {
			LevelCompletionStats = new List<Dictionary<string, LevelStats>>();

			//For Testing Only
			GenerateTestCompletionStats();
		}
	}

	public override void OnDestroy() {	
		base.OnDestroy();
	}

	public void SaveGame() {
		Serializer<List<Dictionary<string, LevelStats>>>.Serialize(LevelCompletionStats, "LevelStats.bin");
	}

	public void GenerateTestCompletionStats() {
		Dictionary<string, LevelStats> statDictionary = new Dictionary<string, LevelStats>();
		statDictionary.Add("Captain", new LevelStats(1, 100, 1) { BlocksCollected = new CollectedBlockStats(100, 100) });
		statDictionary.Add("FirstMate", new LevelStats(5, 100, 5) { BlocksCollected = new CollectedBlockStats(90, 100) });
		statDictionary.Add("Quartermaster", new LevelStats(15, 100, 15) { BlocksCollected = new CollectedBlockStats(80, 100) });
		statDictionary.Add("Cook", new LevelStats(20, 100, 20) { BlocksCollected = new CollectedBlockStats(70, 100) });

		Dictionary<string, LevelStats> statDictionary2 = new Dictionary<string, LevelStats>();
		statDictionary2.Add("Captain", new LevelStats(10, 100, 15) { BlocksCollected = new CollectedBlockStats(120, 120) });
		statDictionary2.Add("FirstMate", new LevelStats(25, 100, 30) { BlocksCollected = new CollectedBlockStats(90, 120) });
		statDictionary2.Add("Quartermaster", new LevelStats(45, 100, 55) { BlocksCollected = new CollectedBlockStats(60, 120) });
		statDictionary2.Add("Cook", new LevelStats(60, 100, 60) { BlocksCollected = new CollectedBlockStats(30, 120) });

		//Test Serialization
		Serializer<List<Dictionary<string, LevelStats>>>.Serialize(new List<Dictionary<string, LevelStats>> { statDictionary, statDictionary2 }, "LevelStats.bin");
	}
}
