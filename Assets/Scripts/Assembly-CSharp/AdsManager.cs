/*using IAP;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
	public enum AdName
	{
		Interstitial = 80001,
		RewardMoreSurvival = 80002,
		RewardKillAll = 80003,
		RewardMulriplierTime = 80004,
		RewardUpgradeHero = 80005,
		RewardOpenCard = 80006,
		RewardPresentBox = 80007,
		RewardX2CoinsPresent = 80008,
		RewardSpin = 80009,
		RewardX2GameOver = 80010,
		RewardX3Offline = 80012,
		RewardIncRatingArena = 80013,
		RewardIncRatingPVP = 80014
	}

	public delegate void RewardedDel();

	[SerializeField]
	private string pubIdIOS = "10221";

	[SerializeField]
	private string pubIdAndroid = "10220";

	[HideInInspector]
	public int interstitialAdsCounter;

	private RewardedDel currentDel;

	private bool bannerLoaded;

	public static AdsManager instance { get; private set; }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		Debug.Log("Call ads initialize");
		DCGAds.InitializeSDK(base.gameObject.name, "http://mo.dotjoy.io", pubIdAndroid, "FvijHlomwg8xwxds");
	}

	private void Start()
	{
		DCGRewardsAds.onShowHandler += rewardAdOnShowStart;
		DCGRewardsAds.onCloseHandler += rewardAdOnShowFinish;
		DCGRewardsAds.onShowErrorHandler += rewardAdOnShowError;
		DCGInterstitialAds.onCloseHandler += interAdOnClose;
		DCGInterstitialAds.onClickedHandler += interAdOnClicked;
		Invoke("LoadRewaredAds", 2f);
		if (!PlayerPrefs.HasKey(StaticConstants.interstitialAdsKey))
		{
			DCGInterstitialAds.LoadAd(base.gameObject.name, 80001.ToString());
		}
	}

	private void LoadRewaredAds()
	{
		Debug.Log("Loading rewarded ads");
		DCGRewardsAds.LoadRewardAd(base.gameObject.name, 80005.ToString());
	}

	public void ShowInterstitial()
	{
		if (!PlayerPrefs.HasKey(StaticConstants.interstitialAdsKey) && !InAppManager.Instance.IsSubscribed())
		{
			string slotId = 80001.ToString();
			if (DCGInterstitialAds.IsAdReady(base.gameObject.name, slotId))
			{
				DCGInterstitialAds.ShowAd(base.gameObject.name, slotId);
			}
			DCGInterstitialAds.LoadAd(base.gameObject.name, slotId);
		}
	}

	public void ShowRewarded(RewardedDel del, AdName adName)
	{
		int num = (int)adName;
		string slotId = num.ToString();
		if (DCGRewardsAds.IsAdReady(base.gameObject.name, slotId))
		{
			currentDel = del;
			DCGRewardsAds.PlayRewardAd(base.gameObject.name, slotId);
		}
		DCGRewardsAds.LoadRewardAd(base.gameObject.name, slotId);
	}

	public void TestSuite()
	{
	}

	public bool ShowBanner()
	{
		if (bannerLoaded && !NoAdsManager.instance.NoAdsPurchased())
		{
			return true;
		}
		return false;
	}

	public void HideBanner()
	{
	}

	public void DecreaseInterstitialCounter()
	{
		if (interstitialAdsCounter == 0)
		{
			interstitialAdsCounter--;
		}
	}

	public void SetBannerListener(bool disable = false)
	{
	}

	public bool IsRewardedVideoAvailable(AdName adName)
	{
		int num = (int)adName;
		string slotId = num.ToString();
		return DCGRewardsAds.IsAdReady(base.gameObject.name, slotId);
	}

	private void rewardAdOnLoadSuccess(string slotId)
	{
		Log("reward ad on load success , slotid: " + slotId);
	}

	private void rewardAdOnLoadError(string slotId, string error)
	{
		Log("reward ad on load error, slotid: " + slotId + " ,error: " + error);
	}

	private void rewardAdOnShowStart(string slotId)
	{
		SoundManager.Instance.MuteAll();
		Log("reward ad on show start , slotid: " + slotId);
	}

	private void rewardAdOnShowFinish(string slotId, bool isReward)
	{
		if (isReward)
		{
			currentDel();
		}
		SoundManager.Instance.UnMuteAll();
		Log("reward ad on show finish , slotid: " + slotId + " ,isReward: " + isReward);
	}

	private void rewardAdOnShowError(string slotId, string error)
	{
		SoundManager.Instance.UnMuteAll();
		Log("reward ad on show error, slotid: " + slotId + " ,error: " + error);
	}

	private void interAdOnLoadSuccess(string slotId)
	{
		Log("interstitial ad on laod success, slotId:" + slotId);
	}

	private void interAdOnLoadError(string slotId, string error)
	{
		Log("interstitial ad on laod error, slotId:" + slotId + " ,error: " + error);
	}

	private void interAdOnClose(string slotId)
	{
		Log("interstitial ad on close, slotId:" + slotId);
	}

	private void interAdOnClicked(string slotId)
	{
		Log("interstitial ad on clicked, slotId:" + slotId);
	}

	private void Log(string msg)
	{
		Debug.Log("ADS: " + msg);
	}
}
*/