using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILanguageCell : MonoBehaviour
{
	public GameObject selectedImage;

	public SystemLanguage selectedLanguage;

	public Text languageText;

	public void Start()
	{
		SetLanguageText();
	}

	public void Select()
	{
		LanguageManager.instance.SetCurrentLanguage(selectedLanguage);
		DataLoader.gui.UpdateMenuContent();
		DataLoader.Instance.UpdateClosedWallsText();
		Object.FindObjectOfType<UILanguagePanel>().Reset();
		selectedImage.SetActive(true);
	}

	public void SetLanguageText()
	{
		Language language = LanguageManager.instance.languages.First((Language l) => l.language == selectedLanguage);
		languageText.text = language.languageName;
		languageText.font = language.font;
	}
}
