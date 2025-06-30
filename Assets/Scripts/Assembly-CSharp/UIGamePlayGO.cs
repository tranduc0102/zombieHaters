using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlayGO : UIBaseGO
{
	[SerializeField]
	private Text expCountText;

	[SerializeField]
	private Text hatersCountText;

	[SerializeField]
	private Text scoreText;

	public override void SetContent(double newcoins, int newHaters, int exp, long score, int time)
	{
		base.SetContent(newcoins, newHaters, exp, score, time);
		if (GameManager.instance.isTutorialNow)
		{
			buttonX2.gameObject.SetActive(false);
			buttonOk.image.rectTransform.anchoredPosition = Vector2.zero;
			AnalyticsManager.instance.LogEvent("TutorialComleted", new Dictionary<string, string> { 
			{
				"TimeInSeconds",
				TimeInSeconds(time)
			} });
		}
		else
		{
			AnalyticsManager.instance.LogEvent("GameOver", new Dictionary<string, string>
			{
				{
					"TimeInSeconds",
					TimeInSeconds(time)
				},
				{
					"Coins",
					((int)newcoins).ToString()
				},
				{
					"Experience",
					exp.ToString()
				},
				{
					"NewHaters",
					newHaters.ToString()
				},
				{
					"Score",
					score.ToString()
				}
			});
			SoundManager.Instance.soundVolume = 0.15f;
			SoundManager.Instance.PlaySound(SoundManager.Instance.gameOverSound, 1f);
		}
		scoreText.font = LanguageManager.instance.currentLanguage.font;
		scoreText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Score) + ": " + score;
		DataLoader.gui.gameOverManager.popUpRect.sizeDelta = new Vector2(DataLoader.gui.gameOverManager.popUpRect.sizeDelta.x, 1190f);
		coinsCountText.text = "0";
		expCountText.text = "0";
		hatersCountText.text = "0";
	}

	protected override string GetClipName()
	{
		return (!GameManager.instance.isTutorialNow) ? base.GetClipName() : "TutorialComleted";
	}

	protected override string GetTriggerName()
	{
		return (!GameManager.instance.isTutorialNow) ? base.GetTriggerName() : "TutorialCompleted";
	}

	public override void SetPopupDifferentData()
	{
		base.SetPopupDifferentData();
		if (GameManager.instance.isTutorialNow)
		{
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Tutorial_Completed));
			popUpName.fontSize = 75;
		}
		else
		{
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Game_over));
			popUpName.fontSize = 100;
		}
	}

	public override void OnPopupAnimationCompleted()
	{
		base.OnPopupAnimationCompleted();
		AnimateAllText();
	}

	public void AnimateAllText()
	{
		StartCoroutine(AnimateText(coinsCountText, 0.0, coins, true));
		StartCoroutine(AnimateText(expCountText, 0.0, exp, true));
		StartCoroutine(AnimateText(hatersCountText, 0.0, haters, false));
	}
}
