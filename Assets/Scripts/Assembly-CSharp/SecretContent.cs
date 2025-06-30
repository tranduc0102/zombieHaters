using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecretContent : MonoBehaviour
{
	public RectTransform rect;

	[Header("Active")]
	[SerializeField]
	private GameObject active;

	[SerializeField]
	private Text activeCoinsText;

	[SerializeField]
	private Text activeLevelText;

	[SerializeField]
	private Image activeHatImage;

	[SerializeField]
	private Text activeDescriptionText;

	[Header("Inactive")]
	[SerializeField]
	private GameObject inactive;

	[SerializeField]
	private Text inactiveCoinsText;

	[Header("Next")]
	[SerializeField]
	private GameObject next;

	[SerializeField]
	private Text nextCoinsText;

	[SerializeField]
	private Text nextLevelText;

	[SerializeField]
	private Image nextHatImage;

	public void SetContent(int level)
	{
		activeCoinsText.text = DataLoader.Instance.moneyBoxGold[level].ToString();
		inactiveCoinsText.text = activeCoinsText.text;
		nextCoinsText.text = activeCoinsText.text;
		activeLevelText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Level) + " " + (level + 1);
		activeLevelText.font = LanguageManager.instance.currentLanguage.font;
		nextLevelText.text = activeLevelText.text;
		activeHatImage.sprite = UIController.instance.multiplyImages.secretHat[level % 5];
		nextHatImage.sprite = activeHatImage.sprite;
		activeDescriptionText.text = MoneyBoxManager.instance.currentHelpText;
		activeDescriptionText.font = LanguageManager.instance.currentLanguage.font;
	}

	public void SetActive(DailyContentType type)
	{
		active.SetActive(type == DailyContentType.Active);
		inactive.SetActive(type == DailyContentType.Inactive);
		next.SetActive(type == DailyContentType.Next);
	}

	public void SetTimeText()
	{
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(TimeText());
		}
		else
		{
			nextLevelText.text = activeLevelText.text;
		}
	}

	private IEnumerator TimeText()
	{
		DateTime d = TimeManager.CurrentDateTime;
		TimeSpan t = d.AddDays(1.0).Date.Subtract(d);
		while (true)
		{
			nextLevelText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
			yield return new WaitForSeconds(1f);
			t = t.Add(TimeSpan.FromSeconds(-1.0));
		}
	}
}
