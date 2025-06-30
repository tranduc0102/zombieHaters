using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PVPManager : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Follow hero opening sequence")]
	private List<PvpOpenInfo> survivorsInfo;

	[SerializeField]
	private int numberOfPlayers = 8;

	[SerializeField]
	private List<Transform> playerSpawnPoints;

	[SerializeField]
	private List<Transform> botSpawnPoints;

	public List<Transform> lootBoxes;

	[SerializeField]
	private PlayerPVPGroupController playerControllerPrefab;

	[SerializeField]
	private BotPVPGroupController botControllerPrefab;

	[HideInInspector]
	public static List<PVPPlayerInfo> pvpPlayers;

	[SerializeField]
	private LootObject lootPrefab;

	[Header("survivor born fxs")]
	public ParticleSystem birthOfOurSurvivorFx;

	public ParticleSystem birthOfEnemieSurvivorFx;

	[Header("zombies spawn parameters")]
	[SerializeField]
	public float zombiesGroupPower = 1.5f;

	[SerializeField]
	public int zombiesLimiterWaveNumber = 1;

	public int[] levels;

	[HideInInspector]
	public int currentArenaIndex;

	[HideInInspector]
	public int lastPlace;

	public float pvpMinutes;

	[Space]
	[SerializeField]
	private AudioClip pizzaPickUpClip;

	private bool gameOverActivated;

	public static PVPManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		ArenaManager.instance.StartArenaGame();
		gameOverActivated = false;
		ObjectPooler.Instance.AddLootObject(lootPrefab);
		Camera.main.transform.parent.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y + 4f, Camera.main.transform.parent.position.z - 2f);
		currentArenaIndex = CalculateCurrentArenaIndex();
		CreatePlayers();
		StartCoroutine(Timer());
		lastPlace = pvpPlayers.Count - 1;
	}

	private IEnumerator Timer()
	{
		int seconds = (int)(pvpMinutes * 60f);
		while (seconds > 0)
		{
			seconds--;
			DataLoader.gui.lbl_pvpInGameTimer.text = string.Format("{0:D2}:{1:D2}", seconds / 60, seconds % 60);
			yield return new WaitForSeconds(1f);
		}
		PvpGameOver();
	}

	public void PvpGameOver()
	{
		if (!gameOverActivated)
		{
			gameOverActivated = true;
			StartCoroutine(DelayedGameOver());
		}
	}

	private IEnumerator DelayedGameOver()
	{
		yield return new WaitForSeconds(1.5f);
		GameManager.instance.GameOver();
	}

	public void Reset()
	{
		for (int i = 0; i < pvpPlayers.Count; i++)
		{
			if (pvpPlayers[i].controller != null)
			{
				Object.Destroy(pvpPlayers[i].controller.gameObject);
			}
		}
	}

	private int CalculateCurrentArenaIndex()
	{
		for (int i = 0; i < DataLoader.Instance.botsData.arenaRating.Count - 1; i++)
		{
			if (DataLoader.playerData.arenaRating < DataLoader.Instance.botsData.arenaRating[i + 1])
			{
				return i;
			}
		}
		return DataLoader.Instance.botsData.arenaRating.Count - 1;
	}

	private void OnDestroy()
	{
		ObjectPooler.Instance.DisablePvpObjects();
		if (Camera.main != null)
		{
			Camera.main.transform.parent.position = new Vector3(Camera.main.transform.parent.position.x, Camera.main.transform.parent.position.y - 4f, Camera.main.transform.parent.position.z + 2f);
		}
		Reset();
	}

	public void CreatePlayers()
	{
		Transform transform = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Count)];
		PlayerPVPGroupController playerPVPGroupController = Object.Instantiate(playerControllerPrefab, transform.position, Quaternion.identity);
		playerPVPGroupController.SpawnSquad(transform.position, 0);
		List<Transform> list = botSpawnPoints;
		pvpPlayers[0].place = -1;
		for (int i = 1; i < numberOfPlayers; i++)
		{
			if (list.Count == 0)
			{
				break;
			}
			transform = list[Random.Range(0, list.Count)];
			Object.Instantiate(botControllerPrefab, transform.position, Quaternion.identity).SpawnSquad(transform.position, i);
			list.Remove(transform);
			pvpPlayers[i].place = -1;
		}
		DataLoader.gui.pvpScoreBoard.UpdateLeaderBoard();
	}

	public void RemoveLootBox(PVPLootBox lootBox)
	{
		lootBoxes.Remove(lootBox.transform);
		Object.Destroy(lootBox.gameObject);
	}

	public SurvivorHuman IsInMyTeam(GameObject obj, int index)
	{
		for (int i = 0; i < pvpPlayers[index].survivors.Count; i++)
		{
			if (pvpPlayers[index].survivors[i].gameObject == obj)
			{
				return pvpPlayers[index].survivors[i];
			}
		}
		return null;
	}

	public int GetStrongestGroupIndex()
	{
		int result = 0;
		int count = pvpPlayers[0].survivors.Count;
		for (int i = 1; i < pvpPlayers.Count; i++)
		{
			if (pvpPlayers[i].survivors.Count > count)
			{
				result = i;
				count = pvpPlayers[i].survivors.Count;
			}
		}
		return result;
	}

	public bool GetWeakGroups(int myGroupIndex, out List<PVPPlayerInfo> weakGroups)
	{
		int count = pvpPlayers[myGroupIndex].survivors.Count;
		weakGroups = new List<PVPPlayerInfo>();
		for (int i = 0; i < pvpPlayers.Count; i++)
		{
			if (i != myGroupIndex && pvpPlayers[i].survivors.Count + 1 < count)
			{
				weakGroups.Add(pvpPlayers[i]);
			}
		}
		return weakGroups.Count > 0;
	}

	public bool AddLoot(GameObject obj)
	{
		for (int i = 0; i < pvpPlayers.Count; i++)
		{
			for (int j = 0; j < pvpPlayers[i].survivors.Count; j++)
			{
				if (pvpPlayers[i].survivors[j].gameObject == obj)
				{
					if (i == 0)
					{
						pvpPlayers[i].IncreaseLoot(1f);
						SoundManager.Instance.PlaySound(pizzaPickUpClip, 1f);
					}
					else
					{
						pvpPlayers[i].IncreaseLoot(1.15f + (float)ArenaManager.instance.GetArenaIndexByRating(DataLoader.playerData.arenaRating) * 0.24f);
					}
					return true;
				}
			}
		}
		return false;
	}

	public void AddZombieLoot(int pvpIndex)
	{
		pvpPlayers[pvpIndex].IncreaseLoot(0.1f);
	}

	public PVPSoldierSurvivor GetSoldierByLevel(int pvpIndex)
	{
		if (pvpPlayers.Count <= pvpIndex)
		{
			return survivorsInfo[0].prefab;
		}
		int level = pvpPlayers[pvpIndex].controller.level;
		List<int> list = new List<int>();
		for (int i = 0; i < survivorsInfo.Count; i++)
		{
			if (CheckOpened(pvpIndex, survivorsInfo[i].heroType))
			{
				list.Add(i);
				if (i >= survivorsInfo.Count - 1)
				{
					if (level <= survivorsInfo[i].openLevel + 2)
					{
						return survivorsInfo[i].prefab;
					}
				}
				else if (i + 1 < survivorsInfo.Count && level <= survivorsInfo[i + 1].openLevel)
				{
					return survivorsInfo[i].prefab;
				}
			}
			else if (i + 1 < survivorsInfo.Count && level <= survivorsInfo[i + 1].openLevel)
			{
				break;
			}
		}
		return survivorsInfo[list[Random.Range(0, list.Count)]].prefab;
	}

	public bool CheckOpened(int pvpIndex, SaveData.HeroData.HeroType type)
	{
		return DataLoader.Instance.IsHeroOpened(type);
	}

	public PVPSoldierSurvivor GetRandomSoldier()
	{
		return survivorsInfo[Random.Range(0, survivorsInfo.Count)].prefab;
	}
}
