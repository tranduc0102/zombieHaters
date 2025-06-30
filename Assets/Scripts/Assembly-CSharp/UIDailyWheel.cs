using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyWheel : MonoBehaviour
{
	public enum SpinType
	{
		Free = 0,
		Sub = 1,
		Pizza = 2
	}

	public enum RewardType
	{
		Booster = 0,
		BigGems = 1,
		SmallGems = 2,
		MidGems = 3,
		TimeWarp = 4,
		Money = 5,
		BigMoney = 6
	}

	[Serializable]
	public class Reward
	{
		public RewardType rewardType;

		public int weight;

		public int reward;

		[NonSerialized]
		public int index;

		[NonSerialized]
		public int hitsInRow;

		public int SetIndexGetReward(int index)
		{
			this.index = index;
			return reward;
		}
	}

	[SerializeField]
	private UIWheelCell[] cells;

	[SerializeField]
	private UIWheelCell smallCell;

	[SerializeField]
	private RectTransform wheelCircle;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private Image prizeImg;

	[SerializeField]
	private Image countBg;

	[SerializeField]
	private Text prizeCountText;

	[SerializeField]
	private Text subBonusText;

	[SerializeField]
	private Button btnSpin;

	[SerializeField]
	private CanvasGroup btnSpinCanvasGroup;

	[SerializeField]
	private Text spinText;

	[SerializeField]
	private Text pizzaSpinText;

	[SerializeField]
	private Button btnOk;

	[SerializeField]
	private Button btnGetSub;

	[SerializeField]
	private CanvasGroup okSubCanvasGroup;

	[SerializeField]
	private Text pizzaSpinTextCount;

	[SerializeField]
	private GameObject askAboutNotificationsPanel;

	[SerializeField]
	private GameObject dailyCloseBg;

	[SerializeField]
	private Button buttonYes;

	[SerializeField]
	private List<Reward> rewardWeights;

	[SerializeField]
	private Image lightSegment;

	[SerializeField]
	private ParticleSystem wheelFx;

	private int maxHitsInRow = 2;

	private int selectedCellIndex;

	private bool spinning;

	private bool subEnabled;

	private SpinType spinType;

	public int spinPrice = 25;

	private int spinCount;

	private void Start()
	{
		btnOk.onClick.AddListener(ClosePopup);
		btnGetSub.onClick.AddListener(GetSub);
	}

	private IEnumerator LightSegmentAnimation()
	{
		lightSegment.rectTransform.localRotation = Quaternion.identity;
		lightSegment.gameObject.SetActive(true);
		while (!spinning)
		{
			lightSegment.rectTransform.Rotate(new Vector3(0f, 0f, 60f));
			yield return new WaitForSeconds(0.07f);
		}
		lightSegment.gameObject.SetActive(false);
	}

	public void RefreshContent()
	{
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		list.Add(5);
		List<int> list2 = list;
		int index = UnityEngine.Random.Range(0, list2.Count);
		int timeWarp = rewardWeights.First((Reward item) => item.rewardType == RewardType.TimeWarp).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetTimeWarp(timeWarp);
		list2.Remove(list2[index]);
		index = UnityEngine.Random.Range(0, list2.Count);
		timeWarp = rewardWeights.First((Reward item) => item.rewardType == RewardType.MidGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(timeWarp);
		list2.Remove(list2[index]);
		index = UnityEngine.Random.Range(0, list2.Count);
		timeWarp = rewardWeights.First((Reward item) => item.rewardType == RewardType.Booster).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetBoosters(timeWarp, SaveData.BoostersData.BoosterType.NewSurvivor);
		list2.Remove(list2[index]);
		index = UnityEngine.Random.Range(0, list2.Count);
		timeWarp = rewardWeights.Last((Reward item) => item.rewardType == RewardType.Booster).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetBoosters(timeWarp, SaveData.BoostersData.BoosterType.KillAll);
		list2.Remove(list2[index]);
		index = UnityEngine.Random.Range(0, list2.Count);
		timeWarp = rewardWeights.First((Reward item) => item.rewardType == RewardType.SmallGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(timeWarp);
		list2.Remove(list2[index]);
		index = UnityEngine.Random.Range(0, list2.Count);
		timeWarp = rewardWeights.First((Reward item) => item.rewardType == RewardType.BigGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(timeWarp);
		list2.Remove(list2[index]);
	}

	public Reward ChooseReward()
	{
		List<Reward> list = new List<Reward>();
		for (int i = 0; i < rewardWeights.Count; i++)
		{
			if (rewardWeights[i].hitsInRow < maxHitsInRow)
			{
				list.Add(rewardWeights[i]);
				continue;
			}
			Reward reward = rewardWeights[i];
			reward.hitsInRow = 0;
			rewardWeights[i] = reward;
		}
		int max = list.Sum((Reward c) => c.weight);
		int num = UnityEngine.Random.Range(0, max);
		int num2 = 0;
		for (int j = 0; j < list.Count; j++)
		{
			for (int k = num2; k < list[j].weight + num2; k++)
			{
				if (k < num)
				{
					continue;
				}
				Reward reward = list[j];
				reward.hitsInRow++;
				for (int l = 0; l < rewardWeights.Count; l++)
				{
					if (rewardWeights[l].rewardType == list[j].rewardType)
					{
						rewardWeights[l] = reward;
					}
				}
				return list[j];
			}
			num2 += list[j].weight;
		}
		Debug.LogWarning("Something Wrong With Present Weights");
		return rewardWeights.First();
	}

	private void OnEnable()
	{
		wheelFx.Stop();
		spinCount = 0;
		spinning = false;
		StartCoroutine(LightSegmentAnimation());
		dailyCloseBg.SetActive(false);
		askAboutNotificationsPanel.SetActive(false);
		StartCoroutine(UIController.instance.Scale(base.transform));
		spinType = GetSpinType();
		UpdateSpinButton();
		anim.Play("DailyWheelReset");
		btnSpin.interactable = true;
		btnSpin.image.rectTransform.anchoredPosition = Vector2.zero;
		btnOk.gameObject.SetActive(false);
		btnGetSub.gameObject.SetActive(false);
		btnSpinCanvasGroup.alpha = 1f;
		okSubCanvasGroup.alpha = 0f;
		if (DataLoader.playerData.gems >= spinPrice)
		{
			pizzaSpinTextCount.color = Color.white;
		}
		else
		{
			pizzaSpinTextCount.color = Color.red;
		}
		RefreshContent();
	}

	public void ShowNotificationAskPanel()
	{
		if (spinCount > 0)
		{
			SaveReward();
			if (smallCell.type == WheelCellType.TimeWarp)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
			}
		}
		else
		{
			DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
		}
	}

	private void SaveReward()
	{
		smallCell.SaveReward();
		if (subEnabled)
		{
			DataLoader.Instance.RefreshGems(2);
		}
		AnalyticsManager.instance.LogEvent("DailyWheelSaveReward", new Dictionary<string, string>
		{
			{
				"RewardType",
				smallCell.type.ToString()
			},
			{
				"Reward",
				smallCell.rewardAmount.ToString()
			},
			{
				"IsSubscribed",
				subEnabled.ToString()
			}
		});
	}

	public void GoToShop()
	{
		DataLoader.gui.popUpsPanel.shop.gameObject.SetActive(true);
		DataLoader.gui.popUpsPanel.shop.ScrollToCoins();
		base.gameObject.SetActive(false);
	}

	public void Spin()
	{
		if (spinning)
		{
			return;
		}
		if ((spinType == SpinType.Pizza || spinType == SpinType.Sub) && !DataLoader.Instance.RefreshGems(-spinPrice, true))
		{
			GoToShop();
			return;
		}
		if (spinType == SpinType.Sub)
		{
			spinType = SpinType.Pizza;
		}
		AnalyticsManager.instance.LogEvent("DailyWheelSpin", new Dictionary<string, string> { 
		{
			"SpinType",
			spinType.ToString()
		} });
		selectedCellIndex = ChooseReward().index;
		switch (cells[selectedCellIndex].type)
		{
		case WheelCellType.Booster:
			smallCell.SetBoosters(cells[selectedCellIndex].rewardAmount, cells[selectedCellIndex].boosterType);
			break;
		case WheelCellType.Coin:
			smallCell.SetMoney(cells[selectedCellIndex].rewardAmount);
			break;
		case WheelCellType.Pizza:
			smallCell.SetPizza(cells[selectedCellIndex].rewardAmount);
			break;
		case WheelCellType.TimeWarp:
			smallCell.SetTimeWarp(cells[selectedCellIndex].rewardAmount);
			break;
		}
		if (spinType == SpinType.Free)
		{
			PlayerPrefs.SetString(StaticConstants.DailyRewardKey, TimeManager.CurrentDateTime.Date.Ticks.ToString());
			DataLoader.Instance.SetTotalDays(DataLoader.playerData.totalDaysInRow + 1);
		}
		else if (spinType == SpinType.Sub)
		{
			PlayerPrefs.SetString(StaticConstants.SubscriptionRewardkey, TimeManager.CurrentDateTime.Date.Ticks.ToString());
		}
		StartCoroutine(Spinning(selectedCellIndex));
	}

	private IEnumerator Spinning(int targetCell)
	{
		if (!spinning)
		{
			wheelFx.Play();
			spinCount++;
			int targetRotation = UnityEngine.Random.Range(4, 6) * 360 + 360 / cells.Length * targetCell;
			float currentRotation = wheelCircle.eulerAngles.z;
			spinning = true;
			StartCoroutine(HideSpinButton());
			btnSpin.interactable = false;
			while (currentRotation < (float)targetRotation)
			{
				currentRotation += Mathf.Clamp((float)targetRotation - currentRotation, 5f, 360f) * Time.unscaledDeltaTime * 2f;
				wheelCircle.eulerAngles = new Vector3(wheelCircle.eulerAngles.x, wheelCircle.eulerAngles.y, currentRotation);
				yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime / 2f);
			}
			spinning = false;
			anim.Play("SpinCompleted");
			StartCoroutine(ShowAfterSpinButtons());
			if (subEnabled)
			{
				prizeImg.color = Color.white;
				countBg.color = Color.white;
				subBonusText.color = new Color(1f / 15f, 0.3254902f, 39f / 85f, 1f);
				prizeCountText.color = Color.white;
			}
			else
			{
				prizeImg.color = new Color(1f, 1f, 1f, 0.5f);
				countBg.color = new Color(1f, 1f, 1f, 0.5f);
				subBonusText.color = new Color(47f / 85f, 74f / 85f, 14f / 15f, 1f);
				prizeCountText.color = subBonusText.color;
			}
			wheelFx.Stop();
		}
	}

	private IEnumerator HideSpinButton()
	{
		btnSpin.interactable = false;
		while (btnSpinCanvasGroup.alpha > 0f)
		{
			btnSpinCanvasGroup.alpha -= Time.deltaTime * 4.5f;
			btnSpin.image.rectTransform.anchoredPosition = new Vector3(btnSpin.image.rectTransform.anchoredPosition.x, btnSpin.image.rectTransform.anchoredPosition.y - Time.deltaTime * 125f);
			yield return null;
		}
	}

	private IEnumerator ShowAfterSpinButtons()
	{
		yield return new WaitForSeconds(1f);
		btnOk.gameObject.SetActive(true);
		btnOk.image.rectTransform.anchoredPosition = ((!subEnabled) ? new Vector2(-250f, 0f) : Vector2.zero);
		btnGetSub.gameObject.SetActive(!subEnabled);
		while (okSubCanvasGroup.alpha < 1f)
		{
			okSubCanvasGroup.alpha += Time.deltaTime * 4.5f;
			yield return null;
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void GetSub()
	{
		DataLoader.gui.popUpsPanel.gameObject.SetActive(true);
		base.gameObject.SetActive(false);
		DataLoader.gui.popUpsPanel.subscription.gameObject.SetActive(true);
	}

	public void ClosePopup()
	{
		if (!spinning)
		{
			ShowNotificationAskPanel();
		}
	}

	public void UpdateSpinButton()
	{
		if (spinType == SpinType.Pizza || spinType == SpinType.Sub)
		{
			pizzaSpinText.gameObject.SetActive(true);
			spinText.gameObject.SetActive(false);
		}
		else
		{
			pizzaSpinText.gameObject.SetActive(false);
			spinText.gameObject.SetActive(true);
		}
	}

	private SpinType GetSpinType()
	{
		try
		{
			if (!subEnabled)
			{
				subEnabled = InAppManager.Instance.IsSubscribed();
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
		if (PlayerPrefs.HasKey(StaticConstants.DailyRewardKey))
		{
			DateTime value = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.DailyRewardKey)), DateTimeKind.Utc);
			int num = (int)TimeManager.CurrentDateTime.Subtract(value).TotalDays;
			if (num >= DataLoader.playerData.totalDaysInRow + 1)
			{
				Debug.Log("Free spin");
				return SpinType.Free;
			}
			if (num == DataLoader.playerData.totalDaysInRow)
			{
				Debug.Log("Free spin completed");
				if (subEnabled)
				{
					if (PlayerPrefs.HasKey(StaticConstants.SubscriptionRewardkey))
					{
						value = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.SubscriptionRewardkey)), DateTimeKind.Utc);
						num = (int)TimeManager.CurrentDateTime.Subtract(value).TotalDays;
						if (num >= DataLoader.playerData.totalDaysInRow)
						{
							Debug.Log("Sub spin");
							return SpinType.Sub;
						}
						Debug.Log("Sub spin Completed");
						return SpinType.Pizza;
					}
					return SpinType.Sub;
				}
				Debug.Log("Sub spin Completed");
				return SpinType.Pizza;
			}
			return SpinType.Pizza;
		}
		Debug.Log("First enter. Free spin");
		return SpinType.Free;
	}
}
