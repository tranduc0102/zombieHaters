using System;
using System.Collections;
using System.Collections.Generic;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UISubscription : MonoBehaviour
{
	[SerializeField]
	private GameObject priceLoader;

	[SerializeField]
	private Text textPrice;

	[SerializeField]
	private GameObject mainPopup;

	[SerializeField]
	private GameObject claimPopup;

	[SerializeField]
	private Button btn_claim;

	[SerializeField]
	private Button btn_available;

	[SerializeField]
	private Text timerText;

	[SerializeField]
	private Button btn_buy;

	[SerializeField]
	private GameObject redCircle;

	[SerializeField]
	private Text lbl_subRules;

	private Coroutine getPriceCor;

	private void Start()
	{
		btn_buy.onClick.AddListener(delegate
		{
			InAppManager.Instance.BuyProductID(InAppManager.Instance.subscription.index);
		});
		btn_claim.onClick.AddListener(ClaimReward);
	}

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
		UpdateContent();
		if (LanguageManager.instance.currentLanguage.language == SystemLanguage.ChineseSimplified || LanguageManager.instance.currentLanguage.language == SystemLanguage.ChineseTraditional)
		{
			lbl_subRules.resizeTextForBestFit = false;
			lbl_subRules.fontSize = 38;
		}
		else if (LanguageManager.instance.currentLanguage.language == SystemLanguage.Japanese)
		{
			lbl_subRules.resizeTextForBestFit = false;
			lbl_subRules.fontSize = 33;
		}
		else
		{
			lbl_subRules.resizeTextForBestFit = true;
		}
		lbl_subRules.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.AndroidSubscriptionDescription);
		lbl_subRules.font = LanguageManager.instance.currentLanguage.font;
	}

	public void UpdateRedCircle()
	{
		redCircle.SetActive(InAppManager.Instance.GetLocalSubManager().CanDailyClaim());
	}

	public void UpdateContent()
	{
		if (InAppManager.Instance.IsSubscribed())
		{
			claimPopup.SetActive(true);
			mainPopup.SetActive(false);
			StartCoroutine(Timer());
		}
		else
		{
			mainPopup.SetActive(true);
			claimPopup.SetActive(false);
		}
		UpdateRedCircle();
	}

	private IEnumerator Timer()
	{
		btn_claim.gameObject.SetActive(false);
		btn_available.gameObject.SetActive(true);
		TimeSpan timeSpan = InAppManager.Instance.GetLocalSubManager().GetNextClaimDate().Subtract(TimeManager.CurrentDateTime);
		while (timeSpan.TotalSeconds > 0.0)
		{
			if (timeSpan.Days > 0)
			{
				timerText.text = string.Format("{0:D1} {1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else
			{
				timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1.0));
			yield return new WaitForSecondsRealtime(1f);
		}
		btn_claim.gameObject.SetActive(true);
		btn_available.gameObject.SetActive(false);
	}

	private void ClaimReward()
	{
		Debug.Log("Subscription reward claimed.");
		DataLoader.Instance.RefreshGems(7, true);
		InAppManager.Instance.GetLocalSubManager().SetClaimDate();
		InAppManager.Instance.SaveLocalSubManager();
		StartCoroutine(Timer());
		UpdateRedCircle();
		AnalyticsManager.instance.LogEvent("SubscriptionClaimReward", new Dictionary<string, string>());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void OpenTerms()
	{
		Application.OpenURL("https://sites.google.com/view/dotjoyterms/home");
	}

	public void OpenPrivacy()
	{
		Application.OpenURL("https://sites.google.com/view/dotjoy/home");
	}
}
