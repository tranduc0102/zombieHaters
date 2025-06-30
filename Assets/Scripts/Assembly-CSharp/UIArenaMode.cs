using UnityEngine;
using UnityEngine.UI;

public class UIArenaMode : MonoBehaviour
{
	[SerializeField]
	private int requiredLevel;

	[SerializeField]
	private Text requiredLevelText;

	[SerializeField]
	private GameObject levelRequired;

	[SerializeField]
	private Button buttonPlay;

	[SerializeField]
	private Text description;

	public void CheckLevel()
	{
		requiredLevelText.font = LanguageManager.instance.currentLanguage.font;
		requiredLevelText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Reach_x_Level_to_Unlock).Replace("x", requiredLevel.ToString());
		CheckBestFit();
		if (DataLoader.Instance.GetCurrentPlayerLevel() >= requiredLevel)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	private void Open()
	{
		buttonPlay.gameObject.SetActive(true);
		levelRequired.SetActive(false);
	}

	private void Close()
	{
		buttonPlay.gameObject.SetActive(false);
		levelRequired.SetActive(true);
	}

	private void CheckBestFit()
	{
		description.resizeTextForBestFit = LanguageManager.instance.currentLanguage.language != SystemLanguage.ChineseSimplified && LanguageManager.instance.currentLanguage.language != SystemLanguage.ChineseTraditional && LanguageManager.instance.currentLanguage.language != SystemLanguage.Japanese && LanguageManager.instance.currentLanguage.language != SystemLanguage.Korean;
	}
}
