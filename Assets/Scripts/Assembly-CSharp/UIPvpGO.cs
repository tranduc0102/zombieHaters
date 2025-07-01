using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuiInGame;
using UnityEngine;
using UnityEngine.UI;

public class UIPvpGO : UIBaseGO
{
	[Serializable]
	public class Cell
	{
		public Text placeText;

		public Text levelText;

		public Text nameText;

		public Text ratingText;

		public Image bgPlashka;

		public Image skull;

		private readonly int[] ratings = new int[10] { 25, 0, -15, -15, -15, -15, -15, -15, -15, -15 };

		public void SetData(int place, int level, string name, bool isPlayer)
		{
			placeText.text = "#" + (place + 1);
			levelText.text = level.ToString();
			levelText.gameObject.SetActive(level != 0);
			skull.gameObject.SetActive(level == 0);
			nameText.text = name;
			bgPlashka.gameObject.SetActive(isPlayer && level > 0);
			if (!isPlayer)
			{
				placeText.color = new Color(36f / 85f, 0.8980392f, 0.99607843f, 1f);
			}
			else
			{
				placeText.color = Color.white;
			}
			nameText.color = placeText.color;
			int num = ratings[place];
			UpdateRating(ratings[place]);
			if (isPlayer)
			{
				ArenaManager.instance.SaveArenaRating(ArenaManager.instance.GetAddedWinRating() + ratings[place]);
				DataLoader.Instance.RefreshGems((place == 0) ? ArenaManager.instance.currentArenaInfo.gemsReward : 0);
				AnalyticsManager.instance.LogEvent("PVPGameOver", new Dictionary<string, string>
				{
					{
						"Place",
						place.ToString()
					},
					{
						"ArenaIndex",
						PVPManager.Instance.currentArenaIndex.ToString()
					}
				});
			}
		}

		public void UpdateRating(int rating)
		{
			if (rating > 0)
			{
				ratingText.text = "+" + rating;
				ratingText.color = new Color(0.64705884f, 1f, 0f, 1f);
			}
			else if (rating == 0)
			{
				ratingText.text = rating.ToString();
				ratingText.color = Color.white;
			}
			else
			{
				ratingText.text = rating.ToString();
				ratingText.color = new Color(1f, 13f / 85f, 0f, 1f);
			}
		}
	}

	public GameObject bigBg;

	public GameObject smallBg;

	public List<Cell> cells;

	public Cell smallCell;

	public Text pizzaCount;

	private int playerCell;

	public override void SetContent(double newcoins, int newHaters, int exp, long score, int time)
	{
		base.SetContent(newcoins, newHaters, exp, score, time);
		SetPvpContent();
	}

	private void SetPvpContent()
	{
		List<PVPPlayerInfo> list = PVPManager.pvpPlayers.OrderBy((PVPPlayerInfo tb) => tb.survivors.Count).Reverse().ToList();
		int count = PVPManager.pvpPlayers[0].survivors.Count;
		int num = -1;
		pizzaCount.text = ArenaManager.instance.currentArenaInfo.gemsReward.ToString();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].controller.pvpPlayerIndex == 0)
			{
				num = i;
			}
		}
		if (!PVPManager.pvpPlayers[0].IsAlive())
		{
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.You_died);
			DataLoader.gui.gameOverManager.popUpRect.sizeDelta = new Vector2(DataLoader.gui.gameOverManager.popUpRect.sizeDelta.x, 565f);
			bigBg.SetActive(false);
			smallBg.SetActive(true);
			playerCell = -1;
			smallCell.SetData(list[num].place, list[num].survivors.Count, list[num].name, true);
			return;
		}
		DataLoader.gui.gameOverManager.popUpRect.sizeDelta = new Vector2(DataLoader.gui.gameOverManager.popUpRect.sizeDelta.x, 1350f);
		bigBg.SetActive(true);
		smallBg.SetActive(false);
		if (num == 0 && !list[1].IsAlive())
		{
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.You_are_the_last_survivor);
		}
		else
		{
			popUpName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Time_out);
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].controller.pvpPlayerIndex != 0 && list[j].survivors.Count == count)
			{
				Swap(list, j, num);
			}
			else if (list[j].controller.pvpPlayerIndex == 0)
			{
				break;
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (list[k].controller.pvpPlayerIndex == 0)
			{
				playerCell = k;
			}
			cells[k].SetData(k, list[k].survivors.Count, list[k].name, list[k].controller.pvpPlayerIndex == 0);
		}
	}

	public static void Swap<T>(IList<T> list, int indexA, int indexB)
	{
		T value = list[indexA];
		list[indexA] = list[indexB];
		list[indexB] = value;
	}

	protected override IEnumerator ActivateVideoBonus()
	{
		yield return new WaitForSecondsRealtime(0.5f);
		buttonX2.gameObject.SetActive(false);
		buttonOk.image.rectTransform.anchoredPosition = Vector2.zero;
		UIController.instance.scrollControllers.survivorsController.SetRandomVideo();
		DataLoader.gui.DecreaseInterstitialCounter();
		AnalyticsManager.instance.LogEvent(GetVideoEventName(), new Dictionary<string, string>());
		if (playerCell == -1)
		{
			smallCell.UpdateRating(int.Parse(smallCell.ratingText.text) + ArenaManager.instance.loseRating);
		}
		else
		{
			cells[playerCell].UpdateRating(int.Parse(cells[playerCell].ratingText.text) + ArenaManager.instance.loseRating);
		}
		ArenaManager.instance.SaveArenaRating(ArenaManager.instance.loseRating);
	}

	protected override string GetVideoEventName()
	{
		return "PVPVideoReward";
	}

	protected override AdName GetAdName()
	{
		return AdName.RewardIncRatingPVP;
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
		Time.timeScale = 0f;
	}
}
