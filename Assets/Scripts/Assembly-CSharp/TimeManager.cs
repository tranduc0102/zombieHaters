using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	private static DateTime currentTime;

	public static TimeManager instance { get; private set; }

	public static bool gotDateTime { get; private set; }

	public static DateTime CurrentDateTime
	{
		get
		{
			return currentTime;
		}
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		Application.runInBackground = false;
		StartCoroutine(TimeCor());
	}

	private IEnumerator TimeCor()
	{
		UpdateTime();
		while (true)
		{
			yield return new WaitForSecondsRealtime(1f);
			currentTime = currentTime.AddSeconds(1.0);
		}
	}

	public void UpdateTime()
	{
		gotDateTime = GetNistTime(out currentTime);
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			UpdateTime();
		}
	}

	public void SetFirstEnterDate(SaveData saveData)
	{
		StartCoroutine(SetFirstDate(saveData));
	}

	private IEnumerator SetFirstDate(SaveData saveData)
	{
		while (!gotDateTime)
		{
			yield return null;
		}
		if (saveData.firstEnterDate == DateTime.MinValue)
		{
			saveData.firstEnterDate = CurrentDateTime;
			IOSCloudSave.instance.SaveFirstDate();
			Debug.Log("First enter date: " + saveData.firstEnterDate);
		}
	}

	public static bool GetUTCInternetTime(out DateTime dateTime)
	{
		dateTime = DateTime.MinValue;
		string empty = string.Empty;
		string[] array = new string[18]
		{
			"time-a-g.nist.gov", "time-b-g.nist.gov", "time-c-g.nist.gov", "time-d-g.nist.gov", "time-a-wwv.nist.gov", "time-b-wwv.nist.gov", "time-c-wwv.nist.gov", "time-d-wwv.nist.gov", "time-a-b.nist.gov", "time-b-b.nist.gov",
			"utcnist.colorado.edu", "utcnist2.colorado.edu", "nist1-ny.ustiming.org", "time-a.nist.gov", "nist1-chi.ustiming.org", "time.nist.gov", "ntp-nist.ldsbc.edu", "nist1-la.ustiming.org"
		};
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				StreamReader streamReader = new StreamReader(new TcpClient(array[i], 13).GetStream());
				empty = streamReader.ReadToEnd();
				streamReader.Close();
				if (empty.Length > 47 && empty.Substring(38, 9).Equals("UTC(NIST)"))
				{
					int num = int.Parse(empty.Substring(1, 5));
					int num2 = int.Parse(empty.Substring(7, 2));
					int month = int.Parse(empty.Substring(10, 2));
					int day = int.Parse(empty.Substring(13, 2));
					int hour = int.Parse(empty.Substring(16, 2));
					int minute = int.Parse(empty.Substring(19, 2));
					int second = int.Parse(empty.Substring(22, 2));
					num2 = ((num <= 51544) ? (num2 + 1999) : (num2 + 2000));
					dateTime = new DateTime(num2, month, day, hour, minute, second).ToLocalTime();
					Debug.Log("Got time from: " + array[i]);
					return true;
				}
			}
			catch (Exception)
			{
			}
		}
		return dateTime != DateTime.MinValue;
	}

	public static bool GetNistTime(out DateTime dateTime)
	{
		if (StaticConstants.NeedInternetConnection)
		{
			dateTime = DateTime.MinValue;
			string empty = string.Empty;
			string[] array = new string[18]
			{
				"time-a-g.nist.gov", "time-b-g.nist.gov", "time-c-g.nist.gov", "time-d-g.nist.gov", "time-a-wwv.nist.gov", "time-b-wwv.nist.gov", "time-c-wwv.nist.gov", "time-d-wwv.nist.gov", "time-a-b.nist.gov", "time-b-b.nist.gov",
				"utcnist.colorado.edu", "utcnist2.colorado.edu", "nist1-ny.ustiming.org", "time-a.nist.gov", "nist1-chi.ustiming.org", "time.nist.gov", "ntp-nist.ldsbc.edu", "nist1-la.ustiming.org"
			};
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					StreamReader streamReader = new StreamReader(new TcpClient(array[i], 13).GetStream());
					empty = streamReader.ReadToEnd();
					streamReader.Close();
					if (empty.Length > 47 && empty.Substring(38, 9).Equals("UTC(NIST)"))
					{
						int num = int.Parse(empty.Substring(1, 5));
						int num2 = int.Parse(empty.Substring(7, 2));
						int month = int.Parse(empty.Substring(10, 2));
						int day = int.Parse(empty.Substring(13, 2));
						int hour = int.Parse(empty.Substring(16, 2));
						int minute = int.Parse(empty.Substring(19, 2));
						int second = int.Parse(empty.Substring(22, 2));
						num2 = ((num <= 51544) ? (num2 + 1999) : (num2 + 2000));
						dateTime = new DateTime(num2, month, day, hour, minute, second).ToLocalTime();
						Debug.Log("Got time from: " + array[i]);
						break;
					}
				}
				catch (Exception)
				{
				}
			}
			return dateTime != DateTime.MinValue;
		}
		dateTime = DateTime.Now;
		return true;
	}
}
