using UnityEngine;

public class UILanguagePanel : MonoBehaviour
{
	public UIOptions options;

	public UILanguageCell[] languageCells;

	public void Start()
	{
		Reset();
		SelectLanguage((SystemLanguage)PlayerPrefs.GetInt(StaticConstants.lastSelectedLanguage));
	}

	public void Reset()
	{
		options.UpdateLanguageText();
		for (int i = 0; i < languageCells.Length; i++)
		{
			languageCells[i].selectedImage.SetActive(false);
		}
	}

	public void SelectLanguage(SystemLanguage language)
	{
		for (int i = 0; i < languageCells.Length; i++)
		{
			if (language == languageCells[i].selectedLanguage)
			{
				languageCells[i].selectedImage.SetActive(true);
				break;
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
	}
}
