using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
	[Serializable]
	public struct SFWNBL
	{
		public int[] startFromWaveNumberByLevel;
	}

	[Serializable]
	public class Boss
	{
		public BossZombie prefabBoss;

		public int pointsToGo;

		public int reward;

		public int gemsReward;
	}

	[Serializable]
	public struct Bosses
	{
		public Boss[] bosses;

		public List<int> startPointsToGoByLevel;

		public long allBossesReward;
	}

	[Serializable]
	public class ZombieVariants
	{
		public List<ZombieHuman> variants;

		public int minWave;

		public bool readyInIdle = true;

		public ZombieHuman GetVariant()
		{
			return variants[UnityEngine.Random.Range(0, variants.Count)];
		}
	}

	[Serializable]
	public class ZombieRank
	{
		public List<ZombieVariants> zombies;

		public List<int> chances;

		public ZombieHuman GetZombie(float powerAvailable)
		{
			if (zombies.Count <= 0)
			{
				return null;
			}
			int balanceNum = GetBalanceNum();
			ZombieHuman variant = zombies[balanceNum].GetVariant();
			while (zombies.Count > 0 && ((GameManager.instance.currentGameMode == GameManager.GameModes.Idle && !zombies[balanceNum].readyInIdle) || (GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay && inGameWavesActive && zombies[balanceNum].minWave > instance.currentWave) || (GameManager.instance.currentGameMode == GameManager.GameModes.Arena && zombies[balanceNum].minWave > ArenaWavesManager.instance.zombiesLimiterWaveNumber) || (GameManager.instance.currentGameMode == GameManager.GameModes.PVP && zombies[balanceNum].minWave > PVPManager.Instance.zombiesLimiterWaveNumber) || variant.power > powerAvailable))
			{
				zombies.RemoveAt(balanceNum);
				chances.RemoveAt(balanceNum);
				if (zombies.Count > 0)
				{
					balanceNum = GetBalanceNum();
					variant = zombies[balanceNum].GetVariant();
					continue;
				}
				break;
			}
			if (zombies.Count <= 0)
			{
				return null;
			}
			return variant;
		}

		private int GetBalanceNum()
		{
			int num = 0;
			foreach (int chance in chances)
			{
				num += chance;
			}
			int num2 = UnityEngine.Random.Range(0, num);
			int num3 = 0;
			for (int i = 0; i < chances.Count; i++)
			{
				num3 += chances[i];
				if (num2 < num3)
				{
					return i;
				}
			}
			return 0;
		}
	}

	public static WavesManager instance;

	/*
	[HideInInspector]
	*/
	public List<ZombiesSpawn> zombiesSpawns = new List<ZombiesSpawn>();

	public List<ZombieRank> zombies;

	public List<int> chances;

	[SerializeField]
	public int maxCountZombies = 40;

	[SerializeField]
	private float[] spawnDelayByLevel;

	[SerializeField]
	private float[] firstWavePowerByLevel;

	[SerializeField]
	private SFWNBL[] startFromWaveNumberByWorld;

	private float firstWavePower = 10f;

	[SerializeField]
	private float scale1 = 1f;

	[SerializeField]
	private float scale2 = 1f;

	[SerializeField]
	private Transform cameraTarget;

	[SerializeField]
	private float staticSpawnDelay = 5f;

	[Space]
	[SerializeField]
	public Bosses[] bossesByWorld;

	[SerializeField]
	private float bossDelayMultiplier = 2f;

	[Space]
	[SerializeField]
	private Transform dailyBossSpawnPoint;

	[Space]
	public float spawnSmooth = 0.1f;

	private int bossesSpawned;

	private float bossWavesDelayMultiplier = 1f;

	private int bossDeadAtPoints;

	[HideInInspector]
	public bool bossInDaHause;

	private float minDistance = 20f;

	private static bool inGameWavesActive;

	private int addedPoints;

	[Space]
	public int currentWave = -1;

	[HideInInspector]
	public float maxCurrentWavePower { get; private set; }

	private void Awake()
	{
		inGameWavesActive = false;
		instance = this;
	}

	private void Start()
	{
		SetBossesByWorldData();
	}

	public void StartIdle()
	{
		if (!GameManager.instance.isTutorialNow)
		{
			Invoke("SpawnStaticWave", 1f);
		}
	}

	public void SpawnStaticWave()
	{
		maxCurrentWavePower = 0f;
		foreach (SurvivorHuman survivor in GameManager.instance.survivors)
		{
			maxCurrentWavePower += DataLoader.Instance.GetHeroPower(survivor.heroType);
		}
		maxCurrentWavePower *= UnityEngine.Random.Range(0.35f, 0.4f) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus) * (1f + PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus) * (1f + PassiveAbilitiesManager.bonusHelper.GetCriticalHitChance());
		if (GameManager.instance.startPoint != null && GameManager.instance.startPoint.idleZombieSpawns.Length > 0)
		{
			GameManager.instance.startPoint.idleZombieSpawns[UnityEngine.Random.Range(0, GameManager.instance.startPoint.idleZombieSpawns.Length)].Spawn();
			Invoke("SpawnStaticWave", staticSpawnDelay);
		}
	}

	public void StartGame()
	{
		CancelInvoke("SpawnStaticWave");
		if (GameManager.instance.currentWorldNumber - 1 < startFromWaveNumberByWorld.Length)
		{
			firstWavePower = firstWavePowerByLevel[DataLoader.Instance.GetCurrentPlayerLevel()];
			currentWave = startFromWaveNumberByWorld[GameManager.instance.currentWorldNumber - 1].startFromWaveNumberByLevel[DataLoader.Instance.GetCurrentPlayerLevel()] - 1;
			maxCurrentWavePower = firstWavePower;
			Invoke("SpawnGroup", 1f);
			bossesSpawned = 0;
			addedPoints = 0;
			TryToSkipNextBoss();
			bossWavesDelayMultiplier = 1f;
			bossDeadAtPoints = 0;
			bossInDaHause = false;
			DataLoader.gui.SetTopPanelAnimationState(bossInDaHause);
		}
		inGameWavesActive = true;
	}

	private void SpawnGroup()
	{
		if (zombiesSpawns.Count > 0)
		{
			currentWave++;
			maxCurrentWavePower = firstWavePower * Mathf.Pow(scale1, currentWave) + scale2 * (float)currentWave;
			maxCurrentWavePower *= UnityEngine.Random.Range(1f, 2f);
			FindBestSpawnAndDoIt();
			Invoke("SpawnGroup", spawnDelayByLevel[DataLoader.Instance.GetCurrentPlayerLevel()] * bossWavesDelayMultiplier);
		}
	}

	private void FindBestSpawnAndDoIt(bool onlyBoss = false)
	{
		float[] array = new float[zombiesSpawns.Count];
		for (int i = 0; i < zombiesSpawns.Count; i++)
		{
			array[i] = Vector3.Distance(cameraTarget.position, zombiesSpawns[i].transform.position);
		}
		for (int j = 0; j < array.Length; j++)
		{
			for (int k = j; k < array.Length; k++)
			{
				if (array[k] < array[j])
				{
					float num = array[k];
					array[k] = array[j];
					array[j] = num;
					ZombiesSpawn value = zombiesSpawns[k];
					zombiesSpawns[k] = zombiesSpawns[j];
					zombiesSpawns[j] = value;
				}
			}
		}
		for (int l = 0; l < array.Length - 2; l++)
		{
			if (!zombiesSpawns[l].isIdleSpawn && array[l] > minDistance && zombiesSpawns[l].openAtLevel <= DataLoader.Instance.GetCurrentPlayerLevel() && zombiesSpawns[l].worldNumber == GameManager.instance.currentWorldNumber)
			{
				int index;
				do
				{
					index = l + UnityEngine.Random.Range(0, 3);
				}
				while (zombiesSpawns[index].isIdleSpawn || zombiesSpawns[index].openAtLevel > DataLoader.Instance.GetCurrentPlayerLevel() || zombiesSpawns[index].worldNumber != GameManager.instance.currentWorldNumber);
				if (onlyBoss)
				{
					StartCoroutine(DelayedBossSpawn(new Vector3(zombiesSpawns[index].transform.position.x + UnityEngine.Random.Range(-1f, 2f), zombiesSpawns[index].transform.position.y, zombiesSpawns[index].transform.position.z + UnityEngine.Random.Range(-1f, 2f)), bossesSpawned));
				}
				else
				{
					zombiesSpawns[index].Spawn();
				}
				break;
			}
		}
	}

	public void SpawnTutorialWave(float wavePower, int waveNumber)
	{
		currentWave = 0;
		maxCurrentWavePower = wavePower;
		foreach (ZombiesSpawn zombiesSpawn in zombiesSpawns)
		{
			if (zombiesSpawn.worldNumber < 1)
			{
				zombiesSpawn.Spawn();
				break;
			}
		}
	}

	public void StopIt()
	{
		CancelInvoke("SpawnStaticWave");
		CancelInvoke("SpawnGroup");
		inGameWavesActive = false;
	}

	public void TrySpawnBoss()
	{
		if (!bossInDaHause && bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses.Length > bossesSpawned && (float)GetNextBossTargetPoints() <= GameManager.instance.Points - (float)bossDeadAtPoints && !GameManager.instance.isTutorialNow)
		{
			Vector3 spawnPosition;
			if (BossSpawnManager.instance.GetSpawnPosition(out spawnPosition))
			{
				StartCoroutine(DelayedBossSpawn(spawnPosition, bossesSpawned));
				bossWavesDelayMultiplier = bossDelayMultiplier;
				bossesSpawned++;
				TryToSkipNextBoss();
				bossInDaHause = true;
				DataLoader.gui.SetTopPanelAnimationState(bossInDaHause);
			}
			else
			{
				FindBestSpawnAndDoIt(true);
				bossWavesDelayMultiplier = bossDelayMultiplier;
				bossesSpawned++;
				TryToSkipNextBoss();
				bossInDaHause = true;
				DataLoader.gui.SetTopPanelAnimationState(bossInDaHause);
			}
		}
	}

	private bool IsNextBossSkipped()
	{
		if (DataLoader.playerData.killedBosses.Any((KilledBosses kb) => kb.name == bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[bossesSpawned].prefabBoss.myNameIs))
		{
			KilledBosses killedBosses = DataLoader.playerData.killedBosses.First((KilledBosses kb) => kb.name == bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[bossesSpawned].prefabBoss.myNameIs);
			return killedBosses.rewardedStage >= StaticConstants.bossStages.Length || (killedBosses.rewardedStage == StaticConstants.bossStages.Length - 1 && killedBosses.count >= StaticConstants.bossStages[StaticConstants.bossStages.Length - 1]);
		}
		return false;
	}

	private void TryToSkipNextBoss()
	{
		bool flag = true;
		while (bossesSpawned < bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses.Length)
		{
			if (IsNextBossSkipped())
			{
				addedPoints += bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[bossesSpawned].pointsToGo;
				bossesSpawned++;
				flag = false;
				continue;
			}
			if (flag)
			{
				ResetAddedPoints();
			}
			break;
		}
	}

	private void ResetAddedPoints()
	{
		addedPoints = 0;
	}

	public IEnumerator DelayedBossSpawn(Vector3 position, int currentBossCount)
	{
		BossSpawnManager.instance.SpawnFx(position);
		yield return new WaitForSeconds(0.5f);
		UnityEngine.Object.Instantiate(bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[currentBossCount].prefabBoss, position, Quaternion.identity, TransformParentManager.Instance.zombies);
	}

	public void BossDead()
	{
		bossDeadAtPoints = (int)GameManager.instance.Points;
		bossWavesDelayMultiplier = 1f;
		bossInDaHause = false;
		DataLoader.gui.SetTopPanelAnimationState(bossInDaHause);
	}

	public int GetBossDeadAtPoints()
	{
		return bossDeadAtPoints;
	}

	public int GetNextBossTargetPoints()
	{
		if (GameManager.instance.currentWorldNumber - 1 < bossesByWorld.Length && bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses.Length > bossesSpawned)
		{
			return bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[bossesSpawned].pointsToGo + bossesByWorld[GameManager.instance.currentWorldNumber - 1].startPointsToGoByLevel[DataLoader.Instance.GetCurrentPlayerLevel()];
		}
		return 0;
	}

	public void SpawnDailyBoss()
	{
		CancelInvoke("SpawnStaticWave");
		string text = "Boss1";
		Boss[] bosses = bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses;
		foreach (Boss boss in bosses)
		{
			if (boss.prefabBoss.name == text)
			{
				UnityEngine.Object.Instantiate(boss.prefabBoss, dailyBossSpawnPoint.position, boss.prefabBoss.transform.rotation);
				firstWavePower = firstWavePowerByLevel[DataLoader.Instance.GetCurrentPlayerLevel()];
				currentWave = startFromWaveNumberByWorld[GameManager.instance.currentWorldNumber - 1].startFromWaveNumberByLevel[DataLoader.Instance.GetCurrentPlayerLevel()] - 1;
				maxCurrentWavePower = firstWavePower;
				Invoke("SpawnGroup", 1f);
				bossesSpawned = 0;
				bossWavesDelayMultiplier = 1f;
				bossDeadAtPoints = 0;
				bossInDaHause = false;
				DataLoader.gui.SetTopPanelAnimationState(bossInDaHause);
				return;
			}
		}
		Debug.LogError("No found boss prefab with name \"" + text + "\"");
	}

	public void Reset()
	{
		zombiesSpawns.Clear();
	}

	public void StartArenaGame()
	{
		if (zombiesSpawns.Count <= 0)
		{
			return;
		}
		firstWavePower = firstWavePowerByLevel[DataLoader.Instance.GetCurrentPlayerLevel()];
		currentWave = 5;
		maxCurrentWavePower = firstWavePower * Mathf.Pow(scale1, currentWave) + scale2 * (float)currentWave;
		foreach (ZombiesSpawn zombiesSpawn in zombiesSpawns)
		{
			zombiesSpawn.Spawn();
		}
	}

	private void SetBossesByWorldData()
	{
		string[] array = CsvLoader.SplitLines(Resources.Load<TextAsset>("BossInfoCSV1"));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			string bossName = array2[0];
			int num = int.Parse(array2[1]) - 1;
			Boss boss = bossesByWorld[num].bosses.First((Boss item) => item.prefabBoss.myNameIs == bossName);
			boss.gemsReward = int.Parse(array2[4]);
			boss.prefabBoss.countHealth = float.Parse(array2[2]);
			boss.pointsToGo = int.Parse(array2[3]);
		}
		for (int j = 0; j < bossesByWorld.Length; j++)
		{
			bossesByWorld[j].startPointsToGoByLevel.Clear();
		}
		array = CsvLoader.SplitLines(Resources.Load<TextAsset>("StartPointsToGoByLevelCSV"));
		for (int k = 0; k < array.Length; k++)
		{
			string[] array3 = array[k].Split(',');
			bossesByWorld[int.Parse(array3[0]) - 1].startPointsToGoByLevel.Add(int.Parse(array3[1]));
		}
	}
}
