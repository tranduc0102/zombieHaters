using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWheelCell : MonoBehaviour
{
	public RectTransform rectTransform;

	[SerializeField]
	private Text boosterCountText;

	[SerializeField]
	private Text coinsCountText;

	[SerializeField]
	private Text pizzaCount;

	[SerializeField]
	private Text timeWarpText;

	[SerializeField]
	private Image boosterImage;

	[SerializeField]
	private Image coinImage;

	[SerializeField]
	private Image pizzaImage;

	[SerializeField]
	private Image timeWarpImage;

	[HideInInspector]
	public int rewardAmount;

	public SaveData.BoostersData.BoosterType boosterType { get; private set; }

	public WheelCellType type { get; private set; }

	public void SetMoney(int amount, bool doubleReward = false)
	{
		if (amount == 0)
		{
			amount = ((!doubleReward) ? 100 : 200);
		}
		DisableImage(pizzaImage);
		DisableImage(timeWarpImage);
		rewardAmount = amount;
		coinImage.gameObject.SetActive(true);
		boosterImage.gameObject.SetActive(false);
		coinsCountText.text = AbbreviationUtility.AbbreviateNumber(rewardAmount, 2).ToString();
		type = WheelCellType.Coin;
	}

	public void SetBoosters(int amount, SaveData.BoostersData.BoosterType _type)
	{
		rewardAmount = amount;
		boosterImage.gameObject.SetActive(true);
		coinImage.gameObject.SetActive(false);
		DisableImage(pizzaImage);
		DisableImage(timeWarpImage);
		boosterCountText.text = rewardAmount.ToString();
		boosterType = _type;
		type = WheelCellType.Booster;
		switch (_type)
		{
		case SaveData.BoostersData.BoosterType.NewSurvivor:
			boosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[0];
			break;
		case SaveData.BoostersData.BoosterType.KillAll:
			boosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[1];
			break;
		}
	}

	public void SetTimeWarp(int seconds)
	{
		rewardAmount = seconds;
		boosterImage.gameObject.SetActive(false);
		coinImage.gameObject.SetActive(false);
		timeWarpImage.gameObject.SetActive(true);
		DisableImage(pizzaImage);
		timeWarpText.text = (seconds / 3600).ToString();
		type = WheelCellType.TimeWarp;
	}

	public void SetPizza(int amount)
	{
		rewardAmount = amount;
		boosterImage.gameObject.SetActive(false);
		coinImage.gameObject.SetActive(false);
		pizzaImage.gameObject.SetActive(true);
		DisableImage(timeWarpImage);
		pizzaCount.text = amount.ToString();
		type = WheelCellType.Pizza;
	}

	public void SaveReward()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		switch (type)
		{
		case WheelCellType.Booster:
			DataLoader.Instance.BuyBoosters(boosterType, rewardAmount);
			dictionary.Add("Booster", boosterType.ToString());
			dictionary.Add("Count", rewardAmount.ToString());
			break;
		case WheelCellType.Coin:
			DataLoader.Instance.RefreshMoney(rewardAmount);
			dictionary.Add("Money", rewardAmount.ToString());
			break;
		case WheelCellType.Pizza:
			DataLoader.Instance.RefreshGems(rewardAmount);
			break;
		case WheelCellType.TimeWarp:
			DataLoader.gui.popUpsPanel.timeWarp.Open(rewardAmount, true);
			break;
		}
		if (GameManager.instance.currentGameMode != 0)
		{
			AnalyticsManager.instance.LogEvent("ClaimedRewardFromWheelOfFortune", dictionary);
		}
		DataLoader.Instance.SavePlayerData();
	}

	public void DisableImage(Image image)
	{
		if (image != null)
		{
			image.gameObject.SetActive(false);
		}
	}
}
