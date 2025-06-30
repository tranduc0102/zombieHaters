using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRateUs : MonoBehaviour
{
	[SerializeField]
	private Button[] starButtons;

	[SerializeField]
	private Button submit;

	private int activeStarsCount;

	public static UIRateUs instance { get; private set; }

	public UIRateUs()
	{
		instance = this;
	}

	private void Start()
	{
		submit.interactable = false;
		for (int i = 0; i < starButtons.Length; i++)
		{
			int index = i;
			starButtons[i].onClick.AddListener(delegate
			{
				for (int j = 0; j < starButtons.Length; j++)
				{
					starButtons[j].image.sprite = ((j > index) ? UIController.instance.multiplyImages.rateUsStar.inactive : UIController.instance.multiplyImages.rateUsStar.active);
				}
				submit.interactable = true;
				submit.image.sprite = UIController.instance.multiplyImages.upgrageButtons[4].active;
				activeStarsCount = index + 1;
			});
		}
	}

	public void OnSubmit()
	{
		if (activeStarsCount > 3)
		{
			Application.OpenURL("market://details?id=com.woodensword.zombie");
		}
		DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
		AnalyticsManager.instance.LogEvent("Rate", new Dictionary<string, string> { 
		{
			"Stars",
			activeStarsCount.ToString()
		} });
	}

	public void OnCancel()
	{
		DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
		AnalyticsManager.instance.LogEvent("RateCanceled", new Dictionary<string, string>());
	}

	public void Show()
	{
		DataLoader.gui.popUpsPanel.gameObject.SetActive(true);
		base.gameObject.SetActive(true);
	}

	public void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
	}
}
