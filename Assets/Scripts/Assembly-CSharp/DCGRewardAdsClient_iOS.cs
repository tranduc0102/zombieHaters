/*using System.Collections.Generic;
using System.Runtime.InteropServices;
using DCGJSON;
using UnityEngine;

public class DCGRewardAdsClient_iOS : MonoBehaviour
{
	public delegate void DCGRewardAdsLoadSuccess(string slotId);

	public delegate void DCGRewardAdsLoadError(string slotId, string errorMsg);

	public delegate void DCGRewardAdsDidShow(string slotId);

	public delegate void DCGRewardAdsDidClose(string slotId, bool shouldReward);

	public delegate void DCGRewardAdsFailedToOpen(string slotId, string errMsg);

	public delegate void DCGRewardAdsDidClickAds(string slotId);

	private static string callBackFunc = "callbackFunc";

	private static string slotIdKey = "slotIdKey";

	private static string errorMsgKey = "errorMsg";

	private static DCGRewardAdsClient_iOS instance = new DCGRewardAdsClient_iOS();

	public static DCGRewardAdsClient_iOS Instance
	{
		get
		{
			return instance;
		}
	}

	public static event DCGRewardAdsLoadSuccess OnLoadSuccess;

	public static event DCGRewardAdsLoadError OnLoadError;

	public static event DCGRewardAdsDidShow OnShow;

	public static event DCGRewardAdsDidClose OnClose;

	public static event DCGRewardAdsFailedToOpen OnShowError;

	public static event DCGRewardAdsDidClickAds OnClick;

	private DCGRewardAdsClient_iOS()
	{
	}

	[DllImport("__Internal")]
	internal static extern void DCGRewardAdsLoadAd(string gameObject, string slotId);

	[DllImport("__Internal")]
	internal static extern void DCGRewardAdsPlay(string gameObject, string slotId);

	[DllImport("__Internal")]
	internal static extern bool DCGRewardIsAdsReady(string slotId);

	public static void loadRewardAd(string gameObejctName, string slotId)
	{
		DCGRewardAdsLoadAd(gameObejctName, slotId);
	}

	public static void playRewardAd(string gameObjectName, string slotId)
	{
		DCGRewardAdsPlay(gameObjectName, slotId);
	}

	public static bool isRewardAdReady(string gameObjectName, string slotId)
	{
		return DCGRewardIsAdsReady(slotId);
	}

	public void OCRewardAdsCallBack(string data)
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
		string text4 = ((!dictionary.ContainsKey("shouldReward")) ? string.Empty : dictionary["shouldReward"].ToString());
		Debug.Log("slotId:" + text2 + ",funcName:" + text + ",errMsg:" + text3);
		if (text.Length == 0 || text2.Length == 0)
		{
			Debug.Log("回调参数不全");
			return;
		}
		switch (text)
		{
		case "AdLoadSuccess":
			Debug.Log("Unity激励视频加载成功！");
			if (DCGRewardAdsClient_iOS.OnLoadSuccess != null)
			{
				DCGRewardAdsClient_iOS.OnLoadSuccess(text2);
			}
			break;
		case "AdLoadError":
			Debug.Log("Unity激励视频加载失败！");
			if (DCGRewardAdsClient_iOS.OnLoadError != null)
			{
				DCGRewardAdsClient_iOS.OnLoadError(text2, text3);
			}
			break;
		case "AdDidOpen":
			Debug.Log("Unity激励视频已经展示！");
			if (DCGRewardAdsClient_iOS.OnShow != null)
			{
				DCGRewardAdsClient_iOS.OnShow(text2);
			}
			break;
		case "AdDidClose":
		{
			Debug.Log("Unity激励视频已经关闭！");
			bool shouldReward = text4.Equals("true");
			if (DCGRewardAdsClient_iOS.OnClose != null)
			{
				DCGRewardAdsClient_iOS.OnClose(text2, shouldReward);
			}
			break;
		}
		case "DidClickAds":
			Debug.Log("Unity激励视频已经点击广告！");
			if (DCGRewardAdsClient_iOS.OnClick != null)
			{
				DCGRewardAdsClient_iOS.OnClick(text2);
			}
			break;
		case "AdFailedToOpen":
			Debug.Log("Unity激励视频打开失败！");
			if (DCGRewardAdsClient_iOS.OnShowError != null)
			{
				DCGRewardAdsClient_iOS.OnShowError(text2, text3);
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