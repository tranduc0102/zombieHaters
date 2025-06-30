using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
	public Text languageText;

	public RectTransform rect;

	public GameObject leaderboardObj;

	public GameObject restorePurchasesObj;

	public GameObject SignInObj;

	public GameObject SignOutObj;

	private void OnEnable()
	{
		UpdateLanguageText();
		RefreshGooglePlayUI();
		StartCoroutine(UIController.instance.Scale(base.transform));
	}

	public void UpdateLanguageText()
	{
		SetLanguageText();
	}

	public void ShowLeaderboard()
	{
		LeaderboardManager.instance.ShowLeaderboard();
	}

	public void SignIn()
	{
		LeaderboardManager.instance.AuthenticateUser();
	}

	public void SignOut()
	{
		LeaderboardManager.instance.SignOut();
		RefreshGooglePlayUI();
	}

	public void RefreshGooglePlayUI()
	{
		rect.sizeDelta = new Vector2(rect.sizeDelta.x, 1340f);
		restorePurchasesObj.SetActive(false);
		SignOutObj.SetActive(LeaderboardManager.loginSuccessful);
		SignInObj.SetActive(!LeaderboardManager.loginSuccessful);
	}

	public void SetLanguageText()
	{
		Language currentLanguage = LanguageManager.instance.currentLanguage;
		languageText.text = currentLanguage.languageName;
		languageText.font = currentLanguage.font;
	}
}
