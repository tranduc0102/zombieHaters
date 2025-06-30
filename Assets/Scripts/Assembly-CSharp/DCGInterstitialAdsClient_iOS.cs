/*using System.Collections.Generic;
using System.Runtime.InteropServices;
using DCGJSON;
using UnityEngine;

public class DCGInterstitialAdsClient_iOS : MonoBehaviour
{
	public delegate void DCGInterstitialAdsLoadSuccess(string slotId);

	public delegate void DCGInterstitialAdsLoadError(string slotId, string errMsg);

	public delegate void DCGInterstitialAdsDidClose(string slotId);

	public delegate void DCGInterstitialAdsDidClicked(string slotId);

	private static string callBackFunc = "callbackFunc";

	private static string slotIdKey = "slotIdKey";

	private static string errorMsgKey = "errorMsg";

	public static event DCGInterstitialAdsLoadSuccess onLoadSuccessHandler;

	public static event DCGInterstitialAdsLoadError onLoadErrorHandler;

	public static event DCGInterstitialAdsDidClose onCloseHandler;

	public static event DCGInterstitialAdsDidClicked onClickedHandler;

	[DllImport("__Internal")]
	internal static extern void DCGInterstitialAdsLoad(string gameObjectName, string slotId);

	[DllImport("__Internal")]
	internal static extern void DCGInterstitialAdsPlay(string gameObjectName, string slotId);

	[DllImport("__Internal")]
	internal static extern bool DCGInterstitialAdsReady(string slotId);

	public static void loadInterstitialAd(string gameObejctName, string slotId)
	{
		DCGInterstitialAdsLoad(gameObejctName, slotId);
	}

	public static void playInterstitialAd(string gameObejctName, string slotId)
	{
		DCGInterstitialAdsPlay(gameObejctName, slotId);
	}

	public static bool isInterstitialAdReady(string slotId)
	{
		return DCGInterstitialAdsReady(slotId);
	}

	public void OCInterstitialAdsCallBack(string data)
	{
		Debug.Log("收到加载回调，向上层回传");
		Debug.Log("回调数据为:" + data);
		DataParse(data);
	}

	public static void DataParse(string data)
	{
		Debug.Log("进入了方法");
		Debug.Log("传入的data为:" + data);
		Dictionary<string, object> dictionary = Json.Deserialize(data) as Dictionary<string, object>;
		Debug.Log("解析的dic为:" + dictionary);
		string text = ((!dictionary.ContainsKey(callBackFunc)) ? string.Empty : dictionary[callBackFunc].ToString());
		string text2 = ((!dictionary.ContainsKey(slotIdKey)) ? string.Empty : dictionary[slotIdKey].ToString());
		string text3 = ((!dictionary.ContainsKey(errorMsgKey)) ? string.Empty : dictionary[errorMsgKey].ToString());
		Debug.Log("slotId:" + text2 + ",funcName:" + text + ",errMsg:" + text3);
		if (text.Length == 0 || text2.Length == 0)
		{
			Debug.Log("回调参数不全");
			return;
		}
		switch (text)
		{
		case "AdLoadSuccess":
			Debug.Log("插屏广告加载成功！");
			if (DCGInterstitialAdsClient_iOS.onLoadSuccessHandler != null)
			{
				DCGInterstitialAdsClient_iOS.onLoadSuccessHandler(text2);
			}
			break;
		case "AdLoadError":
			Debug.Log("插屏广告加载失败！");
			if (DCGInterstitialAdsClient_iOS.onLoadErrorHandler != null)
			{
				DCGInterstitialAdsClient_iOS.onLoadErrorHandler(text2, text3);
			}
			break;
		case "AdDidClose":
			Debug.Log("插屏广告已经关闭！");
			if (DCGInterstitialAdsClient_iOS.onCloseHandler != null)
			{
				DCGInterstitialAdsClient_iOS.onCloseHandler(text2);
			}
			break;
		case "DidClickAds":
			Debug.Log("插屏广告已经点击广告！");
			if (DCGInterstitialAdsClient_iOS.onClickedHandler != null)
			{
				DCGInterstitialAdsClient_iOS.onClickedHandler(text2);
			}
			break;
		case "AdFailedToOpen":
			Debug.Log("插屏广告打开失败！");
			if (DCGInterstitialAdsClient_iOS.onLoadErrorHandler != null)
			{
				DCGInterstitialAdsClient_iOS.onLoadErrorHandler(text2, text3);
			}
			break;
		default:
			Debug.Log("无此回调类型");
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