/*using System.Collections.Generic;
using DCGJSON;
using UnityEngine;

public class InterstitialAd : MonoBehaviour
{
	public delegate void OnLoadSuccess(string slotId);

	public delegate void OnError(string slotId, string error);

	public delegate void OnAdClosed(string slotId);

	public delegate void OnAdClicked(string slotId);

	private static AndroidJavaObject interAdClient;

	private static string functionName = "functionName";

	private static string mSlotId = "slotId";

	private static string error = "error";

	private static string isReward = "isReward";

	public static event OnLoadSuccess onLoadSuccessHandler;

	public static event OnError onLoadErrorHandler;

	public static event OnAdClosed onAdClosedHandler;

	public static event OnAdClicked onAdClickedHandler;

	public static void LoadAd(string slotId)
	{
		if (interAdClient == null)
		{
			interAdClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		interAdClient.CallStatic("loadInterstitialAd", slotId);
	}

	public static bool IsReady(string slotId)
	{
		if (interAdClient == null)
		{
			interAdClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		return interAdClient.CallStatic<bool>("isInterstitialAdReady", new object[1] { slotId });
	}

	public static void Show(string slotId)
	{
		if (interAdClient == null)
		{
			interAdClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		interAdClient.CallStatic("showInterstitialAd", slotId);
	}

	public void AndroidInterstitialAdsCallBack(string data)
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
			Debug.Log("Unity interstital ad laod success！");
			if (InterstitialAd.onLoadSuccessHandler != null)
			{
				InterstitialAd.onLoadSuccessHandler(text2);
			}
			break;
		case "onError":
			Debug.Log("Unity interstital ad laod error！");
			if (InterstitialAd.onLoadErrorHandler != null)
			{
				InterstitialAd.onLoadErrorHandler(text2, error);
			}
			break;
		case "onAdClosed":
			Debug.Log("Unity interstital ad closed！");
			if (InterstitialAd.onAdClosedHandler != null)
			{
				InterstitialAd.onAdClosedHandler(text2);
			}
			break;
		case "onAdClicked":
			Debug.Log("Unity interstital ad clicked！");
			if (InterstitialAd.onAdClickedHandler != null)
			{
				InterstitialAd.onAdClickedHandler(text2);
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