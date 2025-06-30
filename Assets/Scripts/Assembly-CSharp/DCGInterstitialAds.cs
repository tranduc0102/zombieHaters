/*using UnityEngine;

public class DCGInterstitialAds : MonoBehaviour
{
	public delegate void DCGInterAdsLoadSuccess(string slotId);

	public delegate void DCGInterAdsLoadError(string slotId, string errMsg);

	public delegate void DCGInterAdsDidClose(string slotId);

	public delegate void DCGInterAdsDidClicked(string slotId);

	public static event DCGInterAdsLoadSuccess onLoadSuccessHandler;

	public static event DCGInterAdsLoadError onLoadErrorHandler;

	public static event DCGInterAdsDidClose onCloseHandler;

	public static event DCGInterAdsDidClicked onClickedHandler;

	public static void LoadAd(string gameObejctName, string slotId)
	{
		Debug.Log("实现插屏广告回调");
		setAndroidDelegate();
		InterstitialAd.LoadAd(slotId);
	}

	public static void ShowAd(string gameObjectName, string slotId)
	{
		Debug.Log("展示插屏广告");
		setAndroidDelegate();
		InterstitialAd.Show(slotId);
	}

	public static bool IsAdReady(string gameObjectName, string slotId)
	{
		bool flag = false;
		return InterstitialAd.IsReady(slotId);
	}

	private static void setIOSDelegate()
	{
		DCGInterstitialAdsClient_iOS.onLoadSuccessHandler -= interLoadSuccess;
		DCGInterstitialAdsClient_iOS.onLoadSuccessHandler += interLoadSuccess;
		DCGInterstitialAdsClient_iOS.onLoadErrorHandler -= interLoadError;
		DCGInterstitialAdsClient_iOS.onLoadErrorHandler += interLoadError;
		DCGInterstitialAdsClient_iOS.onCloseHandler -= interAdClose;
		DCGInterstitialAdsClient_iOS.onCloseHandler += interAdClose;
		DCGInterstitialAdsClient_iOS.onClickedHandler -= interAdClicked;
		DCGInterstitialAdsClient_iOS.onClickedHandler += interAdClicked;
	}

	private static void setAndroidDelegate()
	{
		InterstitialAd.onLoadSuccessHandler -= interLoadSuccess;
		InterstitialAd.onLoadSuccessHandler += interLoadSuccess;
		InterstitialAd.onLoadErrorHandler -= interLoadError;
		InterstitialAd.onLoadErrorHandler += interLoadError;
		InterstitialAd.onAdClosedHandler -= interAdClose;
		InterstitialAd.onAdClosedHandler += interAdClose;
		InterstitialAd.onAdClickedHandler -= interAdClicked;
		InterstitialAd.onAdClickedHandler += interAdClicked;
	}

	public static void interLoadSuccess(string slotId)
	{
		Debug.Log("C# 接口层：广告加载成功");
		if (DCGInterstitialAds.onLoadSuccessHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGInterstitialAds.onLoadSuccessHandler(slotId);
		}
	}

	public static void interLoadError(string slotId, string errMsg)
	{
		Debug.Log("C# 接口层：广告加载失败");
		if (DCGInterstitialAds.onLoadErrorHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGInterstitialAds.onLoadErrorHandler(slotId, errMsg);
		}
	}

	public static void interAdClose(string slotId)
	{
		Debug.Log("C# 接口层：广告加载成功");
		if (DCGInterstitialAds.onCloseHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGInterstitialAds.onCloseHandler(slotId);
		}
	}

	public static void interAdClicked(string slotId)
	{
		Debug.Log("C# 接口层：广告加载成功");
		if (DCGInterstitialAds.onClickedHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGInterstitialAds.onClickedHandler(slotId);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
*/