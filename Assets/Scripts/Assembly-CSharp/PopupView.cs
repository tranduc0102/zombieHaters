using UnityEngine;

public class PopupView : MonoBehaviour
{
	[Tooltip("market://details?id=BUNDLE-ID")]
	public string gameLink = "market://details?id=com.woodensword.zombie";

	private void OnEnable()
	{
		AndroidRateUsPopUp.onRateUSPopupComplete += OnRateUSPopupComplete;
		AndroidDialog.onDialogPopupComplete += OnDialogPopupComplete;
		AndroidMessage.onMessagePopupComplete += OnMessagePopupComplete;
	}

	private void OnDisable()
	{
		AndroidRateUsPopUp.onRateUSPopupComplete -= OnRateUSPopupComplete;
		AndroidDialog.onDialogPopupComplete -= OnDialogPopupComplete;
		AndroidMessage.onMessagePopupComplete -= OnMessagePopupComplete;
	}

	private void OnRateUSPopupComplete(MessageState state)
	{
		switch (state)
		{
		case MessageState.RATED:
			Debug.Log("Rate Button pressed");
			break;
		case MessageState.REMIND:
			Debug.Log("Remind Button pressed");
			break;
		case MessageState.DECLINED:
			Debug.Log("Declined Button pressed");
			break;
		}
	}

	private void OnDialogPopupComplete(MessageState state)
	{
		switch (state)
		{
		case MessageState.YES:
			Debug.Log("Yes button pressed");
			break;
		case MessageState.NO:
			Debug.Log("No button pressed");
			break;
		}
	}

	private void OnMessagePopupComplete(MessageState state)
	{
		Debug.Log("Ok button Clicked");
	}

	public void OnDialogPopUp()
	{
		NativeReviewRequest.RequestReview();
	}

	public void OnRatePopUp()
	{
		NativeRateUS nativeRateUS = new NativeRateUS("Like this game?", "Please rate to support future updates!");
		nativeRateUS.SetAppLink(gameLink);
		nativeRateUS.InitRateUS();
	}

	public void OnMessagePopUp()
	{
		NativeMessage nativeMessage = new NativeMessage("TheAppGuruz", "Welcome To TheAppGuruz");
	}
}
