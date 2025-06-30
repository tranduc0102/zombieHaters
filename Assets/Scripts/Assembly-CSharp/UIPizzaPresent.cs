using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPizzaPresent : UIPresent
{
	[SerializeField]
	private GameObject openSkip;

	[SerializeField]
	private GameObject claim;

	[SerializeField]
	private GameObject topPlashka;

	[SerializeField]
	private RectTransform boxCap;

	[SerializeField]
	private Image img_bigPizza;

	[SerializeField]
	private Text lbl_topPlashkaAmount;

	[SerializeField]
	private Text lbl_winAmount;

	[SerializeField]
	private int pizzaAmount;

	private void Start()
	{
		lbl_topPlashkaAmount.text = pizzaAmount.ToString();
		lbl_winAmount.text = pizzaAmount.ToString();
	}

	public override void SetContent(int money)
	{
		base.SetContent(money);
		ResetButtons();
		StopSimulation();
		base.gameObject.SetActive(true);
		openSkip.SetActive(true);
		claim.SetActive(false);
		img_bigPizza.gameObject.SetActive(false);
		topPlashka.SetActive(true);
		boxCap.anchoredPosition = new Vector2(0f, 200f);
	}

	public override string GetSkipEventName()
	{
		return "PizzaPresentSkipped";
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
		DataLoader.Instance.RefreshGems(pizzaAmount, true);
		yield return new WaitForSecondsRealtime(0.3f);
		openSkip.SetActive(false);
		claim.SetActive(true);
		topPlashka.SetActive(false);
		img_bigPizza.gameObject.SetActive(true);
		boxCap.anchoredPosition = new Vector2(100f, -224f);
/*		AdsManager.instance.DecreaseInterstitialCounter();
*/		StartSimulation();
		SoundManager.Instance.PlaySound(rewardClip, 1f);
		AnalyticsManager.instance.LogEvent("PizzaPresentOpenned", new Dictionary<string, string> { 
		{
			"Reward",
			pizzaAmount.ToString()
		} });
	}

	public void Claim()
	{
		GoToGameOver();
	}
}
