using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public enum GameModes
	{
		Idle = 0,
		GamePlay = 1,
		Arena = 2,
		DailyBoss = 3,
		PVP = 4
	}

	public static GameManager instance;

	[HideInInspector]
	public AsyncOperation operation;

	[HideInInspector]
	public float Points;

	[HideInInspector]
	public int newSurvivorsLeft;

	[HideInInspector]
	public StartPoint[] startPoints;

	[SerializeField]
	public GameObject prefabHealingFx;

	[SerializeField]
	public GameObject prefabTakeDamageSurvivor;

	[SerializeField]
	public GameObject prefabTakeDamageZombie;

	[SerializeField]
	public GameObject prefabZombieBlood;

	[SerializeField]
	public GameObject prefabTakeDamageZombieBoss;

	[SerializeField]
	public GameObject prefabRareGlowFx;

	[SerializeField]
	public GameObject prefabBafFx;

	[SerializeField]
	public GameObject prefabFlyEndFx;

	[SerializeField]
	public Animation prefabCritFx;

	[SerializeField]
	public TrailRenderer prefabZombieTrail;

	[SerializeField]
	public ParticleSystem prefabProtestingZombieFx;

	[SerializeField]
	public ParticleSystem prefabZombieSpawnFx;

	[SerializeField]
	public ParticleSystem prefabArmageddonFx;

	[SerializeField]
	public ParticleSystem prefabBossDeathFx;

	[SerializeField]
	public ParticleSystem prefabHeroDeathFx;

	public ParticleSystem inGameUpgrade;

	[SerializeField]
	private Transform protectDome;

	private Transform tutorialStartPoint;

	public string[] worldNames;

	private ParticleSystem killAllFx;

	private CameraTarget cameraTarget;

	[HideInInspector]
	public bool isTutorialNow;

	private bool squadSpawned = true;

	public List<SurvivorHuman> survivors = new List<SurvivorHuman>();

	public List<ZombieHuman> zombies = new List<ZombieHuman>();

	public FloatingText floatingTextPrefab;

	[Space]
	[SerializeField]
	public int[] bossKillsForOpenWorld;

	public int currentWorldNumber = 1;

	[NonSerialized]
	public int inGameTime;

	[HideInInspector]
	public StartPoint startPoint;

	private int currentArenaNumber = 1;

	[HideInInspector]
	public bool GoToFinishLine;

	public GameModes currentGameMode { get; private set; }

	public float allHeroPowers { get; private set; }

	private void Awake()
	{
		instance = this;
		if (!PlayerPrefs.HasKey(StaticConstants.TutorialCompleted))
		{
			isTutorialNow = true;
			currentGameMode = GameModes.GamePlay;
			InvokeRepeating("IncreaseGameTime", 0f, 1f);
		}
		else
		{
			currentGameMode = GameModes.Idle;
		}
		cameraTarget = UnityEngine.Object.FindObjectOfType<CameraTarget>();
	}

	private void Start()
	{
		RefreshCurrentWorldNumber();
		operation = SceneManager.LoadSceneAsync("World" + currentWorldNumber, LoadSceneMode.Additive);
		StartCoroutine(WaitForScene(OpenGame));
	}

	private IEnumerator WaitForScene(Action callback)
	{
		while (!operation.isDone)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		RefreshStartPoints();
		callback();
		DataLoader.gui.Loading(false);
		Resources.UnloadUnusedAssets();
		WeatherManager.Instance.EnableRandomWeather();
	}

	private void RefreshStartPoints()
	{
		startPoints = UnityEngine.Object.FindObjectsOfType<StartPoint>();
		StartPoint[] array = startPoints;
		foreach (StartPoint startPoint in array)
		{
			if (startPoint.worldNumber < 1)
			{
				tutorialStartPoint = startPoint.transform;
				break;
			}
		}
	}

	private void OpenGame()
	{
		SpawnStartSquad();
	}

	public void RefreshCurrentWorldNumber()
	{
		int worldNumber = ((!PlayerPrefs.HasKey(StaticConstants.currentWorld)) ? 1 : PlayerPrefs.GetInt(StaticConstants.currentWorld));
		if (!IsWorldOpen(worldNumber))
		{
			worldNumber = 1;
		}
		currentWorldNumber = worldNumber;
	}

	public void Reset()
	{
		Time.timeScale = 1f;
		if (currentGameMode == GameModes.DailyBoss)
		{
			RefreshCurrentWorldNumber();
		}
		if (currentGameMode == GameModes.Arena || currentGameMode == GameModes.PVP)
		{
			if (DataLoader.gui != null)
			{
				DataLoader.gui.Loading(true);
			}
			if (currentGameMode == GameModes.Arena)
			{
				SceneManager.UnloadSceneAsync("Arena" + currentArenaNumber);
			}
			else
			{
				SceneManager.UnloadSceneAsync("PVPtestArena");
			}
			currentGameMode = GameModes.Idle;
			SpawnManager.instance.Reset();
			WavesManager.instance.Reset();
			operation = SceneManager.LoadSceneAsync("World" + currentWorldNumber, LoadSceneMode.Additive);
			StartCoroutine(WaitForScene(Reset));
			return;
		}
		currentGameMode = GameModes.Idle;
		SpawnManager.instance.StopIt();
		WavesManager.instance.StopIt();
		MoneyCoinManager.instance.EndGame();
		MoneyBoxManager.instance.EndGame();
		foreach (SurvivorHuman survivor in survivors)
		{
			if (survivor != null)
			{
				UnityEngine.Object.Destroy(survivor.gameObject);
			}
		}
		survivors.Clear();
		squadSpawned = false;
		DropPlace[] array = UnityEngine.Object.FindObjectsOfType<DropPlace>();
		DropPlace[] array2 = array;
		foreach (DropPlace dropPlace in array2)
		{
			UnityEngine.Object.Destroy(dropPlace.gameObject);
		}
		NewSurvivor[] array3 = UnityEngine.Object.FindObjectsOfType<NewSurvivor>();
		NewSurvivor[] array4 = array3;
		foreach (NewSurvivor newSurvivor in array4)
		{
			UnityEngine.Object.Destroy(newSurvivor.gameObject);
		}
		foreach (ZombieHuman zombie in zombies)
		{
			if (zombie != null)
			{
				UnityEngine.Object.Destroy(zombie.gameObject);
			}
		}
		zombies.Clear();
		if (!squadSpawned)
		{
			SpawnStartSquad();
		}
		Points = 0f;
		newSurvivorsLeft = 0;
		DataLoader.Instance.CheckClosedWalls();
		SurvivorHuman.moveForceArenaMultiplier = 1f;
		SurvivorHuman.moveForceAbilityMultiplier = 1f;
	}

	public void Go()
	{
		protectDome.gameObject.SetActive(false);
		currentGameMode = GameModes.GamePlay;
		if (isTutorialNow || PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted))
		{
			WavesManager.instance.StartGame();
		}
		else
		{
			WavesManager.instance.StopIt();
		}
		inGameTime = 0;
		InvokeRepeating("IncreaseGameTime", 0f, 1f);
	//	AnalyticsManager.instance.LogEvent("GameStarted", new Dictionary<string, string>());
		SpawnManager.instance.StartGame();
		MoneyCoinManager.instance.StartGame();
		MoneyBoxManager.instance.StartGame();
		DataLoader.Instance.ResetLocalInfo();
		CalculateAllHeroPowers();
		UIStreak.instance.ResetStreak();
	}

	public void GoDailyBoss()
	{
		currentWorldNumber = 0;
		Reset();
		protectDome.gameObject.SetActive(false);
		currentGameMode = GameModes.DailyBoss;
		WavesManager.instance.SpawnDailyBoss();
		inGameTime = 0;
		InvokeRepeating("IncreaseGameTime", 0f, 1f);
		DataLoader.Instance.ResetLocalInfo();
	}

	public void GoArena()
	{
		currentGameMode = GameModes.Arena;
		cameraTarget.enabled = false;
		GoToFinishLine = false;
		WavesManager.instance.StopIt();
		if (DataLoader.gui != null)
		{
			DataLoader.gui.Loading(true);
		}
		foreach (ZombieHuman zombie in zombies)
		{
			UnityEngine.Object.Destroy(zombie.gameObject);
		}
		zombies.Clear();
		foreach (SurvivorHuman survivor in survivors)
		{
			UnityEngine.Object.Destroy(survivor.gameObject);
		}
		survivors.Clear();
		SceneManager.UnloadSceneAsync("World" + currentWorldNumber);
		SpawnManager.instance.Reset();
		WavesManager.instance.Reset();
		if (DataLoader.playerData.arenaRating <= 0)
		{
			currentArenaNumber = 1;
		}
		else
		{
			currentArenaNumber = UnityEngine.Random.Range(1, 6);
		}
		operation = SceneManager.LoadSceneAsync("Arena" + currentArenaNumber, LoadSceneMode.Additive);
		StartCoroutine(WaitForScene(StartArena));
		ArenaWavesManager.instance.zombiesLimiterWaveNumber = ArenaManager.instance.currentArenaInfo.waveNumber;
		ArenaManager.instance.StartArenaGame();
	}

	public void StartArena()
	{
		SpawnStartSquad();
		protectDome.gameObject.SetActive(false);
		Points = 0f;
		newSurvivorsLeft = 0;
		AnalyticsManager.instance.LogEvent("ArenaStarted", new Dictionary<string, string> { 
		{
			"ArenaNumber",
			currentArenaNumber.ToString()
		} });
		DataLoader.Instance.ResetLocalInfo();
		CalculateAllHeroPowers();
		UIStreak.instance.ResetStreak();
	}

	public void ArenaReadyToStart()
	{
		inGameTime = 0;
		InvokeRepeating("IncreaseGameTime", 0f, 1f);
		ArenaWavesManager.instance.StartArenaTimer();
		DataLoader.gui.StartArena();
	}

	public void PVPReadyToStart()
	{
		inGameTime = 0;
		CancelInvoke("IncreaseGameTime");
		InvokeRepeating("IncreaseGameTime", 0f, 1f);
		DataLoader.gui.StartPVP();
	}

	public void CalculateAllHeroPowers()
	{
		allHeroPowers = 0f;
		for (int i = 0; i < DataLoader.playerData.heroData.Count; i++)
		{
			allHeroPowers += DataLoader.Instance.GetHeroPower(DataLoader.playerData.heroData[i].heroType);
		}
	}

	private void SpawnStartSquad()
	{
		Vector3 position = Vector3.zero;
		if (!isTutorialNow)
		{
			List<StartPoint> list = new List<StartPoint>();
			StartPoint[] array = startPoints;
			foreach (StartPoint startPoint in array)
			{
				if (currentGameMode == GameModes.Arena || (startPoint.openAtLevel <= DataLoader.Instance.GetCurrentPlayerLevel() && startPoint.worldNumber == currentWorldNumber))
				{
					list.Add(startPoint);
				}
			}
			if (list.Count <= 0)
			{
				Debug.LogError("Compatible places for SpawnStartSquad Not Found!");
			}
			else
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				position = list[index].transform.position;
				this.startPoint = list[index];
			}
		}
		else
		{
			position = tutorialStartPoint.position;
		}
		if (isTutorialNow)
		{
			protectDome.gameObject.SetActive(false);
		}
		else
		{
			protectDome.position = position;
			protectDome.gameObject.SetActive(true);
		}
		DataLoader.Instance.startSquad = new int[Enum.GetValues(typeof(SaveData.HeroData.HeroType)).Length];
		for (int j = 0; j < DataLoader.Instance.survivors.Count; j++)
		{
			if (DataLoader.playerData.IsHeroOpened(DataLoader.Instance.survivors[j].heroType))
			{
				float num = UnityEngine.Random.Range(-0.5f, 0.5f);
				float num2 = UnityEngine.Random.Range(-0.5f, 0.5f);
				UnityEngine.Object.Instantiate(DataLoader.Instance.GetSurvivorPrefab(DataLoader.Instance.survivors[j].heroType), new Vector3(position.x + num, position.y, position.z + num2), default(Quaternion), TransformParentManager.Instance.heroes);
				DataLoader.Instance.startSquad[j]++;
				Debug.LogError("XXXX");
			}
		}
		cameraTarget.transform.position = position;
		cameraTarget.enabled = true;
		if (!isTutorialNow && currentGameMode == GameModes.Idle)
		{
			WavesManager.instance.StartIdle();
		}
	}

	public void SpawnSurvivor(SurvivorHuman survivor)
	{
		Vector3 position = TransformParentManager.Instance.heroes.GetChild(0).transform.position;
		survivors.Clear();
		survivors.Add(UnityEngine.Object.Instantiate(survivor, position, Quaternion.identity, TransformParentManager.Instance.heroes));
		for (int i = 0; i < TransformParentManager.Instance.heroes.childCount - 1; i++)
		{
			UnityEngine.Object.Destroy(TransformParentManager.Instance.heroes.GetChild(i).gameObject);
		}
		Debug.LogError("X2");
	}

	public void IncreaseGameTime()
	{
		inGameTime++;
	}

	public void DecreaseSurvivor(SurvivorHuman survivor)
	{
		if (survivors.Remove(survivor) && survivors.Count <= 0 && UnityEngine.Object.FindObjectsOfType<Parashute>().Length <= 0)
		{
			GameOver();
		}
	}

	public bool IsTutorialCompleted()
	{
		return PlayerPrefs.HasKey(StaticConstants.TutorialCompleted) && PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted) && PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted);
	}

	public void DecreaseZombie(ZombieHuman zombie)
	{
		if (!zombies.Remove(zombie) || isTutorialNow)
		{
			return;
		}
		if (currentGameMode == GameModes.GamePlay)
		{
			Points += zombie.power;
			if (zombie.zombieType != SaveData.ZombieData.ZombieType.BOSS)
			{
				DataLoader.Instance.SaveDeadZombie(zombie.zombieType, zombie.power);
			}
			UIStreak.instance.IncreaseStreak();
			WavesManager.instance.TrySpawnBoss();
			DataLoader.gui.RefreshProgressToBoss();
		}
		else if (currentGameMode == GameModes.DailyBoss && zombies.Count <= 0)
		{
			DailyBossComplete();
		}
	}

	public void KillAll()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			if (!(zombies[i].tag == "ZombieBoss"))
			{
				zombies[i].TakeDamage(zombies[i].maxCountHealth + 10f);
				i--;
			}
		}
		if (killAllFx == null)
		{
			killAllFx = UnityEngine.Object.Instantiate(prefabArmageddonFx);
		}
		killAllFx.transform.position = new Vector3(cameraTarget.transform.position.x, prefabArmageddonFx.transform.position.y, cameraTarget.transform.position.z);
		killAllFx.Play();
	}

	public void DeleteAllInactiveZombies()
	{
		for (int i = 0; i < zombies.Count; i++)
		{
			if (!zombies[i].isActive)
			{
				UnityEngine.Object.Destroy(zombies[i].gameObject);
				zombies.RemoveAt(i);
				i--;
			}
		}
	}

	public void WinArena()
	{
		ArenaManager.instance.SaveArenaRating(ArenaManager.instance.GetAddedWinRating() + ArenaManager.instance.winRating);
		squadSpawned = false;
		CancelInvoke("IncreaseGameTime");
		DataLoader.Instance.SaveEndMatchInfo();
		DataLoader.gui.WinArena();
	}

	public void GameOver()
	{
		if (currentGameMode == GameModes.Arena)
		{
			ArenaManager.instance.UpdateAll();
			ArenaWavesManager.instance.StopArenaTimer();
		}
		SpawnManager.instance.StopIt();
		WavesManager.instance.StopIt();
		MoneyCoinManager.instance.EndGame();
		MoneyBoxManager.instance.EndGame();
		squadSpawned = false;
		CancelInvoke("IncreaseGameTime");
		DataLoader.Instance.SaveEndMatchInfo();
		DataLoader.gui.GameOver();
		if (isTutorialNow)
		{
			isTutorialNow = false;
			PlayerPrefs.SetInt(StaticConstants.TutorialCompleted, 0);
			PlayerPrefs.Save();
		}
	}

	public void DailyBossComplete()
	{
		Debug.Log("Daily boss complete");
		squadSpawned = false;
		CancelInvoke("IncreaseGameTime");
		DataLoader.Instance.SaveEndMatchInfo();
		DataLoader.gui.DailyBossComplete();
	}

	public bool IsWorldOpen(int worldNumber)
	{
		if (bossKillsForOpenWorld.Length >= worldNumber && bossKillsForOpenWorld[worldNumber - 1] <= DataLoader.playerData.GetZombieByType(SaveData.ZombieData.ZombieType.BOSS).totalTimesKilled)
		{
			return true;
		}
		return false;
	}

	public int ChangeWorld(int direction)
	{
		if (IsWorldOpen(currentWorldNumber + direction))
		{
			if (DataLoader.gui != null)
			{
				DataLoader.gui.Loading(true);
			}
			SceneManager.UnloadSceneAsync("World" + currentWorldNumber);
			currentWorldNumber += direction;
			SpawnManager.instance.Reset();
			WavesManager.instance.Reset();
			operation = SceneManager.LoadSceneAsync("World" + currentWorldNumber, LoadSceneMode.Additive);
			StartCoroutine(WaitForScene(Reset));
			PlayerPrefs.SetInt(StaticConstants.currentWorld, currentWorldNumber);
			PlayerPrefs.Save();
			return 0;
		}
		return bossKillsForOpenWorld[currentWorldNumber + direction - 1] - DataLoader.playerData.GetZombieByType(SaveData.ZombieData.ZombieType.BOSS).totalTimesKilled;
	}

	public void GoToWorld(int worldIndex)
	{
		if (currentWorldNumber - 1 != worldIndex)
		{
			if (DataLoader.gui != null)
			{
				DataLoader.gui.Loading(true);
			}
			SceneManager.UnloadSceneAsync("World" + currentWorldNumber);
			currentWorldNumber = worldIndex + 1;
			SpawnManager.instance.Reset();
			WavesManager.instance.Reset();
			operation = SceneManager.LoadSceneAsync("World" + currentWorldNumber, LoadSceneMode.Additive);
			StartCoroutine(WaitForScene(Reset));
			PlayerPrefs.SetInt(StaticConstants.currentWorld, currentWorldNumber);
			PlayerPrefs.Save();
		}
	}

	public int GetWorldsCount()
	{
		return bossKillsForOpenWorld.Length;
	}

	public void LoadPVPArena()
	{
		currentGameMode = GameModes.PVP;
		cameraTarget.enabled = false;
		GoToFinishLine = false;
		WavesManager.instance.StopIt();
		if (DataLoader.gui != null)
		{
			DataLoader.gui.Loading(true);
		}
		foreach (ZombieHuman zombie in zombies)
		{
			UnityEngine.Object.Destroy(zombie.gameObject);
		}
		zombies.Clear();
		foreach (SurvivorHuman survivor in survivors)
		{
			UnityEngine.Object.Destroy(survivor.gameObject);
		}
		survivors.Clear();
		SceneManager.UnloadSceneAsync("World" + currentWorldNumber);
		SpawnManager.instance.Reset();
		WavesManager.instance.Reset();
		operation = SceneManager.LoadSceneAsync("PVPtestArena", LoadSceneMode.Additive);
		StartCoroutine(WaitForScene(delegate
		{
			Time.timeScale = 0f;
			protectDome.gameObject.SetActive(false);
			Points = 0f;
			newSurvivorsLeft = 0;
			AnalyticsManager.instance.LogEvent("PVPStarted", new Dictionary<string, string> { 
			{
				"ArenaNumber",
				currentArenaNumber.ToString()
			} });
			DataLoader.Instance.ResetLocalInfo();
			CalculateAllHeroPowers();
			UIStreak.instance.ResetStreak();
		}));
	}

	public void EnableCameraTarget(Vector3 pos)
	{
		cameraTarget.transform.position = pos;
		cameraTarget.enabled = true;
	}
}
