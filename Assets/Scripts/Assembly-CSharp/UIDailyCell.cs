using System.Collections.Generic;
using Daily;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyCell : UIScrollCell
{
	[SerializeField]
	private Image dailyPresent;

	[SerializeField]
	private ParticleSystem claimFx;

	[SerializeField]
	private Image boosterImage;

	[SerializeField]
	private Text boosterCountText;

	[SerializeField]
	private GameObject boosterCountObj;

	[SerializeField]
	private GameObject halfTransparentImage;

	[SerializeField]
	private Text coinsCountText;

	[SerializeField]
	private Text dayNumberText;

	[SerializeField]
	private Button btnClaim;

	[Space]
	private DailyBonusData.RewardType rewardType;

	private DailyContent dailyContent;

	private void Start()
	{
		btnClaim.onClick.AddListener(UIController.instance.scrollControllers.dailyController.ClaimReward);
	}

	public void SetData()
	{
		rewardType = DataLoader.Instance.dailyBonus[base.cellIndex].type;
		switch (rewardType)
		{
		case DailyBonusData.RewardType.Money:
			SetMoney(true);
			SetBooster(false);
			break;
		case DailyBonusData.RewardType.Booster:
			if (DataLoader.Instance.dailyBonus[base.cellIndex].boosterType == SaveData.BoostersData.BoosterType.NewSurvivor)
			{
				boosterImage.sprite = dailyContent.GetBoosterSprite(0);
			}
			else
			{
				boosterImage.sprite = dailyContent.GetBoosterSprite(1);
			}
			SetMoney(false);
			SetBooster(true);
			break;
		}
		dailyPresent.gameObject.SetActive(rewardType == DailyBonusData.RewardType.Money);
		if (LanguageManager.instance.IsReverseLanguage(LanguageManager.instance.currentLanguage.language))
		{
			dayNumberText.text = 1 + base.cellIndex % 7 + LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Day);
		}
		else
		{
			dayNumberText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Day) + " " + (1 + base.cellIndex % 7);
		}
		dayNumberText.font = LanguageManager.instance.currentLanguage.font;
		bool flag = dailyContent.NeedToEnableHalfTransparent();
		halfTransparentImage.SetActive(flag);
		btnClaim.gameObject.SetActive(!flag);
		dayNumberText.gameObject.SetActive(flag);
		dayNumberText.color = dailyContent.GetTextColor();
	}

	public void SetDailyContent(DailyContent dailyContent)
	{
		this.dailyContent = dailyContent;
		dailyPresent.sprite = dailyContent.GetDailyPresentSprite();
		SetData();
	}

	private void SetMoney(bool money)
	{
		coinsCountText.gameObject.SetActive(money);
		coinsCountText.text = CalculateCoinsReward().ToString();
		coinsCountText.color = dailyContent.GetTextColor();
	}

	public float CalculateCoinsReward()
	{
		int currentPlayerLevel = DataLoader.Instance.GetCurrentPlayerLevel();
		return Mathf.Round((float)DataLoader.Instance.dailyBonus[base.cellIndex].reward * Mathf.Pow(StaticConstants.DailyCoinMultiplier, currentPlayerLevel - 1) + StaticConstants.DailyCoinMultiplier2 * (float)(currentPlayerLevel - 1));
	}

	public void SetBooster(bool active)
	{
		boosterImage.gameObject.SetActive(active);
		boosterCountText.text = DataLoader.Instance.dailyBonus[base.cellIndex].reward.ToString();
	}

	public void ActivateReward()
	{
		SoundManager.Instance.PlaySound(SoundManager.Instance.claimSound);
		switch (rewardType)
		{
		case DailyBonusData.RewardType.Money:
			DataLoader.Instance.RefreshMoney(CalculateCoinsReward(), true);
			break;
		case DailyBonusData.RewardType.Multiplier:
			DataLoader.Instance.SetNewMultiplier(DataLoader.Instance.dailyBonus[base.cellIndex].reward, StaticConstants.MultiplierDurationInSeconds * 2);
			break;
		case DailyBonusData.RewardType.Booster:
			DataLoader.Instance.BuyBoosters(DataLoader.Instance.dailyBonus[base.cellIndex].boosterType, DataLoader.Instance.dailyBonus[base.cellIndex].reward);
			break;
		}
		claimFx.Play();
		AnalyticsManager.instance.LogEvent("DailyRewardClaim", new Dictionary<string, string>
		{
			{
				"TotalDays",
				(DataLoader.playerData.totalDaysInRow + 1).ToString()
			},
			{
				"Type",
				rewardType.ToString()
			}
		});
	}
}
