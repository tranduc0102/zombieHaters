using System;
using Daily;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyPanel : UIBaseScrollPanel<UIDailyCell>
{
	public enum StreakType
	{
		GetToday = 0,
		OnStreak = 1,
		StreakEnded = 2,
		Unknown = 3
	}

	[SerializeField]
	[Space]
	private GameObject redCircle;

	[SerializeField]
	private GameObject askAboutNotificationsPanel;

	[SerializeField]
	private GameObject dailyCloseBg;

	[SerializeField]
	private Button buttonYes;

	[SerializeField]
	private Text dailyHeaderText;

	[SerializeField]
	private Image menuDailyPresent;

	public Animator dailyAnim;

	private DateTime currentDate;

	private StreakType type;

	public override void CreateCells()
	{
		base.CreateCells();
		ActivateDailyReward();
	}

	public void OnEnable()
	{
		ActivateDailyReward();
		dailyCloseBg.SetActive(false);
		askAboutNotificationsPanel.SetActive(false);
		StartCoroutine(UIController.instance.Scale(base.transform));
	}

	private void Start()
	{
	}

	public void ShowNotificationAskPanel()
	{
		DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
	}

	public void ActivateDailyReward(bool debug = true)
	{
		UpdateAllContent();
		if (TimeManager.gotDateTime)
		{
			currentDate = TimeManager.CurrentDateTime;
			type = CanGetReward(debug);
			ActivateCurrentDay();
		}
		else
		{
			type = StreakType.Unknown;
			DisableRed(false);
		}
	}

	public StreakType CanGetReward(bool debug = true)
	{
		if (PlayerPrefs.HasKey(StaticConstants.DailyRewardKey))
		{
			DateTime value = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.DailyRewardKey)), DateTimeKind.Utc);
			int num = (int)currentDate.Subtract(value).TotalDays;
			if (num == DataLoader.playerData.totalDaysInRow)
			{
				if (debug)
				{
					Debug.Log("Can't claim reward today. Total days: " + (DataLoader.playerData.totalDaysInRow + 1));
				}
				DisableRed(false);
				return StreakType.OnStreak;
			}
			if (num == DataLoader.playerData.totalDaysInRow + 1)
			{
				if (debug)
				{
					Debug.Log("You can claim reward. Total days: " + (DataLoader.playerData.totalDaysInRow + 1));
				}
				DisableRed(true);
				return StreakType.GetToday;
			}
			if (debug)
			{
				Debug.Log("Streak ended. You can claim reward. Total days: " + (DataLoader.playerData.totalDaysInRow + 1));
			}
			DataLoader.Instance.SetTotalDays(-1);
			DisableRed(true);
			return StreakType.StreakEnded;
		}
		Debug.Log("First enter. You can claim reward.");
		DisableRed(true);
		return StreakType.StreakEnded;
	}

	public void DisableRed(bool b)
	{
		redCircle.SetActive(b);
	}

	private void ActivateCurrentDay()
	{
		int num = 0;
		int num2;
		if (type == StreakType.OnStreak)
		{
			num2 = DataLoader.playerData.totalDaysInRow % 7 + 1;
			num = DataLoader.playerData.totalDaysInRow / 7;
		}
		else
		{
			num2 = (DataLoader.playerData.totalDaysInRow + 1) % 7 + 1;
			num = (DataLoader.playerData.totalDaysInRow + 1) / 7;
		}
		dailyHeaderText.text = (num + 1).ToString();
		DataLoader.Instance.currentDayInRow = num2 + 7 * (num % 2) - 1;
		int currentDayInRow = DataLoader.Instance.currentDayInRow;
		currentDayInRow = ((currentDayInRow < 7) ? currentDayInRow : (currentDayInRow - 7));
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (currentDayInRow == 0 && i == 0)
			{
				if (type == StreakType.OnStreak)
				{
					dataArray[0].SetDailyContent(new Inactive());
				}
				else
				{
					dataArray[0].SetDailyContent(new Active());
				}
				continue;
			}
			int num3 = ((type == StreakType.OnStreak) ? (currentDayInRow + 1) : currentDayInRow);
			if (i < num3)
			{
				dataArray[i].SetDailyContent(new Inactive());
			}
			else if (i == num3)
			{
				if (type == StreakType.OnStreak)
				{
					dataArray[i].SetDailyContent(new Next());
				}
				else
				{
					dataArray[i].SetDailyContent(new Active());
				}
			}
			else
			{
				dataArray[i].SetDailyContent(new Next());
			}
		}
		if (num > 0)
		{
			menuDailyPresent.sprite = UIController.instance.multiplyImages.dailyPresent[MultiplyImages.GetDailyPresentSpriteIndex(DataLoader.Instance.dailyBonus[currentDayInRow + 7].type)];
		}
		else
		{
			menuDailyPresent.sprite = UIController.instance.multiplyImages.dailyPresent[MultiplyImages.GetDailyPresentSpriteIndex(DataLoader.Instance.dailyBonus[currentDayInRow].type)];
		}
	}

	public void ClaimReward()
	{
		if (type == StreakType.GetToday || type == StreakType.StreakEnded)
		{
			DataLoader.Instance.SetTotalDays(DataLoader.playerData.totalDaysInRow + 1);
			if (type == StreakType.StreakEnded)
			{
				PlayerPrefs.SetString(StaticConstants.DailyRewardKey, currentDate.Date.Ticks.ToString());
			}
			dataArray[(DataLoader.Instance.currentDayInRow < 7) ? DataLoader.Instance.currentDayInRow : (DataLoader.Instance.currentDayInRow - 7)].ActivateReward();
			type = StreakType.OnStreak;
			ActivateDailyReward();
			Debug.Log("Got Daily reward");
		}
	}

	public override int GetCellCount()
	{
		return 7;
	}

	public override void UpdateAllContent()
	{
		if (type == StreakType.OnStreak)
		{
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].SetContent((DataLoader.playerData.totalDaysInRow + 1 <= 7) ? i : (i + 7));
			}
		}
		else
		{
			for (int j = 0; j < dataArray.Length; j++)
			{
				dataArray[j].SetContent((DataLoader.playerData.totalDaysInRow + 1 < 7) ? j : (j + 7));
			}
		}
		ActivateCurrentDay();
	}
}
