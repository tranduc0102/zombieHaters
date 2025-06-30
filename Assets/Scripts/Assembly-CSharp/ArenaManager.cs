using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
	[SerializeField]
	private TextAsset csvData;

	[SerializeField]
	private int arenaStages = 1;

	public int winRating = 25;

	public int loseRating = 15;

	public int bonusPercentage = 50;

	private List<ArenaInfo> arenaInfosVariable;

	private List<StagedArenaInfo> testArInfoVar;

	private int currentArena;

	private float currentBonus;

	[HideInInspector]
	public int arenaSwitched;

	[HideInInspector]
	public int ratingBeforeGame;

	public static ArenaManager instance { get; private set; }

	private List<StagedArenaInfo> StagedArenaInfos
	{
		get
		{
			if (arenaInfosVariable == null)
			{
				TestInitArenaInfos();
			}
			return testArInfoVar;
		}
		set
		{
			testArInfoVar = value;
		}
	}

	public ArenaInfo currentArenaInfo { get; private set; }

	public float GetCurrentBonus
	{
		get
		{
			return currentBonus + 1f;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		TestInitArenaInfos();
	}

	private void Start()
	{
		UpdateAll();
	}

	public void UpdateAll()
	{
		currentArena = CalculateCurrentArena();
		currentBonus = 0f;
		currentArenaInfo = GetCurrentArenaInfo();
		if (DataLoader.initialized)
		{
			DataLoader.gui.videoMultiplier.UpdatePercentage();
		}
	}

	private void TestInitArenaInfos()
	{
		string[] array = CsvLoader.SplitLines(csvData);
		testArInfoVar = new List<StagedArenaInfo>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			int arenaIndex = int.Parse(array2[5]) - 1;
			ArenaInfo arenaInfo = new ArenaInfo();
			arenaInfo.rating = int.Parse(array2[0]);
			arenaInfo.power = float.Parse(array2[1]);
			arenaInfo.turretDamage = float.Parse(array2[2]);
			arenaInfo.waveNumber = int.Parse(array2[3]);
			arenaInfo.gemsReward = int.Parse(array2[4]);
			if (testArInfoVar.Any((StagedArenaInfo item) => item.arenaIndex == arenaIndex))
			{
				testArInfoVar.First((StagedArenaInfo item) => item.arenaIndex == arenaIndex).stages.Add(arenaInfo);
			}
			else
			{
				testArInfoVar.Add(new StagedArenaInfo(arenaIndex, arenaInfo));
			}
		}
	}

	private int CalculateCurrentArena()
	{
		foreach (StagedArenaInfo stagedArenaInfo in StagedArenaInfos)
		{
			if (DataLoader.playerData.arenaRating <= stagedArenaInfo.stages.Last().rating)
			{
				return stagedArenaInfo.arenaIndex;
			}
		}
		return GetmaxArenaIndex();
	}

	private float CalculateCurrentArenaBonus()
	{
		return (float)(bonusPercentage * currentArena) / 100f;
	}

	public float GetCurrentStagePower()
	{
		return currentArenaInfo.power;
	}

	public ArenaInfo GetCurrentArenaInfo()
	{
		for (int i = 0; i < StagedArenaInfos.Count; i++)
		{
			if (DataLoader.playerData.arenaRating > StagedArenaInfos[i].stages.Last().rating)
			{
				continue;
			}
			for (int j = 0; j < StagedArenaInfos[i].stages.Count; j++)
			{
				if (DataLoader.playerData.arenaRating <= StagedArenaInfos[i].stages[j].rating)
				{
					if (j == 0)
					{
						return StagedArenaInfos[i].stages[j];
					}
					return StagedArenaInfos[i].stages[j - 1];
				}
			}
		}
		return StagedArenaInfos.Last().stages.Last();
	}

	public StagedArenaInfo GetStagedArenaInfoByIndex(int index)
	{
		return StagedArenaInfos.First((StagedArenaInfo item) => item.arenaIndex == index);
	}

	public void SaveArenaRating(int rating)
	{
		DataLoader.playerData.arenaRating += rating;
		NormalizeRating();
		CheckArenaSwitched();
		DataLoader.Instance.SavePlayerData();
	}

	public void StartArenaGame()
	{
		ratingBeforeGame = DataLoader.playerData.arenaRating;
		SaveArenaRating(-loseRating);
		NormalizeRating();
		DataLoader.Instance.SavePlayerData();
	}

	public int GetAddedWinRating()
	{
		if (ratingBeforeGame < loseRating)
		{
			return ratingBeforeGame;
		}
		return loseRating;
	}

	public int GetmaxArenaIndex()
	{
		return StagedArenaInfos.Last().arenaIndex;
	}

	public int GetMaxRating()
	{
		return StagedArenaInfos.Last().stages.Last().rating;
	}

	private void CheckArenaSwitched()
	{
		int num = DataLoader.playerData.arenaRating - ratingBeforeGame;
		arenaSwitched = 0;
		if (num != 0)
		{
			int arenaIndexByRating = GetArenaIndexByRating(ratingBeforeGame);
			int arenaIndexByRating2 = GetArenaIndexByRating(DataLoader.playerData.arenaRating);
			if (arenaIndexByRating > arenaIndexByRating2)
			{
				arenaSwitched = -1;
			}
			else if (arenaIndexByRating2 > arenaIndexByRating)
			{
				arenaSwitched = 1;
			}
		}
	}

	public int GetArenaIndexByRating(int rating)
	{
		for (int i = 1; i < StagedArenaInfos.Count; i++)
		{
			if (rating < StagedArenaInfos[i].stages.First().rating)
			{
				return StagedArenaInfos[i - 1].arenaIndex;
			}
		}
		return StagedArenaInfos.Last().arenaIndex;
	}

	private void NormalizeRating()
	{
		if (DataLoader.playerData.arenaRating > GetMaxRating())
		{
			DataLoader.playerData.arenaRating = GetMaxRating();
		}
		else if (DataLoader.playerData.arenaRating < 0)
		{
			DataLoader.playerData.arenaRating = 0;
		}
	}
}
