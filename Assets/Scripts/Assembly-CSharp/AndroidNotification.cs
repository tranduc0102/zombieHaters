using System;
using UnityEngine;

internal class AndroidNotification
{
	public enum NotificationExecuteMode
	{
		Inexact = 0,
		Exact = 1,
		ExactAndAllowWhileIdle = 2
	}

	private const string fullClassName = "com.ahg.uanotify.UnityNotification";

	private const string mainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";

	public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker)
	{
		SendNotification(id, (int)delay.TotalSeconds, title, message, ticker, Color.white, true, NotificationExecuteMode.Inexact, true, 2, true, 1000L, 1000L);
	}

	public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
	{
		SendNotification(id, (int)delay.TotalSeconds, title, message, ticker, bgColor, true, executeMode, true, 2, true, 1000L, 1000L);
	}

	public static void SendNotification(int id, TimeSpan delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact, bool sound = true, int soundIndex = 2)
	{
		SendNotification(id, (int)delay.TotalSeconds, title, message, ticker, bgColor, lights, executeMode, sound, soundIndex, true, 1000L, 1000L);
	}

	public static void SendNotification(int id, long delay, string title, string message, string ticker, Color32 bgColor, bool lights = true, NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact, bool sound = true, int soundIndex = 2, bool vibrate = true, long vibrateDelay = 1000L, long vibrateTime = 1000L)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ahg.uanotify.UnityNotification");
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("SetNotification", id, delay * 1000, title, message, ticker, sound ? 1 : 0, soundIndex, vibrate ? 1 : 0, vibrateDelay, vibrateTime, lights ? 1 : 0, bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, "com.unity3d.player.UnityPlayerNativeActivity");
		}
	}

	public static void CancelNotification(int id)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ahg.uanotify.UnityNotification");
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelNotification", id);
		}
	}

	public static void CancelAllNotifications()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.ahg.uanotify.UnityNotification");
		if (androidJavaClass != null)
		{
			androidJavaClass.CallStatic("CancelAll");
		}
	}
}
