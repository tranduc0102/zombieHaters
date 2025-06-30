using UnityEngine;
using UnityEngine.UI;

public class LanguageTextLocalizer : MonoBehaviour
{
	public Text text;

	public LanguageKeysEnum key;

	public string addedString = string.Empty;

	public void Localize()
	{
		if (text != null)
		{
			text.text = LanguageManager.instance.GetLocalizedText(key);
			text.text += addedString;
			text.font = LanguageManager.instance.currentLanguage.font;
		}
		else
		{
			Debug.LogError("Localization text null");
		}
	}
}
