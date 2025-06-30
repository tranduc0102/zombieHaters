using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WheelOfFortune : UIPresent
{
	[SerializeField]
	private RectTransform wheelCircle;

	[SerializeField]
	private Image timer;

	[SerializeField]
	private float showTime;

	[SerializeField]
	private UIWheelCell[] cells;

	[SerializeField]
	private GameObject claim;

	[SerializeField]
	private GameObject spinSkip;

	[SerializeField]
	private GameObject stopSpinObj;

	private readonly int cellsCount = 6;

	[HideInInspector]
	public bool spinning;

	[SerializeField]
	private List<UIDailyWheel.Reward> rewardWeights;

	private int maxHitsInRow = 2;

	private int selectedCellIndex;

	private int spinCount;

	private int targetRotation;

	private Coroutine spinCor;

	private Coroutine timerCor;

	public void OnEnable()
	{
		timerCor = StartCoroutine(TimerFill());
	}

	private IEnumerator TimerFill()
	{
		timer.gameObject.SetActive(true);
		timer.fillAmount = 1f;
		while (timer.fillAmount > 0f)
		{
			timer.fillAmount -= Time.unscaledDeltaTime / showTime;
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime / 4f);
		}
		Skip();
	}

	public override void SetContent(int money)
	{
		base.SetContent(money);
		StopSimulation();
		base.gameObject.SetActive(true);
		spinSkip.SetActive(true);
		claim.SetActive(false);
		spinCount = 0;
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		list.Add(5);
		List<int> list2 = list;
		int index = Random.Range(0, list2.Count);
		int pizza = rewardWeights.First((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.SmallGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(pizza);
		list2.Remove(list2[index]);
		index = Random.Range(0, list2.Count);
		pizza = rewardWeights.First((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.MidGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(pizza);
		list2.Remove(list2[index]);
		index = Random.Range(0, list2.Count);
		pizza = rewardWeights.First((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.Booster).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetBoosters(pizza, SaveData.BoostersData.BoosterType.NewSurvivor);
		list2.Remove(list2[index]);
		index = Random.Range(0, list2.Count);
		pizza = rewardWeights.Last((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.Booster).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetBoosters(pizza, SaveData.BoostersData.BoosterType.KillAll);
		list2.Remove(list2[index]);
		index = Random.Range(0, list2.Count);
		pizza = rewardWeights.First((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.Money).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetMoney(money * pizza);
		list2.Remove(list2[index]);
		index = Random.Range(0, list2.Count);
		pizza = rewardWeights.First((UIDailyWheel.Reward item) => item.rewardType == UIDailyWheel.RewardType.BigGems).SetIndexGetReward(list2[index]);
		cells[list2[index]].SetPizza(pizza);
		list2.Remove(list2[index]);
		stopSpinObj.SetActive(false);
	}

	public UIDailyWheel.Reward ChooseReward()
	{
		List<UIDailyWheel.Reward> list = new List<UIDailyWheel.Reward>();
		for (int i = 0; i < rewardWeights.Count; i++)
		{
			if (rewardWeights[i].hitsInRow < maxHitsInRow)
			{
				list.Add(rewardWeights[i]);
				continue;
			}
			UIDailyWheel.Reward reward = rewardWeights[i];
			reward.hitsInRow = 0;
			rewardWeights[i] = reward;
		}
		int max = list.Sum((UIDailyWheel.Reward c) => c.weight);
		int num = Random.Range(0, max);
		int num2 = 0;
		for (int j = 0; j < list.Count; j++)
		{
			for (int k = num2; k < list[j].weight + num2; k++)
			{
				if (k < num)
				{
					continue;
				}
				UIDailyWheel.Reward reward = list[j];
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

	public override string GetSkipEventName()
	{
		return "WheelOfFortuneSkipped";
	}

	public override void OnEscape()
	{
		if (spinCount > 0)
		{
			if (spinning)
			{
				StopSpin();
			}
			else
			{
				GetReward();
			}
		}
		else
		{
			Skip();
		}
	}

	public void Spin()
	{
		StopSimulation();
		StopCoroutine(timerCor);
        StartCoroutine(InvokeSpin());

    /*    AdsManager.instance.ShowRewarded(delegate
		{
		}, AdsManager.AdName.RewardSpin);*/
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause && spinning)
		{
			StopSpin();
		}
	}

	public IEnumerator InvokeSpin()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		timer.gameObject.SetActive(false);
		stopSpinObj.SetActive(true);
		selectedCellIndex = ChooseReward().index;
		spinCor = StartCoroutine(Spinning(selectedCellIndex));
		spinCount++;
		spinSkip.SetActive(false);
		claim.SetActive(false);
		//AdsManager.instance.DecreaseInterstitialCounter();
		SoundManager.Instance.PlaySound(rewardClip, 1f);
		AnalyticsManager.instance.LogEvent("Spin", new Dictionary<string, string> { 
		{
			"SpinsInRow",
			spinCount.ToString()
		} });
	}

	private IEnumerator Spinning(int targetCell)
	{
		targetRotation = Random.Range(4, 6) * 360 + 360 / cellsCount * targetCell;
		float currentRotation = wheelCircle.eulerAngles.z;
		spinning = true;
		while (currentRotation < (float)targetRotation)
		{
			currentRotation += Mathf.Clamp((float)targetRotation - currentRotation, 5f, 360f) * Time.unscaledDeltaTime * 2f;
			wheelCircle.eulerAngles = new Vector3(wheelCircle.eulerAngles.x, wheelCircle.eulerAngles.y, currentRotation);
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime / 2f);
		}
		spinning = false;
		claim.SetActive(true);
		stopSpinObj.SetActive(false);
		StartSimulation();
	}

	private IEnumerator StopCellRotation()
	{
		while (spinning)
		{
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i].rectTransform.rotation = Quaternion.identity;
			}
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
		}
	}

	public void GetReward()
	{
		cells[selectedCellIndex].SaveReward();
		GoToGameOver();
	}

	public void StopSpin()
	{
		StartSimulation();
		wheelCircle.eulerAngles = new Vector3(wheelCircle.eulerAngles.x, wheelCircle.eulerAngles.y, targetRotation);
		StopCoroutine(spinCor);
		stopSpinObj.SetActive(false);
		claim.SetActive(true);
		spinning = false;
	}

	public void ClosePopup()
	{
		if (spinCount > 0)
		{
			StopSpin();
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
