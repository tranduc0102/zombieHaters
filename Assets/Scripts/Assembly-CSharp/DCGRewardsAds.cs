/*using UnityEngine;

public class DCGRewardsAds : MonoBehaviour
{
	public delegate void DCGRewardAdsLoadSuccess(string slotId);

	public delegate void DCGRewardAdsLoadError(string slotId, string errMsg);

	public delegate void DCGRewardAdsDidShow(string slotId);

	public delegate void DCGRewardAdsDidClose(string slotId, bool shouldReward);

	public delegate void DCGRewardAdsFailedToShow(string slotId, string errMsg);

	public static event DCGRewardAdsLoadSuccess onLoadSuccessHandler;

	public static event DCGRewardAdsLoadError onLoadErrorHandler;

	public static event DCGRewardAdsDidShow onShowHandler;

	public static event DCGRewardAdsDidClose onCloseHandler;

	public static event DCGRewardAdsFailedToShow onShowErrorHandler;

	public static void LoadRewardAd(string gameObejctName, string slotId)
	{
		Debug.Log("实现激励视频回调");
		setAndroidDelegate();
		RewardAd.LoadAd(slotId);
	}

	public static void PlayRewardAd(string gameObjectName, string slotId)
	{
		Debug.Log("播放激励视频");
		setAndroidDelegate();
		RewardAd.Show(slotId);
	}

	public static bool IsAdReady(string gameObjectName, string slotId)
	{
		bool flag = false;
		return RewardAd.IsReady(slotId);
	}

	private static void setIOSDelegate()
	{
		DCGRewardAdsClient_iOS.OnLoadSuccess -= rewardLoadSuccess;
		DCGRewardAdsClient_iOS.OnLoadSuccess += rewardLoadSuccess;
		DCGRewardAdsClient_iOS.OnLoadError -= rewardLoadError;
		DCGRewardAdsClient_iOS.OnLoadError += rewardLoadError;
		DCGRewardAdsClient_iOS.OnShow -= rewardOnShow;
		DCGRewardAdsClient_iOS.OnShow += rewardOnShow;
		DCGRewardAdsClient_iOS.OnClose -= rewardOnClose;
		DCGRewardAdsClient_iOS.OnClose += rewardOnClose;
		DCGRewardAdsClient_iOS.OnShowError -= rewardOnShowError;
		DCGRewardAdsClient_iOS.OnShowError += rewardOnShowError;
	}

	private static void setAndroidDelegate()
	{
		RewardAd.onLoadSuccessHandler -= rewardLoadSuccess;
		RewardAd.onLoadSuccessHandler += rewardLoadSuccess;
		RewardAd.onLoadErrorHandler -= rewardLoadError;
		RewardAd.onLoadErrorHandler += rewardLoadError;
		RewardAd.onShowStartHandler -= rewardOnShow;
		RewardAd.onShowStartHandler += rewardOnShow;
		RewardAd.onShowFinishHandler -= rewardOnClose;
		RewardAd.onShowFinishHandler += rewardOnClose;
		RewardAd.onShowErrorHandler -= rewardOnShowError;
		RewardAd.onShowErrorHandler += rewardOnShowError;
	}

	public static void rewardLoadSuccess(string slotId)
	{
		Debug.Log("C# 接口层：广告加载成功");
		if (DCGRewardsAds.onLoadSuccessHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGRewardsAds.onLoadSuccessHandler(slotId);
		}
	}

	public static void rewardLoadError(string slotId, string errMsg)
	{
		Debug.Log("C# 接口层：广告加载失败");
		if (DCGRewardsAds.onLoadErrorHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGRewardsAds.onLoadErrorHandler(slotId, errMsg);
		}
	}

	public static void rewardOnShow(string slotId)
	{
		Debug.Log("C# 接口层：广告已经展示");
		if (DCGRewardsAds.onShowHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGRewardsAds.onShowHandler(slotId);
		}
	}

	public static void rewardOnClose(string slotId, bool shouldReward)
	{
		Debug.Log("C# 接口层：广告已经关闭");
		if (DCGRewardsAds.onCloseHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGRewardsAds.onCloseHandler(slotId, shouldReward);
		}
	}

	public static void rewardOnShowError(string slotId, string errMsg)
	{
		Debug.Log("C# 接口层：广告播放错误");
		if (DCGRewardsAds.onShowErrorHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGRewardsAds.onShowErrorHandler(slotId, errMsg);
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
	}
}
*/