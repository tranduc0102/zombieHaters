using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoosterPresent : UIPresent
{
	[SerializeField]
	private GameObject openSkip;

	[SerializeField]
	private GameObject claim;

	[SerializeField]
	private GameObject boosterPlashka;

	[SerializeField]
	private RectTransform boxCap;

	[SerializeField]
	private Image newBoosterImage;

	[SerializeField]
	private Text lbl_newSurvAmount;

	[SerializeField]
	private Text lbl_killAllAmount;

	[SerializeField]
	private Text lbl_winAmount;

	[SerializeField]
	private int boostersAmount;

	private SaveData.BoostersData.BoosterType currentBooster;

	private void Start()
	{
		lbl_newSurvAmount.text = boostersAmount.ToString();
		lbl_killAllAmount.text = boostersAmount.ToString();
		lbl_winAmount.text = boostersAmount.ToString();
	}

	public override void SetContent(int money)
	{
		base.SetContent(money);
		ResetButtons();
		StopSimulation();
		base.gameObject.SetActive(true);
		openSkip.SetActive(true);
		claim.SetActive(false);
		newBoosterImage.gameObject.SetActive(false);
		boosterPlashka.SetActive(true);
		boxCap.anchoredPosition = new Vector2(0f, 200f);
		currentBooster = GetRandomBooster();
	}

	public override string GetSkipEventName()
	{
		return "BoosterPresentSkipped";
	}

	public void Open()
	{
        StartCoroutine(InvokeOpen());

      /*  AdsManager.instance.ShowRewarded(delegate
		{
		}, AdsManager.AdName.RewardPresentBox);*/
	}

	public IEnumerator InvokeOpen()
	{
		DataLoader.Instance.BuyBoosters(currentBooster, boostersAmount);
		yield return new WaitForSecondsRealtime(0.3f);
		openSkip.SetActive(false);
		claim.SetActive(true);
		boosterPlashka.SetActive(false);
		newBoosterImage.gameObject.SetActive(true);
		boxCap.anchoredPosition = new Vector2(100f, -224f);
/*		AdsManager.instance.DecreaseInterstitialCounter();
*/		StartSimulation();
		SoundManager.Instance.PlaySound(rewardClip, 1f);
		AnalyticsManager.instance.LogEvent("BoosterPresentOpenned", new Dictionary<string, string> { 
		{
			"RewardBoosterType",
			currentBooster.ToString()
		} });
	}

	public void Claim()
	{
		GoToGameOver();
	}

	public SaveData.BoostersData.BoosterType GetRandomBooster()
	{
		switch (Random.Range(0, 2))
		{
		case 0:
			newBoosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[0];
			return SaveData.BoostersData.BoosterType.NewSurvivor;
		case 1:
			newBoosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[1];
			return SaveData.BoostersData.BoosterType.KillAll;
		default:
			return SaveData.BoostersData.BoosterType.NewSurvivor;
		}
	}
}
