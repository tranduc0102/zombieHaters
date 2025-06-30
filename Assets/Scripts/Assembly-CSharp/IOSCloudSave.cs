using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using EPPZ.Cloud;
using UnityEngine;

public class IOSCloudSave : MonoBehaviour
{
	[HideInInspector]
	public bool loadingCompleted;

	public static IOSCloudSave instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	public void SaveAll()
	{
	}

	public void ForceSave()
	{
	}

	private void SaveHeroData(List<SaveData.HeroData> heroData)
	{
		for (int i = 0; i < heroData.Count; i++)
		{
			Cloud.SetIntForKey(heroData[i].currentLevel, (int)heroData[i].heroType + "level");
			Cloud.SetIntForKey(heroData[i].pickedUpCount, (int)heroData[i].heroType + "pickedUP");
			Cloud.SetIntForKey(heroData[i].diedCount, (int)heroData[i].heroType + "died");
			Cloud.SetBoolForKey(heroData[i].isOpened, (int)heroData[i].heroType + "isopened");
		}
	}

	private List<SaveData.HeroData> LoadHeroData()
	{
		List<SaveData.HeroData> list = new List<SaveData.HeroData>();
		for (int i = 0; i < Enum.GetValues(typeof(SaveData.HeroData.HeroType)).Length; i++)
		{
			int num = Cloud.IntForKey(i + "level");
			if (num < 1)
			{
				num = 1;
			}
			list.Add(new SaveData.HeroData
			{
				heroType = (SaveData.HeroData.HeroType)i,
				currentLevel = num,
				pickedUpCount = Cloud.IntForKey(i + "pickedUP"),
				diedCount = Cloud.IntForKey(i + "died"),
				isOpened = Cloud.BoolForKey(i + "isopened")
			});
		}
		return list;
	}

	public void SaveFirstDate()
	{
		string value = DataLoader.playerData.firstEnterDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		Cloud.SetStringForKey(value, "FirstEnterDate");
	}

	private void LoadFirstDate()
	{
		try
		{
			DataLoader.playerData.firstEnterDate = DateTime.Parse(Cloud.StringForKey("FirstEnterDate"), CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
			Debug.Log("Cant load first date");
			StartCoroutine(WaitForTimeManger());
		}
	}

	private IEnumerator WaitForTimeManger()
	{
		while (!TimeManager.gotDateTime)
		{
			yield return null;
		}
		yield return null;
		SaveFirstDate();
	}

	private void SaveZombieData(List<SaveData.ZombieData> zombieData)
	{
		for (int i = 0; i < zombieData.Count; i++)
		{
			Cloud.SetIntForKey(zombieData[i].totalTimesKilled, (int)zombieData[i].zombieType + "killedtotalZombies");
			Cloud.SetIntForKey(zombieData[i].killedByCapsule, (int)zombieData[i].zombieType + "killedcapsuleZombies");
		}
	}

	private List<SaveData.ZombieData> LoadZombieData()
	{
		List<SaveData.ZombieData> list = new List<SaveData.ZombieData>();
		for (int i = 0; i < Enum.GetValues(typeof(SaveData.ZombieData.ZombieType)).Length; i++)
		{
			list.Add(new SaveData.ZombieData
			{
				zombieType = (SaveData.ZombieData.ZombieType)i,
				totalTimesKilled = Cloud.IntForKey(i + "killedtotalZombies"),
				killedByCapsule = Cloud.IntForKey(i + "killedcapsuleZombies")
			});
		}
		return list;
	}

	private void SavePassiveAbilities(List<SaveData.PassiveAbilityData> dataList)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			Cloud.SetIntForKey(dataList[i].abilityLevel, (int)dataList[i].abilityType + "AbilityLevel");
		}
	}

	private List<SaveData.PassiveAbilityData> LoadPassiveAbilies()
	{
		List<SaveData.PassiveAbilityData> list = new List<SaveData.PassiveAbilityData>();
		for (int i = 0; i < Enum.GetValues(typeof(PassiveAbilityTypes)).Length; i++)
		{
			list.Add(new SaveData.PassiveAbilityData
			{
				abilityType = (PassiveAbilityTypes)i,
				abilityLevel = Cloud.IntForKey(i + "AbilityLevel")
			});
		}
		return list;
	}

	private void SaveBoosterData(List<SaveData.BoostersData> boosters)
	{
		for (int i = 0; i < boosters.Count; i++)
		{
			Cloud.SetIntForKey(boosters[i].count, (int)boosters[i].type + "boostercount");
		}
	}

	private List<SaveData.BoostersData> LoadBoosterData()
	{
		List<SaveData.BoostersData> list = new List<SaveData.BoostersData>();
		for (int i = 0; i < Enum.GetValues(typeof(SaveData.BoostersData.BoosterType)).Length; i++)
		{
			list.Add(new SaveData.BoostersData
			{
				type = (SaveData.BoostersData.BoosterType)i,
				count = Cloud.IntForKey(i + "boostercount")
			});
		}
		return list;
	}

	private void SaveRewardedKilledBosses()
	{
		for (int i = 0; i < DataLoader.playerData.killedBosses.Count; i++)
		{
			Cloud.SetIntForKey(DataLoader.playerData.killedBosses[i].rewardedStage, DataLoader.playerData.killedBosses[i].name + "Stage");
		}
	}

	private void LoadRewardedKilledBosses()
	{
		DataLoader.playerData.killedBosses = new List<KilledBosses>();
		for (int i = 0; i < WavesManager.instance.bossesByWorld.Length; i++)
		{
			for (int j = 0; j < WavesManager.instance.bossesByWorld[i].bosses.Length; j++)
			{
				int num = Cloud.IntForKey(WavesManager.instance.bossesByWorld[i].bosses[j].prefabBoss.myNameIs + "Stage");
				if (num > 0)
				{
					DataLoader.playerData.killedBosses.Add(new KilledBosses
					{
						name = WavesManager.instance.bossesByWorld[i].bosses[j].prefabBoss.myNameIs,
						count = 0,
						rewardedStage = num
					});
				}
			}
		}
	}

	private void SaveAllBossesRewarded()
	{
		for (int i = 0; i < WavesManager.instance.bossesByWorld.Length; i++)
		{
			Cloud.SetIntForKey(PlayerPrefs.HasKey(StaticConstants.allBossesRewardedkey + i) ? 1 : 0, StaticConstants.allBossesRewardedkey + i);
		}
	}

	private void LoadAllBossesRewarded()
	{
		for (int i = 0; i < WavesManager.instance.bossesByWorld.Length; i++)
		{
			if (Cloud.IntForKey(StaticConstants.allBossesRewardedkey + i) == 1)
			{
				PlayerPrefs.SetInt(StaticConstants.allBossesRewardedkey + i, 1);
			}
		}
	}

	public void SaveSimple()
	{
		Cloud.SetFloatForKey((float)DataLoader.playerData.experience, "experience");
		Cloud.SetFloatForKey((float)DataLoader.playerData.money, "money");
		Cloud.SetIntForKey(DataLoader.playerData.gamesPlayed, "gamesplayed");
		Cloud.SetFloatForKey(DataLoader.playerData.totalDamage, "totalDamage");
		Cloud.SetFloatForKey(DataLoader.playerData.bestScore, "bestscore");
		Cloud.SetIntForKey(DataLoader.playerData.arenaRating, "arenaRating");
		Cloud.SetIntForKey(DataLoader.playerData.gems, "gems");
	}

	public bool TryToLoad()
	{
		if (loadingCompleted)
		{
			return true;
		}
		loadingCompleted = true;
		return false;
	}

	private IEnumerator DelayedBossLoading()
	{
		while (WavesManager.instance == null && !DataLoader.initialized)
		{
			yield return null;
		}
		LoadRewardedKilledBosses();
		LoadAllBossesRewarded();
		UIController.instance.scrollControllers.achievementsController.MarkAchievementsCompleted();
		DataLoader.gui.UpdateMenuContent();
		Debug.Log("Delayed boss Loading completed");
	}

	public void SetTutorialsCompleted()
	{
		PlayerPrefs.SetInt(StaticConstants.TutorialCompleted, 1);
		PlayerPrefs.SetInt(StaticConstants.UpgradeTutorialCompleted, 1);
		if (DataLoader.playerData.gamesPlayed > 1)
		{
			PlayerPrefs.SetInt(StaticConstants.AbilityTutorialCompleted, 1);
		}
		if (DataLoader.Instance.GetCurrentPlayerLevel() > 3)
		{
			PlayerPrefs.SetInt(StaticConstants.LeagueTutorialCompleted, 1);
		}
	}

	public void SavePlayerPrefs()
	{
		CheckSavePrefsKey(StaticConstants.starterPackPurchased);
		CheckSavePrefsKey(StaticConstants.interstitialAdsKey);
		CheckSavePrefsKey(StaticConstants.infinityMultiplierPurchased);
	}

	public void LoadPlayerPrefs()
	{
		CheckLoadPrefsKey(StaticConstants.starterPackPurchased);
		CheckLoadPrefsKey(StaticConstants.interstitialAdsKey);
		CheckLoadPrefsKey(StaticConstants.infinityMultiplierPurchased);
	}

	public void CheckSavePrefsKey(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			Cloud.SetIntForKey(1, key);
		}
		else
		{
			Cloud.SetIntForKey(0, key);
		}
	}

	public void CheckLoadPrefsKey(string key)
	{
		if (Cloud.IntForKey(key) == 1)
		{
			PlayerPrefs.SetInt(key, 1);
		}
	}
}
