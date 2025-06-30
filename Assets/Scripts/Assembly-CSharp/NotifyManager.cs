using UnityEngine;

public class NotifyManager
{
	public static string notifyClass = "mobi.andrutil.autolog.AutologManager";

	private static AndroidJavaObject javaNotifyManager = new AndroidJavaObject(notifyClass);

	private static AndroidJavaObject context;

	public static void addBadgeNumber(int num)
	{
		if (context == null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		javaNotifyManager.CallStatic("showBadgeNumber", context, num);
	}

	public static void reduceBadgeNumber(int num)
	{
		if (context == null)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}
		javaNotifyManager.CallStatic("hideBadgeNumber", context, num);
	}

	public static void removeAllBadgeNumber()
	{
		if (context == null)
		{
/*			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
*//*			context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
*/		}
		javaNotifyManager.CallStatic("hideAllBadgeNumber", context);
	}
}
