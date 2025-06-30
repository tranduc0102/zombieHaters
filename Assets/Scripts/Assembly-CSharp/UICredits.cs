using UnityEngine;
using UnityEngine.UI;

public class UICredits : MonoBehaviour
{
	[SerializeField]
	private Text text;

	private void OnEnable()
	{
		UpdateLocalization();
	}

	private void UpdateLocalization()
	{
		text.font = LanguageManager.instance.currentLanguage.font;
		text.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Publisher) + ": DotJoy/Teebik\n" + LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Author) + ": Sergii Orlov\n" + LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.All_Rights_Reserved) + ": Wooden Sword Limited";
	}
}
