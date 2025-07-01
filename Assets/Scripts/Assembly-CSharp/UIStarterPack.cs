using System;
using System.Collections;
using ACEPlay.Bridge;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UIStarterPack : MonoBehaviour
{
	[SerializeField]
	private Text textBoosterSurvivior;

	[SerializeField]
	private Text textBoosterKillAll;

	[SerializeField]
	private Text textCoins;

	[SerializeField]
	private Text textPrice;

	[SerializeField]
	private Text timeLeft;

	[SerializeField]
	private Text packName;

	public GameObject starterMenu;

	public Button buttonBuy;

	[SerializeField]
	private GameObject priceLoader;

	private StarterPackPurchaseInfo sp;

	private TimeSpan timeLeftSpan;

	[HideInInspector]
	public bool autoShowCompleted;

	private Coroutine getPriceCor;

	private string LastPackDate = "LastPackDate";

	private string LastPackIndex = "LastPackIndex";

	private string startPriceText = string.Empty;

	private void Start()
	{
		startPriceText = textPrice.text;
	}

	public void Show(bool fromMenu = false)
	{
		TryToShowPack();
	}

	public void TryToShowPack()
	{
		if (GameManager.instance.currentGameMode != 0)
		{
			return;
		}
		starterMenu.SetActive(GameManager.instance.IsTutorialCompleted());
		TimeSpan timeSpan;
		if (starterMenu.activeSelf && DataLoader.playerData.GetTimeInGameCount(out timeSpan))
		{
			if (timeSpan.TotalDays >= 2.0 || PlayerPrefs.HasKey(StaticConstants.starterPackPurchased))
			{
				ShowDiscountPack();
			}
			else
			{
				ShowStarter();
			}
		}
	}

	private void ShowStarter()
	{
		sp = InAppManager.Instance.starterPack;
		timeLeftSpan = TimeSpan.FromHours(StaticConstants.StarterPackHoursDuration).Subtract(TimeManager.CurrentDateTime.Subtract(DataLoader.playerData.firstEnterDate));
		RefreshPackInfoAndShow();
	}

	private void ShowDiscountPack()
	{
		timeLeftSpan = TimeManager.CurrentDateTime.AddDays(1.0).Date.Subtract(TimeManager.CurrentDateTime);
		sp = ((!IsNewPackTime()) ? GetPackPurchaseInfo(PlayerPrefs.GetInt(LastPackIndex)) : GetNextPack());
		RefreshPackInfoAndShow();
	}

	public StarterPackPurchaseInfo GetNextPack()
	{
		PlayerPrefs.SetString(LastPackDate, TimeManager.CurrentDateTime.Ticks.ToString());
		int num = ((!PlayerPrefs.HasKey(LastPackIndex)) ? 1 : GetNextIndex(PlayerPrefs.GetInt(LastPackIndex)));
		PlayerPrefs.SetInt(LastPackIndex, num);
		return GetPackPurchaseInfo(num);
	}

	private void RefreshPackInfoAndShow()
	{
		textBoosterSurvivior.text = sp.boosters[0].amount.ToString();
		textBoosterKillAll.text = sp.boosters[1].amount.ToString();
		textCoins.text = string.Format("{0:N0}", sp.reward);
		packName.text = LanguageManager.instance.GetLocalizedText(sp.displayedPackName);
		packName.font = LanguageManager.instance.currentLanguage.font;
		buttonBuy.onClick.RemoveAllListeners();
		buttonBuy.onClick.AddListener(delegate
		{
			InAppManager.Instance.BuyProductID(sp.index);
		});
		autoShowCompleted = true;
		base.gameObject.SetActive(true);
		DataLoader.gui.popUpsPanel.OpenPopup();
	}

	public IEnumerator Countdown()
	{
		if (timeLeftSpan.Ticks == 0)
		{
			yield return null;
		}
		while (timeLeftSpan.Ticks > 0)
		{
			timeLeft.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeftSpan.Hours + timeLeftSpan.Days * 24, timeLeftSpan.Minutes, timeLeftSpan.Seconds);
			yield return new WaitForSecondsRealtime(1f);
			timeLeftSpan = timeLeftSpan.Add(TimeSpan.FromSeconds(-1.0));
		}
		Show();
	}

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
		StartCoroutine(Countdown());
		UIController.instance.StartGetPrice(textPrice, priceLoader, sp, ref getPriceCor);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
	private StarterPackPurchaseInfo GetPackPurchaseInfo(int index)
	{
		return InAppManager.Instance.packs[index];
	}

	private int GetNextIndex(int currentIndex)
	{
		return (currentIndex + 1 < InAppManager.Instance.packs.Count) ? (currentIndex + 1) : 0;
	}

	private bool IsNewPackTime()
	{
		return !PlayerPrefs.HasKey(LastPackDate) || (TimeManager.CurrentDateTime - new DateTime(Convert.ToInt64(PlayerPrefs.GetString(LastPackDate)))).Days > 1;
	}
}
