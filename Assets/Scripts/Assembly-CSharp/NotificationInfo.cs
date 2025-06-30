using System;
using UnityEngine;

[Serializable]
public class NotificationInfo
{
	public enum DelayType
	{
		Year = 0,
		Month = 1,
		Day = 2,
		Hour = 3,
		Minute = 4,
		Second = 5,
		WorkDay = 6,
		Holiday = 7
	}

	public enum NotificationTimeType
	{
		Hours = 0,
		Minutes = 1,
		Seconds = 2
	}

	public string header1;

	public string description;

	public string header2;

	[Tooltip("Truncates notification time value")]
	public NotificationTimeType timeType;

	public DelayType delayType = DelayType.Day;

	[Tooltip("The value will not be considered if delayType equals Holiday or WorkDay")]
	public int delay;
}
