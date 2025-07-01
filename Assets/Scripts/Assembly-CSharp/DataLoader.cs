using System;
using System.Collections.Generic;
using System.Linq;
using IAP;
using UnityEngine;
using GUI = GuiInGame.GUI;
public class DataLoader : MonoBehaviour
{
	public static SaveData playerData;

	public static GUI gui;

	public static DataUpdateManager dataUpdateManager;

	public static bool initialized;

	[SerializeField]
	private TextAsset survivorLevels;

	[SerializeField]
	private TextAsset achievementData;

	[SerializeField]
	private TextAsset experienceData;

	[SerializeField]
	private TextAsset moneyBoxData;

	[SerializeField]
	private TextAsset survivorPowers;

	[SerializeField]
	private TextAsset survivorDamage;

	[SerializeField]
	private TextAsset survivorHP;

	[SerializeField]
	private TextAsset pvpBotsData;

	public List<ZombiePrefabData> zombiePrefabData;

	public List<ZombieHuman> zombiesPrefabs;

	public List<Survivors> survivors;

	public List<Achievements> achievements;

	public List<DailyBonusData> dailyBonus;

	public PVPBotsData botsData;

	public PassiveAbilitiesManager passiveAbilitiesManager;

	[HideInInspector]
	public List<DailyBossData> dailyBosses;

	[HideInInspector]
	public float[] moneyBoxGold;

	[HideInInspector]
	public double[] levelExperience;

	[HideInInspector]
	public int[] startSquad;

	[HideInInspector]
	public int moneyMultiplier = 1;

	private List<KilledZombies> killedZombies;

	private HidingObject[] hidingObjects;

	private int[] killedZombieCapsule;

	private int[] pickedSurvivors;

	[NonSerialized]
	public int currentDayInRow = -1;

	[NonSerialized]
	public bool secretPicked;

	[NonSerialized]
	public NotificationManager notifManger;

	[NonSerialized]
	public double inGameMoneyCounter;

	private DateTime lastCheatDt = DateTime.MinValue;

	public static DataLoader Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Initialize();
		Application.targetFrameRate = 60;
	}
	private void Initialize()
	{
        InitializePlayerData();
		SavePlayerData();
		FillBotsData();
		FillAchievements();
        FillSurvivorLevels();
        SetExperienceLevels();
		SetMoneyBoxData();
		FillZombiesPrefabData();
		dataUpdateManager = UnityEngine.Object.FindObjectOfType<DataUpdateManager>();
		notifManger = UnityEngine.Object.FindObjectOfType<NotificationManager>();
		hidingObjects = null;
		LoadingScene loadingScene = UnityEngine.Object.FindObjectOfType<LoadingScene>();
		if (loadingScene != null)
		{
			loadingScene.StartLoading();
		}
    }

	public void InitializePlayerData()
	{
		try
		{
            initialized = false;
            if (!SaveManager.Load(StaticConstants.PlayerSaveDataPath, ref playerData))
            {
                if (playerData == null || (playerData != null && !playerData.CheckNewData()))
                {
                    PlayerPrefs.DeleteAll();
                    SaveManager.RemoveData(StaticConstants.PlayerSaveDataPath);
                    playerData = new SaveData();
                    playerData.Init();
                    GetNewOpenedHeroes();
                    gui = UnityEngine.Object.FindObjectOfType<GUI>();
                    if (gui != null)
                    {
                        gui.StartTutorialAfterReset();
                    }
                }
            }
            else
            {
                playerData.CheckNewData();
            }
            passiveAbilitiesManager.Initialize();
		}
        catch (Exception ex)
        {
	        Debug.LogError("Lỗi InitializePlayerData: " + ex);
        }
    }

    public static void SetGui(GUI _gui)
	{
		gui = _gui;
		initialized = true;
	}

	private void FillBotsData()
	{
		try
		{
            botsData = new PVPBotsData();
            string[] array = CsvLoader.SplitLines(pvpBotsData);
            int[] array2 = new int[9] { 0, 1, 2, 3, 4, 7, 5, 8, 6 };
            for (int i = 0; i < survivors.Count; i++)
            {
                botsData.botData.Add(new PVPBotsData.BotData
                {
                    heroType = (SaveData.HeroData.HeroType)array2[i]
                });
            }
            for (int j = 0; j < array.Length; j++)
            {
                string[] array3 = array[j].Split(',');
                botsData.arenaRating.Add(int.Parse(array3[0]));
                botsData.displayedPower.Add(float.Parse(array3[1]));
                for (int k = 0; k < survivors.Count; k++)
                {
                    botsData.botData[k].levels.Add(int.Parse(array3[k + 2]));
                }
            }
        }catch (Exception ex)
		{
			Debug.LogError("Lỗi FillBotsData: " + ex);
        }
    }

    private void FillSurvivorLevels()
    {
        try
        {
            int[,] array = new int[survivors.Count, playerData.survivorMaxLevel];
            float[,] array2 = new float[playerData.survivorMaxLevel, survivors.Count];
            float[,] array3 = new float[playerData.survivorMaxLevel, survivors.Count];
            float[,] array4 = new float[playerData.survivorMaxLevel, survivors.Count];
            CsvLoader.SplitText(survivorLevels, ',', ref array);
            CsvLoader.SplitText(survivorPowers, ',', ref array2);
            CsvLoader.SplitText(survivorDamage, ',', ref array3);
            CsvLoader.SplitText(survivorHP, ',', ref array4);
            int[] array5 = new int[9] { 0, 1, 2, 3, 4, 6, 8, 5, 7 };
            for (int i = 0; i < survivors.Count; i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    survivors[array5[i]].levels.Add(new Survivors.SurvivorLevels
                    {
                        damage = array3[j, i],
                        cost = array[i, j],
                        power = array2[j, i],
                        hp = array4[j, i]
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
	       Debug.LogError("Lỗi FillSurvivorLevels: " + ex);
        }
    }

    public void SavePlayerData()
	{
		try
		{
            if (playerData != null)
            {
                SaveManager.Save(playerData, StaticConstants.PlayerSaveDataPath);
                if (initialized)
                {
                    IOSCloudSave.instance.SaveAll();
                    GPGSCloudSave.CloudSync(false);
                }
            }
		}
		catch(Exception ex)
		{
			Debug.LogError("Lỗi SavePlayerData: " + ex);
        }
    }

	private void FillAchievements()
	{
		try
		{
			string[] array = CsvLoader.SplitLines(achievementData);
			for (int i = 0; i < achievements.Count; i++)
			{
				string[] array2 = array[i].Split(';');
				achievements[i].ID = int.Parse(array2[0]);
				achievements[i].name = array2[1];
				achievements[i].description = array2[2];
				achievements[i].type = int.Parse(array2[3]);
				achievements[i].count = int.Parse(array2[4]);
				achievements[i].reward = int.Parse(array2[5]);
			}
		}
        catch (Exception ex)
        {
	        Debug.LogError("Lỗi FillAchievements: " + ex);
        }

    }
    private void SetExperienceLevels()
	{
		try
		{
            CsvLoader.SplitText<double>(experienceData, ',', out levelExperience);
        }
        catch (Exception ex)
        {
	        Debug.LogError("Lỗi SetExperienceLevels: " + ex);
        }
    }

    private void SetMoneyBoxData()
	{
		try
		{
            CsvLoader.SplitText<float>(moneyBoxData, ',', out moneyBoxGold);
            if (playerData.moneyBoxPicked >= moneyBoxGold.Length - 1)
            {
                playerData.moneyBoxPicked -= 5;
            }
        }
        catch (Exception ex)
        {
	        Debug.LogError("Lỗi SetMoneyBoxData: " + ex);
        }
    }

    private void FillZombiesPrefabData()
	{
		try
		{
            string[] array = CsvLoader.SplitLines(Resources.Load<TextAsset>("ZombiesInfoCSV"));
            if (array.Length != zombiesPrefabs.Count)
            {
	            Debug.LogError("CSV data and prefabs count don't match. CSV: " + array.Length + " Prefabs: " + zombiesPrefabs.Count);
                return;
            }
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(',');
                zombiesPrefabs[i].zombieType = (SaveData.ZombieData.ZombieType)int.Parse(array2[2]);
                zombiesPrefabs[i].countHealth = float.Parse(array2[0]);
                zombiesPrefabs[i].damage = int.Parse(array2[1]);
                zombiesPrefabs[i].power = zombiesPrefabs[i].countHealth / StaticConstants.ZombiePowerConst;
            }
            zombiesPrefabs.Clear();
            zombiesPrefabs = null;
        }
        catch (Exception ex)
        {
	        Debug.LogError("Lỗi FillZombiesPrefabData: " + ex);
        }
    }

    private void SetZombiePrefabData()
	{
		for (int i = 0; i < zombiePrefabData.Count; i++)
		{
			zombiePrefabData[i].power = (float)zombiePrefabData[i].health / StaticConstants.ZombiePowerConst;
			zombiePrefabData[i].SetPrefabData();
		}
	}

	public bool OpenHero(SaveData.HeroData.HeroType heroType)
	{
		int index = playerData.heroData.IndexOf(playerData.heroData.First((SaveData.HeroData hd) => hd.heroType == heroType));
		if (playerData.heroData[index].isOpened)
		{
			return false;
		}
		SaveData.HeroData heroData = playerData.heroData[index];
		heroData.isOpened = true;
		playerData.heroData[index] = heroData;
		SavePlayerData();
		gui.CheckOpenedHeroes();
		gui.UpdateMenuContent();
		GameManager.instance.Reset();
		UnityEngine.Object.FindObjectOfType<HeroInfo>().SetIsLocked(false);
		return true;
	}

	public void RefreshIdleZombieGold(double money, bool needToSave = false)
	{
		money *= (double)((!InAppManager.Instance.IsSubscribed()) ? 1f : 1.5f);
		if (money < 1.0)
		{
			if ((TimeManager.CurrentDateTime - lastCheatDt).TotalSeconds > 3.0)
			{
				lastCheatDt = TimeManager.CurrentDateTime;
				RefreshMoney(1.0, needToSave);
			}
			else
			{
				RefreshMoney(money, needToSave);
			}
		}
		else
		{
			RefreshMoney(money, needToSave);
		}
	}

	public bool RefreshMoney(double money, bool needToSave = false)
	{
		if (playerData.money + money >= 0.0)
		{
			playerData.money += money;
			gui.UpdateMoney();
			if (needToSave)
			{
				SavePlayerData();
			}
			return true;
		}
		return false;
	}

	public bool RefreshGems(int gemsAmount, bool needToSave = false)
	{
		if (playerData.gems + gemsAmount >= 0)
		{
			playerData.gems += gemsAmount;
			gui.UpdateGems();
			if (needToSave)
			{
				SavePlayerData();
			}
			return true;
		}
		return false;
	}

	public float GetHeroDamage(SaveData.HeroData.HeroType heroType)
	{
		return GetHeroDamageByLevel(heroType, GetHeroLevel(heroType));
	}

	public float GetHeroDamageByLevel(SaveData.HeroData.HeroType heroType, int level)
	{
		return survivors.FirstOrDefault((Survivors s) => s.heroType == heroType).levels[level - 1].damage;
	}

	public float GetHeroHP(SaveData.HeroData.HeroType heroType)
	{
		return GetHeroHPByLevel(heroType, GetHeroLevel(heroType));
	}

	public float GetHeroHPByLevel(SaveData.HeroData.HeroType heroType, int level)
	{
		return survivors.FirstOrDefault((Survivors s) => s.heroType == heroType).levels[level - 1].hp;
	}

	public float GetHeroPowerByLevel(SaveData.HeroData.HeroType heroType, int level)
	{
		if (!IsHeroOpened(heroType))
		{
			return 0f;
		}
		return survivors.FirstOrDefault((Survivors s) => s.heroType == heroType).levels[level - 1].power / 10f;
	}

	public float GetHeroPower(SaveData.HeroData.HeroType heroType)
	{
		return GetHeroPowerByLevel(heroType, GetHeroLevel(heroType));
	}

	public int GetHeroLevel(SaveData.HeroData.HeroType heroType)
	{
		return playerData.heroData.FirstOrDefault((SaveData.HeroData hd) => hd.heroType == heroType).currentLevel;
	}

	public float GetTimeWarpGoldCount(int seconds = 270)
	{
		float num = 0f;
		for (int i = 0; i < playerData.heroData.Count; i++)
		{
			num += GetHeroPower(playerData.heroData[i].heroType);
		}
		float num2 = num * (1f + PassiveAbilitiesManager.bonusHelper.GetCriticalHitChance()) * (1f + PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus) * (1f + PassiveAbilitiesManager.bonusHelper.GoldBonus) * (float)seconds * StaticConstants.IdleGoldConst;
		return (!InAppManager.Instance.IsSubscribed()) ? num2 : (num2 * 1.5f);
	}

	public List<SaveData.HeroData> GetNewOpenedHeroes()
	{
		List<SaveData.HeroData> list = new List<SaveData.HeroData>();
		int currentPlayerLevel = GetCurrentPlayerLevel();
		bool flag = false;
		TimeSpan timeSpan;
		flag = playerData.GetTimeInGameCount(out timeSpan);
		for (int i = 0; i < playerData.heroData.Count; i++)
		{
			SaveData.HeroData heroData = playerData.heroData[i];
			if (heroData.isOpened)
			{
				continue;
			}
			foreach (Survivors survivor in survivors)
			{
				if (survivor.heroType != heroData.heroType)
				{
					continue;
				}
				switch (survivor.heroOpenType)
				{
				case HeroOpenType.Level:
					if (currentPlayerLevel >= survivor.requiredLevelToOpen)
					{
						heroData.isOpened = true;
						if (survivor.heroType != 0)
						{
							list.Add(heroData);
						}
					}
					break;
				case HeroOpenType.Day:
					if (flag && timeSpan.TotalDays >= (double)survivor.daysToOpen)
					{
						heroData.isOpened = true;
						list.Add(heroData);
					}
					break;
				}
			}
			if (playerData.heroData[i].isOpened != heroData.isOpened)
			{
				playerData.heroData[i] = heroData;
			}
		}
		return list;
	}

	public int GetCurrentPlayerLevel()
	{
		for (int i = 0; i < levelExperience.Length; i++)
		{
			if (i + 1 < levelExperience.Length)
			{
				if (playerData.experience >= levelExperience[i] && playerData.experience < levelExperience[i + 1])
				{
					return i + 1;
				}
				continue;
			}
			return levelExperience.Length;
		}
		return 1;
	}

	public bool IsHeroOpened(SaveData.HeroData.HeroType heroType)
	{
		return playerData.heroData.First((SaveData.HeroData hd) => hd.heroType == heroType).isOpened;
	}

	public void SetPlayerLevel(int level)
	{
		playerData.experience = levelExperience[level - 1];
		SavePlayerData();
	}

	public void UpdateInGameMoneyCounter(double money)
	{
		inGameMoneyCounter += money;
		gui.UpdateMoney(inGameMoneyCounter);
	}

	public void ResetInGameMoneyCounter()
	{
		inGameMoneyCounter = 0.0;
		gui.UpdateMoney(inGameMoneyCounter);
	}

	public double SaveDeadZombie(SaveData.ZombieData.ZombieType type, float _power)
	{
		if (killedZombies == null)
		{
			killedZombies = new List<KilledZombies>();
		}
		if (killedZombies.Any((KilledZombies kz) => kz.power == _power))
		{
			killedZombies.First((KilledZombies kz) => kz.power == _power).count++;
		}
		else
		{
			killedZombies.Add(new KilledZombies
			{
				id = (int)type,
				count = 1,
				power = _power
			});
		}
		double num = _power * StaticConstants.InGameGoldConst * (1f + PassiveAbilitiesManager.bonusHelper.GoldBonus) * ((!InAppManager.Instance.IsSubscribed()) ? 1f : 1.5f);
		UpdateInGameMoneyCounter(num);
		return num;
	}

	public void AddPickedUpCount(SaveData.HeroData.HeroType type)
	{
		pickedSurvivors[(int)type]++;
	}

	public void SaveDeadByCapsule(SaveData.ZombieData.ZombieType type)
	{
		killedZombieCapsule[(int)type]++;
	}

	public double SaveKilledBoss(float power, string _name)
	{
		SaveData.ZombieData value = playerData.zombieData[5];
		value.totalTimesKilled++;
		playerData.zombieData[5] = value;
		if (playerData.killedBosses.Any((KilledBosses kb) => kb.name == _name))
		{
			playerData.killedBosses.First((KilledBosses kb) => kb.name == _name).count++;
		}
		else
		{
			playerData.killedBosses.Add(new KilledBosses
			{
				name = _name,
				count = 1,
				rewarded = false,
				rewardedStage = 0
			});
		}
		SavePlayerData();
		AnalyticsManager.instance.LogEvent("KilledBoss", new Dictionary<string, string>
		{
			{ "Name", _name },
			{
				"TimeFromStart",
				GameManager.instance.inGameTime.ToString()
			}
		});
		return SaveDeadZombie(SaveData.ZombieData.ZombieType.BOSS, ((!(_name == "Angry Phil")) ? power : (power * 6f)) * StaticConstants.BossZombieMultiplier);
	}

	public void SaveEndMatchInfo()
	{
		double num = 0.0;
		int num2 = 0;
		int i;
		for (i = 0; i < killedZombies.Count; i++)
		{
			SaveData.ZombieData value = playerData.zombieData.First((SaveData.ZombieData zd) => zd.zombieType == (SaveData.ZombieData.ZombieType)killedZombies[i].id);
			if (value.zombieType != SaveData.ZombieData.ZombieType.BOSS)
			{
				value.totalTimesKilled += killedZombies[i].count;
			}
			for (int j = 0; j < playerData.zombieData.Count; j++)
			{
				if (playerData.zombieData[j].zombieType == (SaveData.ZombieData.ZombieType)killedZombies[i].id)
				{
					playerData.zombieData[j] = value;
				}
			}
			playerData.totalDamage += killedZombies[i].CalculateTotalDamage();
		}
		num = GameManager.instance.Points * StaticConstants.InGameExpConst;
		if (GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay)
		{
			playerData.money += inGameMoneyCounter;
			playerData.experience += Math.Ceiling(num);
			if (playerData.experience > levelExperience[levelExperience.Length - 1])
			{
				playerData.experience = levelExperience[levelExperience.Length - 1];
			}
		}
		for (int k = 0; k < pickedSurvivors.Length; k++)
		{
			SaveData.HeroData heroData = playerData.heroData[k];
			heroData.pickedUpCount += pickedSurvivors[k];
			num2 += pickedSurvivors[k];
			heroData.diedCount += pickedSurvivors[k] + startSquad[k];
			playerData.heroData[k] = heroData;
		}
		long num3 = (long)GameManager.instance.Points;
		playerData.gamesPlayed++;
		UIController.instance.scrollControllers.survivorsController.SetRandomVideo();
		if (!GameManager.instance.isTutorialNow)
		{
			if (num3 > playerData.bestScore)
			{
				playerData.bestScore = num3;
			}
			LeaderboardManager.instance.PostScoreOnLeaderBoard(num3);
			gui.gameOverManager.SetGameOverMenu(inGameMoneyCounter, num2, (int)Math.Ceiling(num), num3, GameManager.instance.inGameTime);
		}
		SavePlayerData();
	}

	public void ResetLocalInfo()
	{
		UpdateIdleHeroDamage();
		killedZombies = new List<KilledZombies>();
		pickedSurvivors = new int[Enum.GetValues(typeof(SaveData.HeroData.HeroType)).Length];
		ResetInGameMoneyCounter();
		CheckClosedWalls();
		secretPicked = false;
		gui.isnewHeroOpened = false;
	}

	public void UpdateIdleHeroDamage()
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			SurvivorHuman[] array = UnityEngine.Object.FindObjectsOfType<SurvivorHuman>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].heroDamage = GetHeroDamage(array[i].heroType);
			}
		}
	}

	public void UpdateIdleHeroShootDelay()
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			SurvivorHuman[] array = UnityEngine.Object.FindObjectsOfType<SurvivorHuman>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CalculateShootDelay();
			}
		}
	}

	public void UpdateIdleHero(SaveData.HeroData.HeroType type)
	{
		Transform parent = null;
		SurvivorHuman[] array = UnityEngine.Object.FindObjectsOfType<SurvivorHuman>();
		for (int i = 0; i < array.Length; i++)
		{
			if (type == array[i].heroType)
			{
				array[i].heroDamage = GetHeroDamage(array[i].heroType);
				parent = array[i].transform;
			}
		}
		ParticleSystem particleSystem = UnityEngine.Object.Instantiate(GameManager.instance.inGameUpgrade, parent);
		particleSystem.Play();
		UnityEngine.Object.Destroy(particleSystem.gameObject, particleSystem.main.duration);
	}

	public void SetNewMultiplier(int multiplier, int durationInSeconds)
	{
		if (TimeManager.gotDateTime)
		{
			if (multiplier > moneyMultiplier)
			{
				PlayerPrefs.SetInt(StaticConstants.MultiplierKey, multiplier);
			}
			PlayerPrefs.SetString(StaticConstants.DailyMultiplierTime, TimeManager.CurrentDateTime.AddSeconds((double)durationInSeconds + GetMultiplierTime().TotalSeconds).Ticks.ToString());
			dataUpdateManager.UpdateMoneyMultiplier();
			SavePlayerData();
		}
	}

	public void SetTotalDays(int value, bool needToSave = false)
	{
		SaveData saveData = playerData;
		saveData.totalDaysInRow = value;
		playerData = saveData;
		if (needToSave)
		{
			SavePlayerData();
		}
	}

	public SurvivorHuman GetSurvivorPrefab(SaveData.HeroData.HeroType type)
	{
		foreach (Survivors survivor in survivors)
		{
			if (survivor.heroType == type)
			{
				return survivor.survivorPrefab;
			}
		}
		return null;
	}

	public TimeSpan GetMultiplierTime()
	{
		TimeSpan result = TimeSpan.MinValue;
		if (PlayerPrefs.HasKey(StaticConstants.DailyMultiplierTime))
		{
			result = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.DailyMultiplierTime)), DateTimeKind.Utc).Subtract(TimeManager.CurrentDateTime);
		}
		if (result.TotalSeconds > 0.0)
		{
			return result;
		}
		return default(TimeSpan);
	}

	public void SavePickedMoneyBox(Vector3 position)
	{
		if (TimeManager.gotDateTime)
		{
			PlayerPrefs.SetString(StaticConstants.MoneyBoxKey, TimeManager.CurrentDateTime.Date.Ticks.ToString());
			playerData.moneyBoxPicked++;
			FloatingText floatingText = UnityEngine.Object.Instantiate(GameManager.instance.floatingTextPrefab, position, Quaternion.identity, TransformParentManager.Instance.fx);
			floatingText.StartBaseAnimation(moneyBoxGold[playerData.moneyBoxPicked - 1].ToString());
			if (playerData.moneyBoxPicked >= moneyBoxGold.Length - 1)
			{
				playerData.moneyBoxPicked -= 5;
			}
			RefreshMoney(moneyBoxGold[playerData.moneyBoxPicked - 1] * (float)moneyMultiplier, true);
			gui.newSecret.SetActive(false);
			MoneyBoxManager.instance.TrySpawnBox();
			secretPicked = true;
			AnalyticsManager.instance.LogEvent("MoneyBoxPicked", new Dictionary<string, string> { 
			{
				"Level",
				playerData.moneyBoxPicked.ToString()
			} });
		}
	}

	public void CheckClosedWalls()
	{
		int currentPlayerLevel = GetCurrentPlayerLevel();
		ClosedWall[] array = UnityEngine.Object.FindObjectsOfType<ClosedWall>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Check(currentPlayerLevel);
		}
		GetHidingObjects();
		for (int j = 0; j < hidingObjects.Length; j++)
		{
			hidingObjects[j].UpdateObject(currentPlayerLevel);
		}
	}

	public void GetHidingObjects()
	{
		hidingObjects = UnityEngine.Object.FindObjectsOfType<HidingObject>();
	}

	public void UpdateClosedWallsText()
	{
		ClosedWall[] array = UnityEngine.Object.FindObjectsOfType<ClosedWall>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateText();
		}
	}

	public void BuyBoosters(SaveData.BoostersData.BoosterType boosterType, int amount = 1)
	{
		for (int i = 0; i < playerData.boosters.Count; i++)
		{
			SaveData.BoostersData value = playerData.boosters[i];
			if (value.type == boosterType)
			{
				value.count += amount;
				playerData.boosters[i] = value;
				SavePlayerData();
				gui.popUpsPanel.shop.UpdateBoosters();
			}
		}
	}

	public bool UseBoosters(SaveData.BoostersData.BoosterType boosterType)
	{
		for (int i = 0; i < playerData.boosters.Count; i++)
		{
			SaveData.BoostersData value = playerData.boosters[i];
			if (value.type == boosterType)
			{
				value.count--;
				playerData.boosters[i] = value;
				AnalyticsManager.instance.LogEvent("Used_Ability", new Dictionary<string, string>
				{
					{
						"Type",
						boosterType.ToString()
					},
					{
						"Count_Remaining",
						value.count.ToString()
					}
				});
				Debug.LogFormat("Used booster {0}", boosterType);
				return true;
			}
		}
		return false;
	}

	public int GetBoostersCount(SaveData.BoostersData.BoosterType boosterType)
	{
		for (int i = 0; i < playerData.boosters.Count; i++)
		{
			SaveData.BoostersData boostersData = playerData.boosters[i];
			if (boostersData.type == boosterType)
			{
				return boostersData.count;
			}
		}
		return 0;
	}
}
