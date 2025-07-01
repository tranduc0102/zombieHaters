using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/*using GooglePlayGames;
using GooglePlayGames.BasicApi;*/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;
using Range = UnityEngine.SocialPlatforms.Range;

public class LeaderboardManager : MonoBehaviour
{
	[HideInInspector]
	public static bool loginSuccessful;

	public static bool autoSignIn;

	private string leaderboardID;

	[SerializeField]
	private string IosLeaderboardID;

	[SerializeField]
	private string AndroidLeaderboardID;

	[HideInInspector]
	public CustomLeaderboardInfo leaderboardInfo;

	private ILeaderboard leaderboard;

	public static LeaderboardManager instance { get; private set; }

	private void Awake()
	{
		instance = this;
		leaderboardID = AndroidLeaderboardID;
	}

	private void Start()
	{
		/*PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();*/
		if (!PlayerPrefs.HasKey(StaticConstants.autoSignIn))
		{
			PlayerPrefs.SetInt(StaticConstants.autoSignIn, Convert.ToInt32(true));
			PlayerPrefs.Save();
			autoSignIn = true;
		}
		else
		{
			autoSignIn = Convert.ToBoolean(PlayerPrefs.GetInt(StaticConstants.autoSignIn));
		}
		if (autoSignIn)
		{
			AuthenticateUser();
		}
		else
		{
			GPGSCloudSave.syncWithCloud = true;
		}
		StartCoroutine(CustomLeaderboardFirstLoad());
	}

	private IEnumerator CustomLeaderboardFirstLoad()
	{
		while (UIController.instance == null)
		{
			yield return null;
		}
		while (!UIController.instance.scrollControllers.leaderboardController.cellsCreated)
		{
			yield return null;
		}
		while (!loginSuccessful)
		{
			yield return null;
		}
		GetScores(UIController.instance.scrollControllers.leaderboardController.UpdateAllContent);
	}

	public void SignOut()
	{
        loginSuccessful = false;
        autoSignIn = false;
        PlayerPrefs.SetInt(StaticConstants.autoSignIn, Convert.ToInt32(false));
        PlayerPrefs.Save();
       /* if (PlayGamesPlatform.Instance.localUser.authenticated)
		{
			PlayGamesPlatform.Instance.SignOut();
			
		}*/
	}

	private void OnApplicationPause(bool pause)
	{
	}

	public void AuthenticateUser()
	{
		StartTryLogin();
		Social.localUser.Authenticate(delegate(bool success)
		{
			loginSuccessful = success;
			if (success)
			{
				Debug.Log("Successfully Authenticated");
				UIOptions uIOptions = UnityEngine.Object.FindObjectOfType<UIOptions>();
				if (uIOptions != null)
				{
					uIOptions.RefreshGooglePlayUI();
				}
				LoginSuccess();
				GPGSCloudSave.CloudSync(true);
			}
			else
			{
				Debug.Log("Authentication failed");
				GPGSCloudSave.syncWithCloud = true;
			}
		});
	}

	public void PostScoreOnLeaderBoard(long myScore)
	{
		/*if (loginSuccessful)
		{
			Social.ReportScore(myScore, leaderboardID, delegate(bool success)
			{
				if (success)
				{
					Debug.Log("Scores successfully uploaded");
				}
			});
			return;
		}
		Debug.Log("PostScoreOnLeaderBoard | autoSigIn - " + autoSignIn);
		if (!autoSignIn)
		{
			return;
		}
		StartTryLogin();
		Social.localUser.Authenticate(delegate(bool success)
		{
			if (success)
			{
				loginSuccessful = true;
				Social.ReportScore(myScore, leaderboardID, delegate(bool successful)
				{
					if (successful)
					{
						Debug.Log("Scores successfully uploaded");
					}
				});
				LoginSuccess();
			}
			else
			{
				Debug.Log("Authentication failed");
				GPGSCloudSave.syncWithCloud = true;
			}
		});*/
	}

	public void ShowLeaderboard()
	{
        AuthenticateUser();
       /* if (loginSuccessful)
		{
			PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderboardID);
		}
		else
		{
			
		}*/
	}

	private void StartTryLogin()
	{
		autoSignIn = false;
		PlayerPrefs.SetInt(StaticConstants.autoSignIn, Convert.ToInt32(false));
		PlayerPrefs.Save();
		Debug.Log("StartTryLogin | autoSigIn - " + autoSignIn);
	}

	private void LoginSuccess()
	{
		autoSignIn = true;
		PlayerPrefs.SetInt(StaticConstants.autoSignIn, Convert.ToInt32(true));
		PlayerPrefs.Save();
		Debug.Log("LoginSuccess | autoSigIn - " + autoSignIn);
	}

	public string GetPlayerName()
	{
		if (loginSuccessful)
		{
			return Social.localUser.userName;
		}
		return LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Player);
	}

	private void CreateLeaderboard()
	{
/*		leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
*/		leaderboard.range = new Range(1, UIController.instance.scrollControllers.leaderboardController.GetCellCount());
		leaderboard.id = leaderboardID;
	}

	public void GetScores(UnityAction callback)
	{
		leaderboardInfo = null;
		List<long> scores = new List<long>();
		List<string> list = new List<string>();
		/*if (loginSuccessful)
		{
			PlayGamesPlatform.Instance.LoadScores(leaderboardID, LeaderboardStart.TopScores, 100, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, delegate(LeaderboardScoreData data)
			{
				GetAndroidScoresRecursive(data, scores, new List<string>(), callback);
			});
		}*/
	}

	public long GetUserScore()
	{
		return leaderboard.localUserScore.value;
	}

	public int GetUserRank()
	{
		return leaderboard.localUserScore.rank;
	}

/*	public void GetAndroidScoresRecursive(LeaderboardScoreData data, List<long> scores, List<string> userIDs, UnityAction callback)
	{
		Debug.Log("Recursive Call");
		IScore[] scores2 = data.Scores;
		foreach (IScore score in scores2)
		{
			userIDs.Add(score.userID);
			scores.Add(score.value);
		}
		if (scores.Count < UIController.instance.scrollControllers.leaderboardController.GetCellCount())
		{
			PlayGamesPlatform.Instance.LoadMoreScores(data.NextPageToken, 25, delegate(LeaderboardScoreData results)
			{
				GetAndroidScoresRecursive(results, scores, userIDs, callback);
			});
			return;
		}
		List<string> names = new List<string>();
		PlayGamesPlatform.Instance.LoadUsers(userIDs.Distinct().ToArray(), delegate(IUserProfile[] userProfiles)
		{
			for (int j = 0; j < userProfiles.Length; j++)
			{
				names.Add(userProfiles[j].userName);
			}
			Debug.Log("Names Count: " + names.Count);
			Debug.Log("Scores Count: " + scores.Count);
			leaderboardInfo = new CustomLeaderboardInfo();
			leaderboardInfo.names = names;
			leaderboardInfo.scores = scores;
			leaderboardInfo.localPlayerName = GetPlayerName();
			PlayGamesPlatform.Instance.LoadScores(leaderboardID, LeaderboardStart.PlayerCentered, 1, LeaderboardCollection.Public, LeaderboardTimeSpan.AllTime, delegate(LeaderboardScoreData playerData)
			{
				leaderboardInfo.localPlayerScore = playerData.Scores[0].value;
				leaderboardInfo.localPlayerRank = playerData.Scores[0].rank;
				callback();
			});
		});
	}*/
}
