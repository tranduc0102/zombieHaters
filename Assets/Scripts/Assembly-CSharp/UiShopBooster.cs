using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UiShopBooster : MonoBehaviour
{
	public Text lbl_price;

	public Text busterCountText;

	public Text newBoosterCountText;

	public ParticleSystem buyFx;

	public Image icon;

	[Space(20f)]
	public SaveData.BoostersData.BoosterType boosterType;

	public int amount = 1;

	private UIPopupShop uIPopupShop;

	private Vector2 iconStartSize;

	private Coroutine incIcon;

	private float speed = 450f;

	[SerializeField]
	private GameObject priceLoader;

	private Coroutine priceCor;

	private void Start()
	{
		uIPopupShop = GetComponentInParent<UIPopupShop>();
		iconStartSize = icon.rectTransform.sizeDelta;
	}

	private void OnEnable()
	{
		BoosterPurchaseInfo boosterPurchaseInfo = InAppManager.Instance.boosterPurchases.Find((BoosterPurchaseInfo b) => b.boosterType == boosterType);
		UIController.instance.StartGetPrice(lbl_price, priceLoader, boosterPurchaseInfo, ref priceCor);
		newBoosterCountText.text = "+" + boosterPurchaseInfo.boosterCount;
	}

	public void updatePrice()
	{
		busterCountText.text = DataLoader.Instance.GetBoostersCount(boosterType).ToString();
	}

	public void OnBuyMoney()
	{
	}

	public void OnBuyReal()
	{
		InAppManager.Instance.BuyProductID(InAppManager.Instance.boosterPurchases.First((BoosterPurchaseInfo bp) => bp.boosterType == boosterType).index);
	}

	public void OnShowAds()
	{
        DataLoader.Instance.BuyBoosters(boosterType, amount);
        PurchaseFx();
        uIPopupShop.UpdateBoosters();
        AnalyticsManager.instance.LogEvent("BuyBoosterVideo", new Dictionary<string, string> {
            {
                "Type",
                boosterType.ToString()
            } });
  //      AdsManager.instance.ShowRewarded(delegate
		//{
			
		//}, (boosterType != SaveData.BoostersData.BoosterType.KillAll) ? AdsManager.AdName.RewardMoreSurvival : AdsManager.AdName.RewardKillAll);
	}

	public void PurchaseFx()
	{
		if (incIcon != null)
		{
			StopCoroutine(incIcon);
		}
		incIcon = StartCoroutine(IncIcon());
	}

	private IEnumerator IncIcon()
	{
		icon.rectTransform.sizeDelta = iconStartSize;
		Vector2 newSize = iconStartSize * 1.18f;
		while (icon.rectTransform.sizeDelta != newSize)
		{
			icon.rectTransform.sizeDelta = Vector2.MoveTowards(icon.rectTransform.sizeDelta, newSize, speed * Time.deltaTime);
			yield return null;
		}
		buyFx.Play();
		while (icon.rectTransform.sizeDelta != iconStartSize)
		{
			icon.rectTransform.sizeDelta = Vector2.MoveTowards(icon.rectTransform.sizeDelta, iconStartSize, speed * Time.deltaTime);
			yield return null;
		}
		icon.rectTransform.sizeDelta = iconStartSize;
	}
}
