using UnityEngine;

public class AndroidMessage : MonoBehaviour
{
	public delegate void OnMessagePopupComplete(MessageState state);

	public string title;

	public string message;

	public string ok;

	public static event OnMessagePopupComplete onMessagePopupComplete;

	private void RaiseOnMessagePopupComplete(MessageState state)
	{
		if (AndroidMessage.onMessagePopupComplete != null)
		{
			AndroidMessage.onMessagePopupComplete(state);
		}
	}

	public static AndroidMessage Create(string title, string message)
	{
		return Create(title, message, "Ok");
	}

	public static AndroidMessage Create(string title, string message, string ok)
	{
		AndroidMessage androidMessage = new GameObject("AndroidMessagePopup").AddComponent<AndroidMessage>();
		androidMessage.title = title;
		androidMessage.message = message;
		androidMessage.ok = ok;
		androidMessage.init();
		return androidMessage;
	}

	public void init()
	{
		AndroidNative.showMessage(title, message, ok);
	}

	public void OnMessagePopUpCallBack(string buttonIndex)
	{
		RaiseOnMessagePopupComplete(MessageState.OK);
		Object.Destroy(base.gameObject);
	}
}
