/*using UnityEngine;

public class DCGBannerAds : MonoBehaviour
{
	public delegate void DCGBannerAdsLoadSuccess(string slotId);

	public delegate void DCGBannerAdsLoadError(string slotId, string errMsg);

	public delegate void DCGBannerAdsDidClose(string slotId);

	public delegate void DCGBannerAdsDidClicked(string slotId);

	public static event DCGBannerAdsLoadSuccess onLoadSuccessHandler;

	public static event DCGBannerAdsLoadError onLoadErrorHandler;

	public static event DCGBannerAdsDidClose onCloseHandler;

	public static event DCGBannerAdsDidClicked onClickedHandler;

	public static void ShowAd(string gameObejctName, string slotId, int postion)
	{
		Debug.Log("实现Banner广告回调");
		setAndroidDelegate();
		BannerAd.ShowAd(slotId, postion);
	}

	public static void DestroyAd(string gameObjectName, string slotId)
	{
		BannerAd.DestroyAd(slotId);
	}

	private static void setIOSDelegate()
	{
		DCGBannerAdsClient_iOS.onLoadSuccessHandler -= bannerLoadSuccess;
		DCGBannerAdsClient_iOS.onLoadSuccessHandler += bannerLoadSuccess;
		DCGBannerAdsClient_iOS.onLoadErrorHandler -= bannerLoadError;
		DCGBannerAdsClient_iOS.onLoadErrorHandler += bannerLoadError;
		DCGBannerAdsClient_iOS.onCloseHandler -= bannerAdClose;
		DCGBannerAdsClient_iOS.onCloseHandler += bannerAdClose;
		DCGBannerAdsClient_iOS.onClickedHandler -= bannerAdClicked;
		DCGBannerAdsClient_iOS.onClickedHandler += bannerAdClicked;
	}

	private static void setAndroidDelegate()
	{
		BannerAd.onLoadSuccessHandler -= bannerLoadSuccess;
		BannerAd.onLoadSuccessHandler += bannerLoadSuccess;
		BannerAd.onLoadErrorHandler -= bannerLoadError;
		BannerAd.onLoadErrorHandler += bannerLoadError;
		BannerAd.onAdClosedHandler -= bannerAdClose;
		BannerAd.onAdClosedHandler += bannerAdClose;
		BannerAd.onAdClickedHandler -= bannerAdClicked;
		BannerAd.onAdClickedHandler += bannerAdClicked;
	}

	public static void bannerLoadSuccess(string slotId)
	{
		Debug.Log("C# 接口层：广告加载成功");
		if (DCGBannerAds.onLoadSuccessHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGBannerAds.onLoadSuccessHandler(slotId);
		}
	}

	public static void bannerLoadError(string slotId, string errMsg)
	{
		Debug.Log("C# 接口层：广告加载失败");
		if (DCGBannerAds.onLoadErrorHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGBannerAds.onLoadErrorHandler(slotId, errMsg);
		}
	}

	public static void bannerAdClose(string slotId)
	{
		Debug.Log("C# 接口层：广告关闭");
		if (DCGBannerAds.onCloseHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGBannerAds.onCloseHandler(slotId);
		}
	}

	public static void bannerAdClicked(string slotId)
	{
		Debug.Log("C# 接口层：广告被点击");
		if (DCGBannerAds.onClickedHandler != null)
		{
			Debug.Log("收到加载回调，向上层回传");
			DCGBannerAds.onClickedHandler(slotId);
		}
	}
}
*/