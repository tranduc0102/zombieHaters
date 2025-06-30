/*using System.Collections.Generic;
using DCGJSON;
using UnityEngine;

public class RewardAd : MonoBehaviour
{
	public delegate void OnLoadSuccess(string slotId);

	public delegate void OnLoadError(string slotId, string error);

	public delegate void OnShowStart(string slotId);

	public delegate void OnShowFinish(string slotId, bool isReward);

	public delegate void OnShowError(string slotId, string error);

	private static AndroidJavaObject rewardClient;

	private static string functionName = "functionName";

	private static string mSlotId = "slotId";

	private static string error = "error";

	private static string isReward = "isReward";

	public static event OnLoadSuccess onLoadSuccessHandler;

	public static event OnLoadError onLoadErrorHandler;

	public static event OnShowStart onShowStartHandler;

	public static event OnShowFinish onShowFinishHandler;

	public static event OnShowError onShowErrorHandler;

	public static void LoadAd(string slotId)
	{
		if (rewardClient == null)
		{
			rewardClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		rewardClient.CallStatic("loadRewardAd", slotId);
	}

	public static bool IsReady(string slotId)
	{
		if (rewardClient == null)
		{
			rewardClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		return rewardClient.CallStatic<bool>("isRewardAdReady", new object[1] { slotId });
	}

	public static void Show(string slotId)
	{
		if (rewardClient == null)
		{
			rewardClient = new AndroidJavaObject("mobi.android.StormSdkH");
		}
		rewardClient.CallStatic("showRewardAd", slotId);
	}

	public void AndroidRewardAdsCallBack(string data)
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
		Debug.Log("slotId: " + text2 + " ,funcName: " + text + " ,errMsg: " + text3 + " , isReward:" + text4);
		if (text.Length == 0 || text2.Length == 0)
		{
			Debug.Log("callback data invalid");
			return;
		}
		switch (text)
		{
		case "onLoad":
			Debug.Log("Unity reward ad laod success！");
			if (RewardAd.onLoadSuccessHandler != null)
			{
				RewardAd.onLoadSuccessHandler(text2);
			}
			break;
		case "onLoadError":
			Debug.Log("Unity reward ad laod error！");
			if (RewardAd.onLoadErrorHandler != null)
			{
				RewardAd.onLoadErrorHandler(text2, error);
			}
			break;
		case "onStart":
			Debug.Log("Unity reward ad show success！");
			if (RewardAd.onShowStartHandler != null)
			{
				RewardAd.onShowStartHandler(text2);
			}
			break;
		case "onFinish":
		{
			Debug.Log("Unity reward ad closed！");
			bool flag = text4.Equals("true");
			if (RewardAd.onShowFinishHandler != null)
			{
				RewardAd.onShowFinishHandler(text2, flag);
			}
			break;
		}
		case "onClick":
			Debug.Log("Unity reward ad click！");
			break;
		case "onShowError":
			Debug.Log("Unity reward ad show error！");
			if (RewardAd.onShowErrorHandler != null)
			{
				RewardAd.onShowErrorHandler(text2, error);
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