using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
	public List<Language> languages;

	public readonly SystemLanguage defaultLanguage = SystemLanguage.English;

	private List<string> defaultLanguageString;

	private List<string> currentLanguageString;

	public static LanguageManager instance { get; private set; }

	public Language currentLanguage { get; private set; }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		if (PlayerPrefs.HasKey(StaticConstants.lastSelectedLanguage))
		{
			SetCurrentLanguage((SystemLanguage)PlayerPrefs.GetInt(StaticConstants.lastSelectedLanguage));
		}
		else
		{
			SetCurrentLanguage(Application.systemLanguage);
		}
	}

	public void SetCurrentLanguage(SystemLanguage language)
	{
		defaultLanguageString = languages.First((Language l) => l.language == SystemLanguage.English).languageTextAsset.text.Split('\n').ToList();
		if (languages.Any((Language l) => l.language == language))
		{
			currentLanguage = languages.First((Language l) => l.language == language);
		}
		else
		{
			currentLanguage = languages.First((Language l) => l.language == SystemLanguage.English);
		}
		Debug.Log("Language set: " + currentLanguage.language);
		currentLanguageString = currentLanguage.languageTextAsset.text.Split('\n').ToList();
		PlayerPrefs.SetInt(StaticConstants.lastSelectedLanguage, (int)currentLanguage.language);
		PlayerPrefs.Save();
		CheckLocalizedValues();
		UpdateLocalizers();
	}

	public void CheckLocalizedValues()
	{
		if (currentLanguageString.Count < defaultLanguageString.Count)
		{
			for (int i = currentLanguageString.Count; i < defaultLanguageString.Count; i++)
			{
				currentLanguageString.Add(defaultLanguageString[i]);
				Debug.LogWarning("TextNotLocalized: " + defaultLanguageString[i]);
			}
		}
	}

	public string GetLocalizedText(string defaultLanguageText)
	{
		for (int i = 0; i < defaultLanguageString.Count; i++)
		{
			if (string.Equals(defaultLanguageText.Trim(), defaultLanguageString[i].Trim(), StringComparison.OrdinalIgnoreCase))
			{
				if (currentLanguage.language == SystemLanguage.Arabic)
				{
					return Reverse(currentLanguageString[i]);
				}
				return currentLanguageString[i];
			}
		}
		for (int j = 0; j < currentLanguageString.Count; j++)
		{
			if (defaultLanguageText.ToUpper() == currentLanguageString[j].ToUpper())
			{
				if (currentLanguage.language == SystemLanguage.Arabic)
				{
					return Reverse(defaultLanguageText);
				}
				return defaultLanguageText;
			}
		}
		Debug.LogWarning("TextNotLocalized: " + defaultLanguageText);
		return defaultLanguageText;
	}

	public string GetLocalizedText(LanguageKeysEnum key)
	{
		if (currentLanguage.language == SystemLanguage.Arabic)
		{
			return Reverse(currentLanguageString[(int)key]);
		}
		return currentLanguageString[(int)key];
	}

	public static string Reverse(string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse(array);
		return new string(array);
	}

	public void UpdateLocalizers()
	{
		LanguageTextLocalizer[] array = Resources.FindObjectsOfTypeAll<LanguageTextLocalizer>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Localize();
		}
		if (DataLoader.initialized)
		{
			UIController.instance.scrollControllers.achievementsController.UpdateLocalization();
		}
	}

	public bool IsReverseLanguage(SystemLanguage language)
	{
		switch (language)
		{
		case SystemLanguage.ChineseSimplified:
			return true;
		case SystemLanguage.ChineseTraditional:
			return true;
		case SystemLanguage.Korean:
			return true;
		case SystemLanguage.Japanese:
			return true;
		default:
			return false;
		}
	}
}
