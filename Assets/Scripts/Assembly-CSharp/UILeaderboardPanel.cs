using System;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardPanel : UIBaseScrollPanel<UILeaderboardCell>
{
	[SerializeField]
	private GameObject loader;

	[SerializeField]
	private RectTransform localUserRect;

	[SerializeField]
	private Text lbl_localUserRank;

	[SerializeField]
	private Text lbl_localUserScore;

	[SerializeField]
	private Text lbl_localUserName;

	private bool scoresLoaded;

	private DateTime lastUpdateTime = DateTime.MinValue;

	public override int GetCellCount()
	{
		return 100;
	}

	public void OnEnable()
	{
		SetLoader();
		if ((TimeManager.CurrentDateTime - lastUpdateTime).TotalMinutes > 1.0)
		{
			lastUpdateTime = TimeManager.CurrentDateTime;
			LeaderboardManager.instance.GetScores(UpdateAllContent);
		}
	}

	public void SetLoader()
	{
		loader.SetActive(!scoresLoaded);
		scrollRect.gameObject.SetActive(scoresLoaded);
		localUserRect.gameObject.SetActive(scoresLoaded);
	}

	public override void UpdateAllContent()
	{
		if (LeaderboardManager.instance.leaderboardInfo != null && LeaderboardManager.instance.leaderboardInfo.names.Count > 0)
		{
			scoresLoaded = true;
			for (int i = 0; i < LeaderboardManager.instance.leaderboardInfo.names.Count; i++)
			{
				dataArray[i].UpdateContent(LeaderboardManager.instance.leaderboardInfo.names[i], LeaderboardManager.instance.leaderboardInfo.scores[i]);
			}
			lbl_localUserRank.text = LeaderboardManager.instance.leaderboardInfo.localPlayerRank.ToString();
			lbl_localUserScore.text = string.Format("{0:N0}", LeaderboardManager.instance.leaderboardInfo.localPlayerScore);
			lbl_localUserName.text = LeaderboardManager.instance.leaderboardInfo.localPlayerName;
			SetLoader();
		}
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
		localUserRect.gameObject.SetActive(false);
		loader.SetActive(false);
	}
}
