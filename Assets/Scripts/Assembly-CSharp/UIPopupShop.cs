using System;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupShop : MonoBehaviour
{
	public Text lbl_money;

	public Text lbl_noAdsPrice;

	private double lastShownGold;

	public UIShopCoinPack[] packs;

	public UiShopBooster[] boosters;

	[Space(10f)]
	public float scrollCoinsPosY = 840f;

	public RectTransform scrollRt;

	public GameObject noAdsPriceLoader;

	private Coroutine noAdsPriceCor;

	public ShopWarpInfo smallWarp;

	public ShopWarpInfo bigWarp;

	private void Start()
	{
		for (int i = 0; i < packs.Length; i++)
		{
			CoinsPurchaseInfo coinsPurchaseInfo = InAppManager.Instance.gemsPurchases[i];
			packs[i].lbl_amount.text = string.Format("{0:N0}", coinsPurchaseInfo.reward);
		}
	}

	private void OnEnable()
	{
		updateMoneyCounter();
		for (int i = 0; i < packs.Length; i++)
		{
			packs[i].GetPackPrice(InAppManager.Instance.GetCoinPackPurchase(i));
		}
		UpdateBoosters();
		//if (!NoAdsManager.instance.NoAdsPurchased())
		//{
		//	UIController.instance.StartGetPrice(lbl_noAdsPrice, noAdsPriceLoader, InAppManager.Instance.noAds, ref noAdsPriceCor);
		//}
		StartCoroutine(UIController.instance.Scale(base.transform));
		UpdateWarpInfo(smallWarp);
		UpdateWarpInfo(bigWarp);
	}

	public void UpdateWarpInfo(ShopWarpInfo warpInfo)
	{
		warpInfo.lbl_price.text = warpInfo.price.ToString();
		warpInfo.btn_buy.onClick.RemoveAllListeners();
		warpInfo.btn_buy.onClick.AddListener(delegate
		{
			if (DataLoader.Instance.RefreshGems(-warpInfo.price))
			{
				DataLoader.gui.popUpsPanel.timeWarp.Open(warpInfo.seconds);
			}
			else
			{
				ScrollToCoins();
			}
		});
	}

	private void updateMoneyCounter()
	{
		if (DataLoader.playerData != null)
		{
			lastShownGold = DataLoader.playerData.money;
			lbl_money.text = Math.Floor(DataLoader.playerData.money).ToString();
		}
	}

	private void Update()
	{
		if (lastShownGold != DataLoader.playerData.money)
		{
			updateMoneyCounter();
		}
	}

	public void OnPurchaseCoins(int packIndex)
	{
		InAppManager.Instance.BuyProductID(packIndex);
	}

	public void UpdateBoosters()
	{
		for (int i = 0; i < boosters.Length; i++)
		{
			boosters[i].updatePrice();
		}
	}

	public void ResetScroll()
	{
		scrollRt.anchoredPosition = new Vector2(0f, 0f);
	}

	public void ScrollToCoins()
	{
		scrollRt.anchoredPosition = new Vector2(0f, scrollCoinsPosY);
	}

	public void ScrollToNoAds()
	{
		scrollRt.anchoredPosition = new Vector2(0f, scrollRt.sizeDelta.y);
	}
}
