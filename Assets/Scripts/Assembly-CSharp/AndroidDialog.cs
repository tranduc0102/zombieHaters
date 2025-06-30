using System;
using UnityEngine;

public class AndroidDialog : MonoBehaviour
{
	public delegate void OnDialogPopupComplete(MessageState state);

	public string title;

	public string message;

	public string yes;

	public string no;

	public string urlString;

	public static event OnDialogPopupComplete onDialogPopupComplete;

	private void RaiseOnOnDialogPopupComplete(MessageState state)
	{
		if (AndroidDialog.onDialogPopupComplete != null)
		{
			AndroidDialog.onDialogPopupComplete(state);
		}
	}

	public static AndroidDialog Create(string title, string message)
	{
		return Create(title, message, "Yes", "No");
	}

	public static AndroidDialog Create(string title, string message, string yes, string no)
	{
		AndroidDialog androidDialog = new GameObject("AndroidDialogPopup").AddComponent<AndroidDialog>();
		androidDialog.title = title;
		androidDialog.message = message;
		androidDialog.yes = yes;
		androidDialog.no = no;
		androidDialog.init();
		return androidDialog;
	}

	public void init()
	{
		AndroidNative.showDialog(title, message, yes, no);
	}

	public void OnDialogPopUpCallBack(string buttonIndex)
	{
		switch (Convert.ToInt16(buttonIndex))
		{
		case 0:
			AndroidNative.RedirectToWebPage(urlString);
			RaiseOnOnDialogPopupComplete(MessageState.YES);
			break;
		case 1:
			RaiseOnOnDialogPopupComplete(MessageState.NO);
			break;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
