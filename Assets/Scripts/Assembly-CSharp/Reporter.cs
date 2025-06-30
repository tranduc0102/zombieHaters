public class Reporter
{
	public static void Init(string url, string appFlyerKey, string trafficId, string channel, string buglyId, string appId, string encodeKey, string decodeKey, string itunesKey)
	{
		AnalyticsSdk.Init(url, appFlyerKey, trafficId, channel, buglyId, appId, encodeKey, decodeKey);
	}

	public static void SetDebug(bool isDebug)
	{
		AnalyticsSdk.SetDebug(isDebug);
	}

	public static void SendEvent(string cat, string act, string lab, string val, string extra, string eid)
	{
		AnalyticsSdk.SendEvent(cat, act, lab, val, extra, eid);
	}

	public static void SetProperty(string name, string value)
	{
		AnalyticsSdk.SetProperty(name, value);
	}

	public static void SendRealActiveEvent()
	{
		AnalyticsSdk.SendRealActiveEvent();
	}
}
