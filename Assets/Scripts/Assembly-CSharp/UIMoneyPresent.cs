using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoneyPresent : UIPresent
{
	[SerializeField]
	private Text moneyText;

	private int money;

	public override void SetContent(int money)
	{
		base.SetContent(money);
		base.gameObject.SetActive(true);
		this.money = (int)((float)money * 0.75f);
		moneyText.text = this.money.ToString();
		ResetButtons();
	}

	public void OnEnable()
	{
		StartSimulation();
	}

	public override string GetSkipEventName()
	{
		return "MoneyPresentSkipped";
	}

	public void DoubleMoney()
	{
        StartCoroutine(InvokeX2());

       /* AdsManager.instance.ShowRewarded(delegate
		{
		}, AdsManager.AdName.RewardX2CoinsPresent);*/
	}

	public IEnumerator InvokeX2()
	{
		videoBtn.gameObject.SetActive(false);
		skipBtn.image.color = new Color(skipBtn.image.color.r, skipBtn.image.color.g, skipBtn.image.color.b, 1f);
		skipText.color = Color.white;
		skipBtn.image.rectTransform.anchoredPosition = new Vector2(0f, skipBtn.image.rectTransform.anchoredPosition.y);
		yield return new WaitForSecondsRealtime(0.3f);
		money *= 2;
		moneyText.text = money.ToString();
/*		AdsManager.instance.DecreaseInterstitialCounter();
*/		SoundManager.Instance.PlaySound(rewardClip, 1f);
		AnalyticsManager.instance.LogEvent("MoneyPresentX2", new Dictionary<string, string>());
	}

	public void ClaimMoney()
	{
		DataLoader.Instance.RefreshMoney(money, true);
		GoToGameOver();
	}
}
