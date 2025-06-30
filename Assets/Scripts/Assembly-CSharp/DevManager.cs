using System;
using System.Collections.Generic;
using System.Linq;
using IAP;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DevManager : MonoBehaviour
{
	[SerializeField]
	private RectTransform canvas;

	[SerializeField]
	private RectTransform debugConsolePrefab;

	[SerializeField]
	private Text goldText;

	[SerializeField]
	private Text gemsText;

	[SerializeField]
	private List<AbilityUpgrade> abilityUpgrades;

	[SerializeField]
	private GameObject bottomPanel;

	public void Start()
	{
		for (int i = 0; i < abilityUpgrades.Count; i++)
		{
			PassiveAbilityTypes abilityType = abilityUpgrades[i].abilityType;
			abilityUpgrades[i].upgradeButton.onClick.AddListener(delegate
			{
				UpgradeAbility(abilityType);
			});
			if (abilityUpgrades[i].buttonReset != null)
			{
				abilityUpgrades[i].buttonReset.onClick.AddListener(delegate
				{
					ResetAbility(abilityType);
				});
			}
		}
	}

	public void OnEnable()
	{
		UpdateData();
		for (int i = 0; i < abilityUpgrades.Count; i++)
		{
			abilityUpgrades[i].UpdateText();
		}
	}

	public void UpgradeAbility(PassiveAbilityTypes abilityType)
	{
		if (DataLoader.Instance.passiveAbilitiesManager.UpgradeAbility(abilityType))
		{
			UpdateData();
			abilityUpgrades.First((AbilityUpgrade ab) => ab.abilityType == abilityType).UpdateText();
		}
	}

	public void ResetAbility(PassiveAbilityTypes abilityType)
	{
		DataLoader.playerData.GetPassiveAbilityByType(abilityType).Reset();
		abilityUpgrades.First((AbilityUpgrade ab) => ab.abilityType == abilityType).UpdateText();
		DataLoader.Instance.SavePlayerData();
	}

	public void UpdateData()
	{
		goldText.text = ((int)DataLoader.playerData.money).ToString();
		gemsText.text = DataLoader.playerData.gems.ToString();
	}

	public void IncGems(int amount)
	{
		DataLoader.Instance.RefreshGems(amount);
		UpdateData();
	}

	public void IncreaseMoney(int count)
	{
		DataLoader.Instance.RefreshMoney(count, true);
		UpdateData();
	}

	public void IncreaseExperience()
	{
		if (DataLoader.Instance.GetCurrentPlayerLevel() >= DataLoader.Instance.levelExperience.Length)
		{
			return;
		}
		double num = DataLoader.Instance.levelExperience[DataLoader.Instance.GetCurrentPlayerLevel()] - 1.0;
		if (DataLoader.playerData.experience == num)
		{
			if (DataLoader.Instance.levelExperience.Length != DataLoader.Instance.GetCurrentPlayerLevel() + 1)
			{
				DataLoader.playerData.experience = DataLoader.Instance.levelExperience[DataLoader.Instance.GetCurrentPlayerLevel() + 1] - 1.0;
			}
			else
			{
				DataLoader.playerData.experience = num + 1.0;
			}
		}
		else
		{
			DataLoader.playerData.experience = num;
		}
		DataLoader.gui.UpdateMenuContent();
		DataLoader.gui.CheckOpenedHeroes();
		DataLoader.Instance.SavePlayerData();
		DataLoader.Instance.CheckClosedWalls();
	}

	public void OpenTestSuite()
	{
/*		AdsManager.instance.TestSuite();
*/	}

	public void DailyRewardDay()
	{
		PlayerPrefs.SetString(StaticConstants.DailyRewardKey, new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.DailyRewardKey)), DateTimeKind.Utc).AddDays(-1.0).Ticks.ToString());
		UIController.instance.scrollControllers.dailyController.ActivateDailyReward();
	}

	public void SetHeroesMaxLevel()
	{
		for (int i = 0; i < DataLoader.playerData.heroData.Count; i++)
		{
			SaveData.HeroData heroData = DataLoader.playerData.heroData[i];
			heroData.currentLevel = DataLoader.playerData.survivorMaxLevel;
			DataLoader.playerData.heroData[i] = heroData;
		}
		DataLoader.Instance.SetPlayerLevel(DataLoader.Instance.levelExperience.Length);
		DataLoader.gui.CheckOpenedHeroes();
		UIController.instance.scrollControllers.survivorsController.AnimateLastheroOpened();
		DataLoader.gui.popUpsPanel.DisablePopups();
		DataLoader.Instance.SavePlayerData();
		GameManager.instance.Reset();
	}

	public void AddFirstDateDays(int days)
	{
		DataLoader.playerData.firstEnterDate = DataLoader.playerData.firstEnterDate.AddDays(days);
		DataLoader.gui.UpdateMenuContent();
	}

	public void ShowBanner()
	{
/*		AdsManager.instance.ShowBanner();
*/	}

	public void ResetData(bool startWithTutorial)
	{
		PlayerPrefs.DeleteAll();
		if (!startWithTutorial)
		{
			PlayerPrefs.SetInt(StaticConstants.TutorialCompleted, 0);
			PlayerPrefs.SetInt(StaticConstants.UpgradeTutorialCompleted, 1);
		}
		SaveManager.RemoveData(StaticConstants.PlayerSaveDataPath);
		SceneManager.sceneLoaded += delegate
		{
			DataLoader.playerData = null;
			DataLoader.Instance.InitializePlayerData();
		};
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OpenNextWorld()
	{
		if (GameManager.instance.currentWorldNumber < GameManager.instance.bossKillsForOpenWorld.Length)
		{
			SaveData.ZombieData value = DataLoader.playerData.zombieData.First((SaveData.ZombieData zd) => zd.zombieType == SaveData.ZombieData.ZombieType.BOSS);
			value.totalTimesKilled = GameManager.instance.bossKillsForOpenWorld[GameManager.instance.currentWorldNumber];
			for (int i = 0; i < DataLoader.playerData.zombieData.Count; i++)
			{
				if (DataLoader.playerData.zombieData[i].zombieType == SaveData.ZombieData.ZombieType.BOSS)
				{
					DataLoader.playerData.zombieData[i] = value;
				}
			}
			DataLoader.gui.RefreshWorldsButtons();
			DataLoader.Instance.SavePlayerData();
		}
		else
		{
			Debug.Log("Next world doesn't exist");
		}
	}

	public void SendTestNotif()
	{
		NotificationManager.instance.SendTestNotification();
	}

	public void AddArenaRating(int rating)
	{
		ArenaManager.instance.ratingBeforeGame = DataLoader.playerData.arenaRating;
		UIController.instance.scrollControllers.ratingContoller.lastRating = ArenaManager.instance.ratingBeforeGame;
		ArenaManager.instance.SaveArenaRating(rating);
	}

	public void BuySubscription()
	{
		InAppManager.Instance.BuyProductID(InAppManager.Instance.subscription.index);
	}

	public void GetSubscriptionInfo()
	{
		Debug.Log("Is sub enabled: " + InAppManager.Instance.IsSubscriptionEnabled(InAppManager.Instance.subscription));
	}

	public void LogSub2()
	{
		SubscribedType subscribedType = InAppManager.Instance.GetSubscribedType(InAppManager.Instance.subscription);
		string empty = string.Empty;
		empty = ((subscribedType != SubscribedType.Subscribed) ? "FAILED" : "COMPLETED");
		Debug.Log("Dev manager sub test " + empty + " Result: " + subscribedType);
	}

	public void LoadSampleScene()
	{
		SceneManager.LoadScene("SampleScene");
	}

	public void OpenDebugConsole()
	{
		if (DebugLogManager.Instance == null)
		{
			UnityEngine.Object.Instantiate(debugConsolePrefab, base.transform.parent);
			DebugLogManager.Instance.canvasTR = canvas;
		}
		DebugLogManager.Instance.Show();
	}

	public void ResetSubscriptionData()
	{
		InAppManager.Instance.localSubManager = new LocalSubscriptionManager();
		InAppManager.Instance.SaveLocalSubManager();
		InAppManager.Instance.GetSubscribedType(InAppManager.Instance.subscription);
		DataLoader.gui.popUpsPanel.subscription.UpdateRedCircle();
	}

	public void OpenBottomPanel()
	{
		bottomPanel.SetActive(!bottomPanel.activeInHierarchy);
	}

	public void SpawnTestSurvivor(SurvivorHuman surv)
	{
		GameManager.instance.SpawnSurvivor(surv);
	}
}
