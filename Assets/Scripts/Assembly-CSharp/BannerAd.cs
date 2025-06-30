/*using System.Collections.Generic;
using DCGJSON;
using UnityEngine;

public class BannerAd : MonoBehaviour
{
	public delegate void OnLoadSuccess(string slotId);

	public delegate void OnError(string slotId, string error);

	public delegate void OnAdClosed(string slotId);

	public delegate void OnAdClicked(string slotId);

	private class InterListener : AndroidJavaProxy
	{
		public InterListener()
			: base("mobi.android.base.BannerAdListener")
		{
		}

		public void onAdLoaded(string slotId, AndroidJavaObject javaObject)
		{
			if (BannerAd.onLoadSuccessHandler == null)
			{
			}
		}

		public void onError(string slotId, string error)
		{
			if (BannerAd.onLoadErrorHandler == null)
			{
			}
		}

		public void onAdClosed(string slotId)
		{
			if (BannerAd.onAdClosedHandler == null)
			{
			}
		}

		public void onAdClicked(string slotId)
		{
			if (BannerAd.onAdClickedHandler == null)
			{
			}
		}
	}

	private static AndroidJavaObject bannerAdClient;

	public static int POSTION_TOP = 1;

	public static int POSTION_BOTTOM = 2;

	private static string functionName = "functionName";

	private static string mSlotId = "slotId";

	private static string error = "error";

	private static string isReward = "isReward";

	public static event OnLoadSuccess onLoadSuccessHandler;

	public static event OnError onLoadErrorHandler;

	public static event OnAdClosed onAdClosedHandler;

	public static event OnAdClicked onAdClickedHandler;

	public static void ShowAd(string slotId, int postion)
	{
		if (bannerAdClient == null)
		{
			bannerAdClient = new AndroidJavaObject("mobi.android.BannerAdH");
		}
		bannerAdClient.CallStatic("showAd", slotId, postion, new InterListener());
	}

	public static void DestroyAd(string slotId)
	{
		if (bannerAdClient == null)
		{
			bannerAdClient = new AndroidJavaObject("mobi.android.BannerAdH");
		}
		bannerAdClient.CallStatic("destroyAd", slotId);
	}

	public void AndroidBannerAdsCallBack(string data)
	{
		Debug.Log("get Android callback data: " + data);
		DataParse(data);
	}

	public static void DataParse(string data)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(data) as Dictionary<string, object>;
		string text = ((!dictionary.ContainsKey(functionName)) ? string.Empty : dictionary[functionName].ToString());
		string text2 = ((!dictionary.ContainsKey(mSlotId)) ? string.Empty : dictionary[mSlotId].ToString());
		string text3 = ((!dictionary.ContainsKey(error)) ? string.Empty : dictionary[error].ToString());
		string text4 = ((!dictionary.ContainsKey(isReward)) ? string.Empty : dictionary[isReward].ToString());
		Debug.Log("slotId: " + text2 + " ,funcName: " + text + " ,errMsg: " + text3);
		if (text.Length == 0 || text2.Length == 0)
		{
			Debug.Log("callback data invalid");
			return;
		}
		switch (text)
		{
		case "onAdLoaded":
			Debug.Log("Unity banner ad laod success！");
			if (BannerAd.onLoadSuccessHandler != null)
			{
				BannerAd.onLoadSuccessHandler(text2);
			}
			break;
		case "onError":
			Debug.Log("Unity banner ad laod error！");
			if (BannerAd.onLoadErrorHandler != null)
			{
				BannerAd.onLoadErrorHandler(text2, error);
			}
			break;
		case "onAdClosed":
			Debug.Log("Unity banner ad closed！");
			if (BannerAd.onAdClosedHandler != null)
			{
				BannerAd.onAdClosedHandler(text2);
			}
			break;
		case "onAdClicked":
			Debug.Log("Unity banner ad clicked！");
			if (BannerAd.onAdClickedHandler != null)
			{
				BannerAd.onAdClickedHandler(text2);
			}
			break;
		default:
			Debug.Log("no callback type");
			break;
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