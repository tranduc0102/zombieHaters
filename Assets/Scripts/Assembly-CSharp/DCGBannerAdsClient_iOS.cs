/*using System.Collections.Generic;
using System.Runtime.InteropServices;
using DCGJSON;
using UnityEngine;

public class DCGBannerAdsClient_iOS : MonoBehaviour
{
	public delegate void DCGBannerAdsLoadSuccess(string slotId);

	public delegate void DCGBannerAdsLoadError(string slotId, string errMsg);

	public delegate void DCGBannerAdsClosed(string slotId);

	public delegate void DCGBannerAdsDidClicked(string slotId);

	private static string callBackFunc = "callbackFunc";

	private static string slotIdKey = "slotIdKey";

	private static string errorMsgKey = "errorMsg";

	public static event DCGBannerAdsLoadSuccess onLoadSuccessHandler;

	public static event DCGBannerAdsLoadError onLoadErrorHandler;

	public static event DCGBannerAdsClosed onCloseHandler;

	public static event DCGBannerAdsDidClicked onClickedHandler;

	[DllImport("__Internal")]
	internal static extern void DCGBannerAdsShow(string gameObjectName, string slotId, int position);

	[DllImport("__Internal")]
	internal static extern void DCGBannerAdsDestroy(string gameObjectName, string slotId);

	public static void showBannerAd(string gameObejctName, string slotId, int position)
	{
		DCGBannerAdsShow(gameObejctName, slotId, position);
	}

	public static void destroyBannerAd(string gameObejctName, string slotId)
	{
		DCGBannerAdsDestroy(gameObejctName, slotId);
	}

	public void OCBannerAdsCallBack(string data)
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
			Debug.Log("Banner广告加载成功！");
			if (DCGBannerAdsClient_iOS.onLoadSuccessHandler != null)
			{
				DCGBannerAdsClient_iOS.onLoadSuccessHandler(text2);
			}
			break;
		case "AdLoadError":
			Debug.Log("Banner广告加载失败！");
			if (DCGBannerAdsClient_iOS.onLoadErrorHandler != null)
			{
				DCGBannerAdsClient_iOS.onLoadErrorHandler(text2, text3);
			}
			break;
		case "AdDidClose":
			Debug.Log("Banner广告已经关闭！");
			if (DCGBannerAdsClient_iOS.onCloseHandler != null)
			{
				DCGBannerAdsClient_iOS.onCloseHandler(text2);
			}
			break;
		case "DidClickAds":
			Debug.Log("Banner广告已经点击广告！");
			if (DCGBannerAdsClient_iOS.onClickedHandler != null)
			{
				DCGBannerAdsClient_iOS.onClickedHandler(text2);
			}
			break;
		default:
			Debug.Log("无此回调类型");
			break;
		}
	}
}
*/