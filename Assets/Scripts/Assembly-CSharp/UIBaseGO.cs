using System;
using System.Collections;
using System.Collections.Generic;
using ACEPlay.Bridge;
using GuiInGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GUI = GuiInGame.GUI;

public class UIBaseGO : MonoBehaviour
{
	[SerializeField]
	protected Text popUpName;

	[SerializeField]
	protected Text coinsCountText;

	[SerializeField]
	protected Text timeText;

	[SerializeField]
	protected GameObject multiplierImageObj;

	[SerializeField]
	protected Button buttonOk;

	[SerializeField]
	protected Button buttonX2;

	protected double currentCoins;

	protected int currentTime;

	protected double coins;

	protected int exp;

	protected int haters;

	protected virtual string GetVideoEventName()
	{
		return "GameOverReward";
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		SetButtonListener(buttonOk, ExitToMenu);
		SetButtonListener(buttonX2, AdsReward);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public virtual void SetContent(double newcoins, int newHaters, int exp, long score, int time)
	{
		coins = newcoins;
		this.exp = exp;
		haters = newHaters;
		StopAllCoroutines();
		currentCoins = newcoins;
		currentTime = time;
		CheckShortGame();
		if (multiplierImageObj != null)
		{
			multiplierImageObj.SetActive(false);
		}
		buttonX2.gameObject.SetActive(true);
		buttonX2.interactable = true;
		buttonOk.image.rectTransform.anchoredPosition = new Vector2(-200f, 0f);
		SetPopupDifferentData();
		if (timeText != null)
		{
			timeText.text = string.Format("{0:D2}:{1:D2}", time / 60, time - time / 60 * 60);
		}
		DataLoader.gui.OnAnimationCompleted(GetTriggerName(), GetClipName(), OnPopupAnimationCompleted);
	}

	protected virtual string GetTriggerName()
	{
		return "GameOver";
	}

	protected virtual string GetClipName()
	{
		return "GameOver";
	}

	protected virtual AdName GetAdName()
	{
		return AdName.RewardX2GameOver;
	}

	public void AdsReward()
	{
		StopAllCoroutines();
		buttonX2.interactable = false;
		if (coinsCountText != null)
		{
			coinsCountText.text = AbbreviationUtility.AbbreviateNumber(currentCoins);
		}
		UnityEvent onDone = new UnityEvent();
		onDone.AddListener(GetX2);
		BridgeController.instance.ShowRewarded($"Show reward: {GetAdName()}", onDone);
	}

	private void GetX2()
	{
		StartCoroutine(ActivateVideoBonus());
	}

	protected virtual IEnumerator ActivateVideoBonus()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		buttonX2.gameObject.SetActive(false);
		buttonOk.image.rectTransform.anchoredPosition = Vector2.zero;
		DataLoader.Instance.RefreshMoney(currentCoins, true);
		multiplierImageObj.SetActive(true);
		UIController.instance.scrollControllers.survivorsController.SetRandomVideo();
		DataLoader.gui.DecreaseInterstitialCounter();
		StartCoroutine(AnimateText(coinsCountText, currentCoins, currentCoins * 2.0, true, true));
		AnalyticsManager.instance.LogEvent(GetVideoEventName(), new Dictionary<string, string>());
	}

	private void CheckShortGame()
	{
		if (currentTime < 20)
		{
			DataLoader.gui.DecreaseInterstitialCounter();
		}
	}

	public string TimeInSeconds(int timeInSeconds)
	{
		string text = timeInSeconds - timeInSeconds % 10 + "-" + (timeInSeconds - timeInSeconds % 10 + 10);
		Debug.Log("PlayTime: " + text);
		return text;
	}

	public virtual void ExitToMenu()
	{
		DataLoader.gui.MainMenu();
		DataLoader.gui.ChangeAnimationState("MainMenu");
		DataLoader.gui.TryToShowInterstirial();
	}

	protected IEnumerator AnimateText(Text text, double current, double target, bool needAbbreviate, bool hideMultiplierImage = false)
	{
		text.text = Math.Floor(current).ToString();
		float speed = 15f;
		double delta = target - current;
		for (int i = 1; (float)i <= speed; i++)
		{
			yield return new WaitForSecondsRealtime(0.05f);
			if (needAbbreviate)
			{
				text.text = AbbreviationUtility.AbbreviateNumber(Math.Ceiling(current + delta / (double)speed * (double)i));
			}
			else
			{
				text.text = Math.Ceiling(current + delta / (double)speed * (double)i).ToString();
			}
		}
		if (needAbbreviate)
		{
			text.text = AbbreviationUtility.AbbreviateNumber(Math.Ceiling(current + delta));
		}
		else
		{
			text.text = Math.Ceiling(current + delta).ToString();
		}
		if (hideMultiplierImage)
		{
			multiplierImageObj.SetActive(false);
		}
	}

	private void SetButtonListener(Button button, UnityAction call)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(call);
	}

	public virtual void SetPopupDifferentData()
	{
		popUpName.font = LanguageManager.instance.currentLanguage.font;
	}

	public virtual void OnPopupAnimationCompleted()
	{
		WeatherManager.Instance.EnableRandomWeather();
	}
}
