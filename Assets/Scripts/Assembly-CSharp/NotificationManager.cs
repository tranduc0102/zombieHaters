using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
	private readonly Dictionary<TimeSpan, TimeSpan> bestNotificationTime = new Dictionary<TimeSpan, TimeSpan> { 
	{
		TimeSpan.FromHours(20.0),
		TimeSpan.FromHours(20.0)
	} };

	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private List<NotificationInfo> notifications;

	[SerializeField]
	private NotificationInfo testNotification;

	public static NotificationManager instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		AndroidNotification.CancelAllNotifications();
		StartCoroutine(WaitForLocalization());
	}

	public void SendTestNotification()
	{
		PrintText("Initializing test notification");
		AndroidNotification.SendNotification(999, 10L, testNotification.header1, testNotification.description, testNotification.header2, Color.white, true, AndroidNotification.NotificationExecuteMode.Inexact, true, 2, true, 0L, 500L);
	}

	public void SwichDelayType(int notificationID)
	{
		DateTime date = DateTime.Now;
		switch (notifications[notificationID].delayType)
		{
		case NotificationInfo.DelayType.Year:
			date = date.AddYears(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.Month:
			date = date.AddMonths(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.Day:
			date = date.AddDays(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.Hour:
			date = date.AddHours(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.Minute:
			date = date.AddMinutes(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.Second:
			date = date.AddSeconds(notifications[notificationID].delay);
			break;
		case NotificationInfo.DelayType.WorkDay:
			date = ((DateTime.Now.DayOfWeek != DayOfWeek.Friday) ? ((DateTime.Now.DayOfWeek != DayOfWeek.Saturday) ? date.AddDays(1.0) : date.AddDays(2.0)) : date.AddDays(3.0));
			break;
		case NotificationInfo.DelayType.Holiday:
			date = date.AddDays((double)(UnityEngine.Random.Range(6, 8) - date.DayOfWeek));
			break;
		}
		int num = (int)GetRandomNotificationTime(date, notifications[notificationID].timeType).Subtract(DateTime.Now).TotalSeconds;
		AndroidNotification.SendNotification(notificationID + 1, num, LanguageManager.instance.GetLocalizedText(notifications[notificationID].header1), LanguageManager.instance.GetLocalizedText(notifications[notificationID].description), LanguageManager.instance.GetLocalizedText(notifications[notificationID].header2), Color.white, true, AndroidNotification.NotificationExecuteMode.Inexact, true, 2, true, 0L, 500L);
	}

	public IEnumerator WaitForLocalization()
	{
		while (LanguageManager.instance == null)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < notifications.Count; i++)
		{
			SwichDelayType(i);
		}
	}

	private DateTime GetRandomNotificationTime(DateTime date, NotificationInfo.NotificationTimeType type)
	{
		int index = UnityEngine.Random.Range(0, bestNotificationTime.Count);
		int num = (int)(bestNotificationTime.ElementAt(index).Value - bestNotificationTime.ElementAt(index).Key).TotalSeconds;
		TimeSpan value = TimeSpan.FromHours(12.0);
		switch (type)
		{
		case NotificationInfo.NotificationTimeType.Hours:
			value = bestNotificationTime.ElementAt(index).Key.Add(TimeSpan.FromHours(UnityEngine.Random.Range(0, num / 3600 + 1)));
			break;
		case NotificationInfo.NotificationTimeType.Minutes:
			value = bestNotificationTime.ElementAt(index).Key.Add(TimeSpan.FromMinutes(UnityEngine.Random.Range(0, num / 60)));
			break;
		case NotificationInfo.NotificationTimeType.Seconds:
			value = bestNotificationTime.ElementAt(index).Key.Add(TimeSpan.FromSeconds(UnityEngine.Random.Range(0, num)));
			break;
		}
		return date.Date.Add(value);
	}

	private void PrintText(string text)
	{
		if (debugMode)
		{
			Debug.Log(text);
		}
	}

	private void RegisterIosNotification(int seconds, string description)
	{
	}

	public void RegisterNotifications()
	{
		Debug.Log("RegisteredNotifications");
		Start();
	}
}
