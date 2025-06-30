using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotEnoughMoney : MonoBehaviour
{
	[SerializeField]
	private Text lbl_coinsCount;

	[SerializeField]
	private Text lbl_gemsCount;

	[SerializeField]
	private Button btn_trade;

	private double addedMoney;

	private SurviviorContent survContent;

	private int tradeGemsCount;

	private void Start()
	{
		btn_trade.onClick.AddListener(Trade);
	}

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
		AnalyticsManager.instance.LogEvent("NotEnoughMoneyOpened", new Dictionary<string, string>());
	}

	public void Open(SurviviorContent survContent, double money)
	{
		this.survContent = survContent;
		addedMoney = money;
		survContent.heroInfo.gameObject.SetActive(false);
		DataLoader.gui.popUpsPanel.gameObject.SetActive(true);
		base.gameObject.SetActive(true);
		lbl_coinsCount.text = AbbreviationUtility.AbbreviateNumber(money);
		tradeGemsCount = CalculateGems(money);
		lbl_gemsCount.text = tradeGemsCount.ToString();
	}

	public int CalculateGems(double money)
	{
		return Mathf.CeilToInt((float)money / DataLoader.Instance.GetTimeWarpGoldCount());
	}

	public void Trade()
	{
		if (DataLoader.Instance.RefreshGems(-tradeGemsCount))
		{
			DataLoader.Instance.RefreshMoney(addedMoney);
			DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
			AnalyticsManager.instance.LogEvent("NotEnoughMoneyTrade", new Dictionary<string, string>
			{
				{
					"Money",
					addedMoney.ToString()
				},
				{
					"Gems",
					tradeGemsCount.ToString()
				}
			});
			survContent.Upgrade(false);
		}
		else
		{
			DataLoader.gui.popUpsPanel.shop.ScrollToCoins();
			DataLoader.gui.popUpsPanel.shop.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
			AnalyticsManager.instance.LogEvent("NotEnoughMoneyShop", new Dictionary<string, string>());
		}
	}
}
