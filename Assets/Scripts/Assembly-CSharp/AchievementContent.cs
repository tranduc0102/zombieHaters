using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchievementContent : UIScrollCell
{
	public enum ClaimType
	{
		NOTREADY = 0,
		READY = 1,
		CLAIMED = 2
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private Image fillImage;

	[SerializeField]
	private Text descriptionText;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text currentValueText;

	[SerializeField]
	private Button button;

	[SerializeField]
	private GameObject progress;

	[SerializeField]
	private GameObject completed;

	[SerializeField]
	private Text rewardText;

	[SerializeField]
	private ParticleSystem claimParticle;

	[NonSerialized]
	public ClaimType claimType;

	private string description;

	public override void SetContent(int index)
	{
		base.SetContent(index);
		SetLocalizedText();
		icon.sprite = DataLoader.Instance.achievements[base.cellIndex].icon;
		GetCorrectContentValues();
		SetLocalizedText();
	}

	public void UpdateContent()
	{
		GetCorrectContentValues();
	}

	public void UpdateLocalizaton()
	{
		SetLocalizedText();
	}

	public void SetLocalizedText()
	{
		if (descriptionText != null)
		{
			descriptionText.text = LanguageManager.instance.GetLocalizedText(DataLoader.Instance.achievements[base.cellIndex].description);
			descriptionText.font = LanguageManager.instance.currentLanguage.font;
		}
		if (nameText != null)
		{
			nameText.text = LanguageManager.instance.GetLocalizedText(DataLoader.Instance.achievements[base.cellIndex].name);
			nameText.font = LanguageManager.instance.currentLanguage.font;
		}
	}

	public void ClaimReward()
	{
		DataLoader.playerData.achievementsCompleted.Add(new SaveData.AchievementsCompleted
		{
			typeID = DataLoader.Instance.achievements[base.cellIndex].type,
			localID = DataLoader.Instance.achievements[base.cellIndex].ID
		});
		DataLoader.Instance.RefreshGems(DataLoader.Instance.achievements[base.cellIndex].reward, true);
		claimParticle.Play();
		button.gameObject.SetActive(false);
		progress.gameObject.SetActive(false);
		completed.SetActive(true);
		DataLoader.gui.UpdateMenuContent();
		SoundManager.Instance.PlaySound(SoundManager.Instance.claimSound);
		AnalyticsManager.instance.LogEvent("AchievementClaim", new Dictionary<string, string> { 
		{
			"ID",
			base.cellIndex.ToString()
		} });
	}

	public void MarkComleted()
	{
		DataLoader.playerData.achievementsCompleted.Add(new SaveData.AchievementsCompleted
		{
			typeID = DataLoader.Instance.achievements[base.cellIndex].type,
			localID = DataLoader.Instance.achievements[base.cellIndex].ID
		});
		button.gameObject.SetActive(false);
		progress.gameObject.SetActive(false);
		completed.SetActive(true);
		UpdateContent();
	}

	public void GetCorrectContentValues()
	{
		int num = 0;
		switch (DataLoader.Instance.achievements[base.cellIndex].type)
		{
		case 1:
			num = (DataLoader.playerData.heroData[DataLoader.Instance.achievements[base.cellIndex].ID].isOpened ? 1 : 0);
			break;
		case 2:
		{
			for (int j = 0; j < DataLoader.playerData.heroData.Count; j++)
			{
				num += DataLoader.playerData.heroData[j].pickedUpCount;
			}
			break;
		}
		case 3:
			num = DataLoader.playerData.heroData[DataLoader.Instance.achievements[base.cellIndex].ID - 1].currentLevel;
			break;
		case 4:
		{
			for (int k = 0; k < DataLoader.playerData.heroData.Count; k++)
			{
				num += DataLoader.playerData.heroData[k].diedCount;
			}
			break;
		}
		case 5:
			num = DataLoader.playerData.zombieData[DataLoader.Instance.achievements[base.cellIndex].ID - 1].totalTimesKilled;
			break;
		case 6:
		{
			for (int i = 0; i < DataLoader.playerData.zombieData.Count; i++)
			{
				num += DataLoader.playerData.zombieData[i].killedByCapsule;
			}
			break;
		}
		case 7:
			num = (int)DataLoader.playerData.totalDamage;
			break;
		}
		rewardText.text = string.Format("{0:N0}", DataLoader.Instance.achievements[base.cellIndex].reward);
		fillImage.fillAmount = (float)num / (float)DataLoader.Instance.achievements[base.cellIndex].count;
		currentValueText.text = num + "/" + DataLoader.Instance.achievements[base.cellIndex].count;
		if (DataLoader.playerData.achievementsCompleted.Any((SaveData.AchievementsCompleted a) => a.typeID == DataLoader.Instance.achievements[base.cellIndex].type && a.localID == DataLoader.Instance.achievements[base.cellIndex].ID))
		{
			button.gameObject.SetActive(false);
			progress.gameObject.SetActive(false);
			completed.SetActive(true);
			rewardText.gameObject.SetActive(false);
			claimType = ClaimType.CLAIMED;
		}
		else if (fillImage.fillAmount >= 1f)
		{
			claimType = ClaimType.READY;
			progress.gameObject.SetActive(false);
			button.gameObject.SetActive(true);
			UIController.instance.scrollControllers.achievementsController.newAchievementsCount++;
		}
		else
		{
			claimType = ClaimType.NOTREADY;
			button.gameObject.SetActive(false);
		}
	}

	public float GetProgressPercentage()
	{
		return fillImage.fillAmount;
	}
}
