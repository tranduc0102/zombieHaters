using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArenaGO : UIBaseGO
{
	[SerializeField]
	private Image cup;

	[SerializeField]
	private Text ratingText;

	[SerializeField]
	private Text bonusText;

	[SerializeField]
	private Text percentageText;

	[SerializeField]
	private Text gemsText;

	private int target;

	private int current;

	private Coroutine ratingCor;

	protected override string GetVideoEventName()
	{
		return "LeagueAddRating";
	}

/*	protected override AdsManager.AdName GetAdName()
	{
		return AdsManager.AdName.RewardIncRatingArena;
	}*/

	public override void SetContent(double newcoins, int newHaters, int exp, long score, int time)
	{
		base.SetContent(newcoins, newHaters, exp, score, time);
		DataLoader.gui.gameOverManager.popUpRect.sizeDelta = new Vector2(DataLoader.gui.gameOverManager.popUpRect.sizeDelta.x, 1170f);
		AnalyticsManager.instance.LogEvent(GetResultEventName(), new Dictionary<string, string>
		{
			{
				"Rating",
				DataLoader.playerData.arenaRating.ToString()
			},
			{
				"TimeInSeconds",
				TimeInSeconds(time)
			}
		});
		coinsCountText.text = "0";
	}

	public override void SetPopupDifferentData()
	{
		base.SetPopupDifferentData();
		bonusText.font = LanguageManager.instance.currentLanguage.font;
		bonusText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Bonus) + " " + (ArenaManager.instance.GetArenaIndexByRating(DataLoader.playerData.arenaRating) + 1);
		current = ArenaManager.instance.ratingBeforeGame;
		if (ArenaManager.instance.ratingBeforeGame < DataLoader.playerData.arenaRating)
		{
			DataLoader.Instance.RefreshGems(ArenaManager.instance.currentArenaInfo.gemsReward, true);
			gemsText.text = ArenaManager.instance.currentArenaInfo.gemsReward.ToString();
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Mission_completed);
			cup.sprite = UIController.instance.multiplyImages.arenaRatingCup.active;
			int maxRating = ArenaManager.instance.GetMaxRating();
			if (ArenaManager.instance.ratingBeforeGame >= maxRating)
			{
				ratingText.text = "<color=#fff000>" + ArenaManager.instance.ratingBeforeGame + "</color>";
				target = current;
			}
			else if (ArenaManager.instance.ratingBeforeGame > maxRating - ArenaManager.instance.winRating)
			{
				target = current + (maxRating - ArenaManager.instance.ratingBeforeGame);
				ratingText.text = "<color=#fff000>" + current + "</color><color=#7EEA00> + " + (maxRating - ArenaManager.instance.ratingBeforeGame) + "</color>";
			}
			else
			{
				target = current + ArenaManager.instance.winRating;
				ratingText.text = "<color=#fff000>" + current + "</color><color=#7EEA00> + " + ArenaManager.instance.winRating + "</color>";
			}
		}
		else
		{
			gemsText.text = "0";
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Mission_failed);
			cup.sprite = UIController.instance.multiplyImages.arenaRatingCup.inactive;
			if (ArenaManager.instance.ratingBeforeGame > ArenaManager.instance.loseRating)
			{
				target = current - ArenaManager.instance.loseRating;
				ratingText.text = "<color=#fff000>" + current + "</color><color=#ff0000> - " + ArenaManager.instance.loseRating + "</color>";
			}
			else
			{
				target = current - ArenaManager.instance.ratingBeforeGame;
				ratingText.text = "<color=#fff000>" + current + "</color><color=#ff0000> - " + ArenaManager.instance.ratingBeforeGame + "</color>";
			}
			if (target == current)
			{
				ratingText.text = "<color=#fff000>" + current + "</color>";
			}
		}
	}

	private IEnumerator RatingCounter(bool addRating)
	{
		yield return new WaitForSecondsRealtime(0.3f);
		int delta = target - current;
		float speed = 25f;
		if (delta != 0)
		{
			for (int i = 1; (float)i <= speed; i++)
			{
				yield return new WaitForSecondsRealtime(0.05f);
				int temp = current + Mathf.CeilToInt((float)delta / speed * (float)i);
				if (addRating)
				{
					ratingText.text = "<color=#fff000>" + temp + "</color><color=#7EEA00> + " + (target - temp) + "</color>";
				}
				else
				{
					ratingText.text = "<color=#fff000>" + temp + "</color><color=#ff0000> - " + -(target - temp) + "</color>";
				}
			}
		}
		ratingText.text = "<color=#fff000>" + target + "</color>";
	}

	private string GetResultEventName()
	{
		return (ArenaManager.instance.ratingBeforeGame >= DataLoader.playerData.arenaRating) ? "ArenaLose" : "ArenaWin";
	}

	public override void ExitToMenu()
	{
		base.ExitToMenu();
		DataLoader.gui.popUpsPanel.OpenPopup();
		DataLoader.gui.popUpsPanel.ratingGames.gameObject.SetActive(true);
	}

	public override void OnPopupAnimationCompleted()
	{
		base.OnPopupAnimationCompleted();
		StartCoroutine(AnimateText(coinsCountText, 0.0, coins, true));
		ratingCor = StartCoroutine(RatingCounter(ArenaManager.instance.ratingBeforeGame < DataLoader.playerData.arenaRating));
	}

	protected override IEnumerator ActivateVideoBonus()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		buttonX2.gameObject.SetActive(false);
		buttonOk.image.rectTransform.anchoredPosition = Vector2.zero;
		UIController.instance.scrollControllers.survivorsController.SetRandomVideo();
/*		AdsManager.instance.DecreaseInterstitialCounter();
*/		AnalyticsManager.instance.LogEvent(GetVideoEventName(), new Dictionary<string, string>());
		StopCoroutine(ratingCor);
		current = target;
		target = current + ArenaManager.instance.loseRating;
		ratingCor = StartCoroutine(RatingCounter(true));
		ArenaManager.instance.SaveArenaRating(ArenaManager.instance.loseRating);
	}
}
