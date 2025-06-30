using System.Runtime.InteropServices;
using UnityEngine;

public class DCGReporter_iOS : MonoBehaviour
{
	[DllImport("__Internal")]
	internal static extern void DCGReporter_Init(string appId, string encodeKey, string decodeKey, string serverUrl, string appsFlyerKey, string itunesId, string trafficId, string installChannel, string buglyId);

	[DllImport("__Internal")]
	internal static extern void DCGReporterSetReportInterval(int interval);

	[DllImport("__Internal")]
	internal static extern void DCGReporterSetLocation(string lati, string longi);

	[DllImport("__Internal")]
	internal static extern void DCGReporterSetDebug(bool debug);

	[DllImport("__Internal")]
	internal static extern void DCGReporterSendDailyActive();

	[DllImport("__Internal")]
	internal static extern void DCGReporterTrackEvent(string category, string action, string label, string value, string eid, string extra);

	[DllImport("__Internal")]
	internal static extern void DCGReporterSetProperty(string name, string value);

	public static void InitReportSDK(string appId, string encodeKey, string decodeKey, string serverUrl, string appsFlyerKey, string itunesId, string trafficId, string installChannel, string buglyId)
	{
		DCGReporter_Init(appId, encodeKey, decodeKey, serverUrl, appsFlyerKey, itunesId, trafficId, installChannel, buglyId);
	}

	public static void SetReportInterval(int interval)
	{
		DCGReporterSetReportInterval(interval);
	}

	public static void SetLocation(string lati, string longi)
	{
		DCGReporterSetLocation(lati, longi);
	}

	public static void SetDebug(bool isDebug)
	{
		DCGReporterSetDebug(isDebug);
	}

	public static void SendDailyActive()
	{
		DCGReporterSendDailyActive();
	}

	public static void trackEvent(string category, string action, string label, string value, string eid, string extra)
	{
		Debug.Log("调用iOS记录事件");
		DCGReporterTrackEvent(category, action, label, value, extra, extra);
	}

	public static void SetProperty(string name, string value)
	{
		DCGReporterSetProperty(name, value);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
