using System.Collections.Generic;
using IAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIIngameShop : MonoBehaviour
{
	[SerializeField]
	private Image boosterImage;

	[SerializeField]
	private Text newBoosterCountText;

	[SerializeField]
	private Text currentBoostersCountText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text description;

	[SerializeField]
	private GameObject priceLoader;

	[SerializeField]
	private Button buttonBuy;

	[SerializeField]
	private ParticleSystem purchaseFx;

	private Coroutine priceCor;

	private SaveData.BoostersData.BoosterType currentBoosterType;

	private UnityAction refreshCall;

	private void OnEnable()
	{
		Time.timeScale = 0f;
	}

	private void OnDisable()
	{
		DataLoader.gui.Resume();
	}

	public void OpenShop(SaveData.BoostersData.BoosterType boosterType, UnityAction refreshCall)
	{
		currentBoosterType = boosterType;
		this.refreshCall = refreshCall;
		BoosterPurchaseInfo booster = InAppManager.Instance.boosterPurchases.Find((BoosterPurchaseInfo b) => b.boosterType == boosterType);
		UIController.instance.StartGetPrice(priceText, priceLoader, booster, ref priceCor);
		newBoosterCountText.text = "+" + booster.boosterCount;
		currentBoostersCountText.text = DataLoader.Instance.GetBoostersCount(currentBoosterType).ToString();
		description.font = LanguageManager.instance.currentLanguage.font;
		if (boosterType == SaveData.BoostersData.BoosterType.NewSurvivor)
		{
			boosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[0];
			description.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.You_just_received_an_additional_survivor);
		}
		else
		{
			boosterImage.sprite = UIController.instance.multiplyImages.activeBoosters[1];
			description.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.A_huge_explosion_that_destroys_all_the_zombies_Only_Zombie_bosses_can_survive_it);
		}
		buttonBuy.onClick.RemoveAllListeners();
		buttonBuy.onClick.AddListener(delegate
		{
			InAppManager.Instance.BuyProductID(booster.index);
		});
		DataLoader.gui.popUpsPanel.OpenPopup();
		base.gameObject.SetActive(true);
		AnalyticsManager.instance.LogEvent("InGameShopOpened", new Dictionary<string, string>());
	}

	public void UpdateAfterPurchase()
	{
		currentBoostersCountText.text = DataLoader.Instance.GetBoostersCount(currentBoosterType).ToString();
		refreshCall();
		purchaseFx.Play();
		AnalyticsManager.instance.LogEvent("InGameShopPurchase", new Dictionary<string, string> { 
		{
			"BoosterType",
			currentBoosterType.ToString()
		} });
	}
}
