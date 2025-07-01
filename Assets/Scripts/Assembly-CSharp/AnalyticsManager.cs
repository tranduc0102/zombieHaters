using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
	public static string uniqueUserId;

	public static string START = "SessionStart";

	[Header("Flurry Settings")]
	[SerializeField]
	private string _iosApiKey = string.Empty;

	[SerializeField]
	private string _androidApiKey = string.Empty;

	[Header("Reporter")]
	[SerializeField]
	[Space]
	private string _appsFlyerDevKey = string.Empty;

	[SerializeField]
    private string trafficIdIOS = "864";

	[SerializeField]
	private string trafficIdAndroid = "863";

	[Space]
	[SerializeField]
	private string buglyIdIOS = "5d116d811e";

	[SerializeField]
	private string buglyIdAndroid = "37e560471e";

	[Space]
	[SerializeField]
	private string iTunesKey = "1424127467";

	public static AnalyticsManager instance { get; private set; }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		/*if (!FB.IsInitialized)
		{
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			FB.ActivateApp();
		}*/
/*		FlurryAndroid.SetLogEnabled(true);
*/		/*if (!Application.RequestAdvertisingIdentifierAsync(OnAdvertisingIdentifierRecieved))
		{
			Init();
		}*/
	}

	public void OnAdvertisingIdentifierRecieved(string advertisingId, bool trackingEnabeld, string error)
	{
		uniqueUserId = advertisingId;
		Init();
	}

	public void ReporterInit()
	{
		Debug.Log("Call reporter initialize");
		Reporter.Init("http://stt.dotjoy.io", _appsFlyerDevKey, trafficIdAndroid, uniqueUserId, buglyIdAndroid, "2849642608314753", "H4gWBALF37Jkdw9d0P", "rTpBHKBBAhhuegjq2T", string.Empty);
		Reporter.SendRealActiveEvent();
	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			FB.LogAppEvent(START);
		}
		else
		{
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void Init()
	{
	//	FirebaseAnalytics.SetUserId(uniqueUserId);
	//	FirebaseAnalytics.LogEvent(START);
	//	FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
	//	if ((bool)MonoSingleton<Flurry>.Instance)
	//	{
	//		MonoSingleton<Flurry>.Instance.SetLogLevel(LogLevel.CriticalOnly);
	//		MonoSingleton<Flurry>.Instance.StartSession(_iosApiKey, _androidApiKey);
	//		MonoSingleton<Flurry>.Instance.LogUserID(uniqueUserId);
	//		MonoSingleton<Flurry>.Instance.LogEvent(START);
	//	}
	//	ReporterInit();
	}

	public void LogEvent(string eventName)
	{
	//	FirebaseAnalytics.LogEvent(eventName.Replace(" ", string.Empty));
		/*if ((bool)MonoSingleton<Flurry>.Instance)
		{
			MonoSingleton<Flurry>.Instance.LogEvent(eventName);
		}
		FB.LogAppEvent(eventName);*/
		Reporter.SendEvent("ZombieHaters", eventName, string.Empty, string.Empty, string.Empty, null);
	}

	public void LogEvent(string eventName, Dictionary<string, string> eventParameters)
	{
		Debug.Log("LogEvent: " + eventName);
		string text = string.Empty;
		foreach (KeyValuePair<string, string> eventParameter in eventParameters)
		{
			string text2 = text;
			text = text2 + eventParameter.Key + ":" + eventParameter.Value + ",\n";
		}
		Debug.Log(text);
	//	Parameter[] array = new Parameter[eventParameters.Count];
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		int num = 0;
		foreach (KeyValuePair<string, string> eventParameter2 in eventParameters)
		{
		//	array[num] = new Parameter(eventParameter2.Key.Replace(" ", string.Empty), eventParameter2.Value.Replace(" ", string.Empty));
			num++;
			dictionary.Add(eventParameter2.Key, eventParameter2.Value);
		}
	//	FirebaseAnalytics.LogEvent(eventName.Replace(" ", string.Empty), array);
		//if ((bool)MonoSingleton<Flurry>.Instance)
		//{
		//	MonoSingleton<Flurry>.Instance.LogEvent(eventName, eventParameters);
		//}
/*		FB.LogAppEvent(eventName, null, dictionary);
*/		Reporter.SendEvent("ZombieHaters", eventName, string.Empty, string.Empty, text, null);
	}

	public void LogPurchaseEvent(string eventName, Dictionary<string, string> eventParameters, float logPurchase, string currency)
	{
		Debug.Log("LogPurchaseEvent: " + eventName);
		string text = string.Empty;
		foreach (KeyValuePair<string, string> eventParameter in eventParameters)
		{
			string text2 = text;
			text = text2 + eventParameter.Key + ": " + eventParameter.Value + ",\n";
		}
	//	Parameter[] array = new Parameter[eventParameters.Count];
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		int num = 0;
		foreach (KeyValuePair<string, string> eventParameter2 in eventParameters)
		{
		//	array[num] = new Parameter(eventParameter2.Key.Replace(" ", string.Empty), eventParameter2.Value.Replace(" ", string.Empty));
			num++;
			dictionary.Add(eventParameter2.Key, eventParameter2.Value);
		}
		//FirebaseAnalytics.LogEvent(eventName.Replace(" ", string.Empty), array);
		/*if ((bool)MonoSingleton<Flurry>.Instance)
		{
			MonoSingleton<Flurry>.Instance.LogEvent(eventName, eventParameters);
		}*/
/*		FB.LogPurchase(logPurchase, currency, dictionary);
*/		eventParameters.Add("af_revenue", logPurchase.ToString());
		eventParameters.Add("af_currency", currency);
		Reporter.SendEvent("ZombieHaters", eventName, string.Empty, string.Empty, text, null);
	}
}
