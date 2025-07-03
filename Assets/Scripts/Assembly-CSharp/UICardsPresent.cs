using System;
using System.Collections;
using System.Collections.Generic;
using ACEPlay.Bridge;
using GuiInGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICardsPresent : UIPresent
{
	public enum RewardType
	{
		Booster = 0,
		Money = 1,
		MoreMoney = 2,
		Pizza = 3
	}

	[Serializable]
	public struct Reward
	{
		public RewardType rewardType;

		public int weight;

		[Header("Set here count of busters or money multiplier")]
		public float value;

		[NonSerialized]
		public GameObject content;
	}

	[SerializeField]
	private Text headText;

	[SerializeField]
	private Animator animatorCardsShake;

	[SerializeField]
	private Animator animatorHand;

	[Space]
	[SerializeField]
	private Image prefabBoosterNewSurvivor;

	[SerializeField]
	private Image prefabBoosterKillAll;

	[SerializeField]
	private Image prefabCoins;

	[SerializeField]
	private Image prefabPizza;

	[Space]
	[SerializeField]
	private List<Reward> rewards;

	[Space]
	[SerializeField]
	private Card[] cards;

	[Space]
	[SerializeField]
	private float previewTime = 2f;

	[SerializeField]
	private float showHideSpeed = 2f;

	private SaveData.BoostersData.BoosterType currentBooster;

	private bool freeCardOpened;

	private List<Reward> rewardsInCards;

	private int money;

	public override void SetContent(int money)
	{
		base.SetContent(money);
		this.money = money;
		freeCardOpened = false;
		headText.text = string.Empty;
		base.gameObject.SetActive(true);
		for (int i = 0; i < rewards.Count; i++)
		{
			Reward value = rewards[i];
			if (value.content != null)
			{
				UnityEngine.Object.Destroy(value.content);
			}
			if (rewards[i].rewardType == RewardType.Booster)
			{
				switch (UnityEngine.Random.Range(0, 2))
				{
				case 0:
					value.content = UnityEngine.Object.Instantiate(prefabBoosterKillAll, cards[i].transform).gameObject;
					currentBooster = SaveData.BoostersData.BoosterType.KillAll;
					break;
				default:
					value.content = UnityEngine.Object.Instantiate(prefabBoosterNewSurvivor, cards[i].transform).gameObject;
					currentBooster = SaveData.BoostersData.BoosterType.NewSurvivor;
					break;
				}
				value.content.GetComponentInChildren<Text>().text = value.value.ToString();
			}
			else if (rewards[i].rewardType == RewardType.Pizza)
			{
				value.content = UnityEngine.Object.Instantiate(prefabPizza, cards[i].transform).gameObject;
				value.content.GetComponentInChildren<Text>().text = ((int)value.value).ToString();
			}
			else
			{
				value.content = UnityEngine.Object.Instantiate(prefabCoins, cards[i].transform).gameObject;
				value.content.GetComponentInChildren<Text>().text = ((int)((float)money * value.value)).ToString();
			}
			rewards[i] = value;
		}
		rewardsInCards = new List<Reward>(rewards);
		Card[] array = cards;
		foreach (Card card in array)
		{
			int index = UnityEngine.Random.Range(0, rewardsInCards.Count);
			card.content = rewardsInCards[index].content;
			rewardsInCards[index].content.transform.SetParent(card.transform, false);
			rewardsInCards[index].content.transform.localPosition = Vector3.zero;
			rewardsInCards.RemoveAt(index);
			card.IsVisible(true);
			card.glowFx.Stop();
		}
		rewardsInCards = new List<Reward>(rewards);
		animatorCardsShake.enabled = false;
	}

	private void OnEnable()
	{
		StartCoroutine(hideCards());
	}

	private IEnumerator hideCards()
	{
		yield return new WaitForSecondsRealtime(previewTime);
		Card[] array = cards;
		foreach (Card card in array)
		{
			float directionStep = 0f - showHideSpeed;
			float defaultScale = card.transform.localScale.x;
			do
			{
				card.transform.localScale = new Vector3(card.transform.localScale.x + directionStep * 0.02f, card.transform.localScale.y, card.transform.localScale.z);
				if (card.transform.localScale.x <= 0f)
				{
					directionStep = showHideSpeed;
					card.IsVisible(false);
				}
				yield return new WaitForSecondsRealtime(0.02f);
			}
			while (card.transform.localScale.x < defaultScale);
			card.transform.localScale = new Vector3(defaultScale, card.transform.localScale.y, card.transform.localScale.z);
		}
		headText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Open_the_card);
		animatorCardsShake.enabled = true;
		StartCoroutine(WaitAnimation(animatorCardsShake.GetCurrentAnimatorStateInfo(0).length));
	}

	private IEnumerator WaitAnimation(float time)
	{
		yield return new WaitForSecondsRealtime(time);
		CardsReadyToOpen();
	}

	public override string GetSkipEventName()
	{
		return "CardPresentSkipped";
	}

	public void Open(Card card)
	{
		card.Interactable(false);
		if (!freeCardOpened)
		{
			StartCoroutine(InvokeOpen(card));
			freeCardOpened = true;
		}
		else
		{
			UnityEvent onDone = new UnityEvent();
			onDone.AddListener(delegate { StartCoroutine(InvokeOpen(card)); });
			BridgeController.instance.ShowRewarded($"Show rewarded{AdName.RewardOpenCard}", onDone);
           /* AdsManager.instance.ShowRewarded(delegate
			{
				
			}, AdsManager.AdName.RewardOpenCard);*/
		}
	}

	public IEnumerator InvokeOpen(Card card)
	{
		if (freeCardOpened)
		{
			DataLoader.gui.DecreaseInterstitialCounter();
		}
		else
		{
			animatorHand.gameObject.SetActive(false);
			for (int i = 0; i < cards.Length; i++)
			{
				if (cards[i] != card)
				{
					cards[i].imageVideo.enabled = true;
				}
			}
		}
		int currentRewardNum = 0;
		int max = 0;
		foreach (Reward rewardsInCard in rewardsInCards)
		{
			max += rewardsInCard.weight;
		}
		int randomValue = UnityEngine.Random.Range(0, max);
		int ran = 0;
		for (int j = 0; j < rewardsInCards.Count; j++)
		{
			ran += rewardsInCards[j].weight;
			if (randomValue < ran)
			{
				card.content = rewardsInCards[j].content;
				card.content.transform.SetParent(card.transform, false);
				card.content.transform.localPosition = Vector3.zero;
				currentRewardNum = j;
				break;
			}
		}
		switch (rewardsInCards[currentRewardNum].rewardType)
		{
		case RewardType.Booster:
			DataLoader.Instance.BuyBoosters(currentBooster, (int)rewardsInCards[currentRewardNum].value);
			break;
		default:
			DataLoader.Instance.RefreshMoney((float)money * rewardsInCards[currentRewardNum].value, true);
			break;
		case RewardType.Pizza:
			DataLoader.Instance.RefreshGems((int)rewardsInCards[currentRewardNum].value, true);
			break;
		}
		AnalyticsManager.instance.LogEvent("CardPresentOpenned", new Dictionary<string, string>
		{
			{
				"RewardType",
				rewardsInCards[currentRewardNum].rewardType.ToString()
			},
			{
				"ForVideoAd",
				freeCardOpened.ToString()
			}
		});
		rewardsInCards.RemoveAt(currentRewardNum);
		SoundManager.Instance.PlaySound(rewardClip, 1f);
		float directionStep = 0f - showHideSpeed;
		float defaultScale = card.transform.localScale.x;
		do
		{
			card.transform.localScale = new Vector3(card.transform.localScale.x + directionStep * 0.02f, card.transform.localScale.y, card.transform.localScale.z);
			if (card.transform.localScale.x <= 0f)
			{
				directionStep = showHideSpeed;
				card.IsVisible(true);
			}
			yield return new WaitForSecondsRealtime(0.02f);
		}
		while (card.transform.localScale.x < defaultScale);
		card.transform.localScale = new Vector3(defaultScale, card.transform.localScale.y, card.transform.localScale.z);
		card.glowFx.Play();
		yield return new WaitForSecondsRealtime(0.3f);
		if (rewardsInCards.Count <= 0)
		{
			headText.text = string.Empty;
		}
		else if (rewardsInCards.Count < rewards.Count)
		{
			headText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Open_another_card);
		}
		yield return new WaitForSecondsRealtime(delayBeforeShowSkip);
		StartCoroutine(MakeSkipVisible());
	}

	public void CardsReadyToOpen()
	{
		Card[] array = cards;
		foreach (Card card in array)
		{
			card.Interactable(true);
		}
		animatorHand.gameObject.SetActive(true);
		animatorHand.SetTrigger("ChooseCard");
	}

	public void Claim()
	{
		GoToGameOver();
	}

	protected override void OnPresentLoaded()
	{
	}

	protected override void ResetButtons()
	{
		skipBtn.image.color = new Color(skipBtn.image.color.r, skipBtn.image.color.g, skipBtn.image.color.b, 0f);
		skipText.color = new Color(1f, 1f, 1f, 0f);
		skipBtn.interactable = false;
	}
}
