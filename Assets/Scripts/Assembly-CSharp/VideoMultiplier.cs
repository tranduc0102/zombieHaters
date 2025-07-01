using System;
using System.Collections;
using System.Collections.Generic;
using ACEPlay.Bridge;
using GuiInGame;
using IAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VideoMultiplier : MonoBehaviour
{
	public GameObject infinityTimerImage;

	public GameObject infinityTimerGreen;

	public GameObject imageDone;

	public Text timeText;

	public Text textInfinityPrice;

	public Button buyButton;

	public Button rewardButton;

	public Image imageRewardButton;

	public Image imageRewardVideo;

	[SerializeField]
	private ParticleSystem prefabFX;

	public GameObject redCircle;

	[SerializeField]
	private Text staticTimeText;

	public GameObject priceLoader;

	private Coroutine priceCor;

	[SerializeField]
	private Text bonusPercentageText;

	[SerializeField]
	private Text menuBonusPercentageText;

	public Text menuTimeLeftText;

	public Text popupTimeLeftText;

	public void Start()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(StaticConstants.MultiplierDurationInSeconds);
		staticTimeText.text = string.Format("+{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
		UpdateContent();
	}

	public void UpdatePercentage()
	{
		int num = (int)((ArenaManager.instance.GetCurrentBonus - 1f) * 100f);
		if (DataLoader.Instance.moneyMultiplier > 1)
		{
			num += 100;
		}
		Text text = bonusPercentageText;
		string text2 = "+" + num + "%";
		menuBonusPercentageText.text = text2;
		text.text = text2;
	}

	public void SetInfinityPurchased()
	{
		imageDone.SetActive(true);
		textInfinityPrice.gameObject.SetActive(false);
		bool flag = DataLoader.Instance.moneyMultiplier <= 2;
		rewardButton.interactable = !flag;
		infinityTimerGreen.SetActive(flag);
		infinityTimerImage.SetActive(flag);
		timeText.gameObject.SetActive(!flag);
		rewardButton.interactable = !flag;
		ButtonColor(flag);
		menuTimeLeftText.gameObject.SetActive(!flag);
		DataLoader.gui.multiplierAnim.SetBool("IsOpened", true);
		DataLoader.gui.videoAnim.SetBool("IsOpened", false);
	}

	public void ButtonColor(bool inactive)
	{
		if (inactive)
		{
			imageRewardButton.sprite = UIController.instance.multiplyImages.rewardedButtons[1].rewardedButton;
			imageRewardVideo.sprite = UIController.instance.multiplyImages.rewardedButtons[1].videoImage;
		}
		else
		{
			imageRewardButton.sprite = UIController.instance.multiplyImages.rewardedButtons[0].rewardedButton;
			imageRewardVideo.sprite = UIController.instance.multiplyImages.rewardedButtons[0].videoImage;
		}
	}

	public void Reward()
	{
		UnityEvent onDone = new UnityEvent();
		onDone.AddListener(() =>
		{
			StartCoroutine(DelayedGetReward());
		});
		BridgeController.instance.ShowRewarded($"reward: {AdName.RewardMulriplierTime}", onDone);
	}

	public void BuyInfinityMultiplier()
	{
		InAppManager.Instance.BuyProductID(InAppManager.Instance.infinityMultiplier.index);
	}

	private IEnumerator DelayedGetReward()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		DataLoader.Instance.SetNewMultiplier((DataLoader.Instance.moneyMultiplier != 1) ? DataLoader.Instance.moneyMultiplier : 2, StaticConstants.MultiplierDurationInSeconds);
		UpdatePercentage();
		prefabFX.Play();
		AnalyticsManager.instance.LogEvent("IncreaseMultiplierTimeVideo", new Dictionary<string, string> { 
		{
			"CurrentMultiplier",
			DataLoader.Instance.moneyMultiplier.ToString()
		} });
	}

	public void UpdateContent()
	{
		bool flag = PlayerPrefs.HasKey(StaticConstants.infinityMultiplierPurchased);
		textInfinityPrice.gameObject.SetActive(!flag);
		buyButton.interactable = !flag;
		ButtonColor(DataLoader.Instance.moneyMultiplier <= 2 && flag);
		if (!flag)
		{
			UIController.instance.StartGetPrice(textInfinityPrice, priceLoader, InAppManager.Instance.infinityMultiplier, ref priceCor);
		}
		imageDone.SetActive(flag);
		UpdatePercentage();
	}
}
