using UnityEngine;

public static class StaticConstants
{
	public static readonly string PlayerSaveDataPath = Application.persistentDataPath + "/PlayerData.dat";

	public static readonly string SubscriptionLocalDataPath = Application.persistentDataPath + "/SubData.dat";

	public static readonly string SaltKey = "Salt";

	public static readonly string PasswordKey = "Password";

	public static readonly string DailyRewardKey = "Daily";

	public static readonly string SubscriptionRewardkey = "SubReward";

	public static readonly string DailyMultiplierTime = "DailyMultiplierTime";

	public static readonly string MoneyBoxKey = "MoneyBox";

	public static readonly string MusicMuted = "MusicMuted";

	public static readonly string MusicVolume = "MusicVolume";

	public static readonly string SoundMuted = "SoundMuted";

	public static readonly string SoundVolume = "SoundVolume";

	public static readonly string LastOnlineTime = "LastOnlineTime";

	public static readonly string TutorialCompleted = "GameplayTutorialComplete";

	public static readonly string AbilityTutorialCompleted = "AbilityTutorialComplete";

	public static readonly string UpgradeTutorialCompleted = "UpgradeTutorialComplete";

	public static readonly string GoToNextWorldPopUpShowed = "GoToNextWorldPopUpShowed";

	public static readonly string currentWorld = "CurrentWorld";

	public static readonly string interstitialAdsKey = "interstitialAdsKey";

	public static readonly string infinityMultiplierPurchased = "infinityMultiplierPurchased";

	public static readonly string starterPackPurchased = "starterPackPurchased";

	public static readonly string firstDailyClaim = "firstDailyClaim";

	public static readonly string lastSelectedLanguage = "lastselectedlanguage";

	public static readonly string allBossesRewardedkey = "allBossesRewarded";

	public static readonly string autoSignIn = "autoSignIn";

	public static readonly string LeagueTutorialCompleted = "LeagueTutorialCompleted";

	public static readonly int MultiplierDurationInSeconds = 21600;

	public static readonly int StarterPackHoursDuration = 48;

	public static readonly string MultiplierKey = "CurrentMultiplier";

	public static readonly float InGameGoldConst = 1f;

	public static readonly float IdleGoldConst = 0.45f;

	public static readonly float InGameExpConst = 0.3f;

	public static readonly float OfflineGoldConst = 0.0075f;

	public static readonly float MinOfflineMinutes = 30f;

	public static readonly float MaxOfflineMinutes = 480f;

	public static readonly float DailyCoinMultiplier = 1.65f;

	public static readonly float DailyCoinMultiplier2 = 100f;

	public static readonly float ZombiePowerConst = 53f;

	public static readonly float BossZombieMultiplier = 2f;

	public static readonly bool NeedInternetConnection = false;

	public static readonly byte[] CsvSalt = new byte[32]
	{
		229, 140, 177, 100, 137, 81, 225, 231, 18, 52,
		194, 51, 145, 147, 191, 136, 133, 95, 76, 45,
		63, 150, 154, 32, 56, 56, 99, 170, 113, 181,
		12, 165
	};

	public static readonly string CsvPass = "aca26bad-c47a-41a7-bf08-85565578d163";

	public static int[] bossStages = new int[4] { 1, 3, 5, 10 };

	public static byte[] Salt { get; private set; }

	public static string Password { get; private set; }

	public static float GetPercentageBetweenPoints(float current, float min, float max)
	{
		return (current - min) / (max - min);
	}

	public static bool IsConnectedToInternet()
	{
		if (NeedInternetConnection && Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		return true;
	}
}
