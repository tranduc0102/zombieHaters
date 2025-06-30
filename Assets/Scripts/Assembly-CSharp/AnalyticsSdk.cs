/*using AndroidSdk.Utils;
*/using UnityEngine;

public class AnalyticsSdk
{
	private static AndroidJavaObject anaObj;

	public static void Init(string url, string appFlyerKey, string trafficId, string channel, string buglyId, string appId, string encodeKey, string decodeKey)
	{
		/*AndroidJavaObject androidJavaObject = new AndroidJavaObject("mobi.anasutil.anay.lite.AnalyticsSdk");
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("mobi.anasutil.anay.lite.AnalyticsBuilder$Builder");
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setAnalyticsUrl", new object[1] { url });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setAppsFlyerKey", new object[1] { appFlyerKey });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setTrafficId", new object[1] { trafficId });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setInstallChannel", new object[1] { channel });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setBuglyAppId", new object[1] { buglyId });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setAppId", new object[1] { appId });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setEncodeKey", new object[1] { encodeKey });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("setDecodeKey", new object[1] { decodeKey });
		androidJavaObject2 = androidJavaObject2.Call<AndroidJavaObject>("build", new object[0]);
		androidJavaObject.CallStatic("init", Utils.GetUnityObject(), androidJavaObject2);
		anaObj = androidJavaObject;*/
	}

	public static void SendEvent(string cat, string act, string lab, string val, string extra, string eid)
	{
		if (anaObj != null)
		{
			anaObj.CallStatic("sendEvent", cat, act, lab, val, extra, eid);
		}
	}

	public static void SetDebug(bool isDebug)
	{
		if (anaObj != null)
		{
			anaObj.CallStatic("setDebugMode", isDebug);
		}
	}

	public static void SendRealActiveEvent()
	{
		if (anaObj != null)
		{
			anaObj.CallStatic("sendRealActiveEvent");
		}
	}

	public static void SetProperty(string name, string value)
	{
		if (anaObj != null)
		{
			anaObj.CallStatic("setProperty", name, value);
		}
	}
}
