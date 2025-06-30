using UnityEngine;
using UnityEngine.UI;

public class UIHeroInfoPerk : UIScrollCell
{
	[SerializeField]
	private Image imagePerk;

	[SerializeField]
	private Text textLevel;

	[SerializeField]
	private Text textPower;

	[SerializeField]
	private GameObject active;

	[SerializeField]
	private GameObject inactive;

	private Color ActiveTextColor
	{
		get
		{
			return Color.white;
		}
	}

	private Color InactiveTextColor
	{
		get
		{
			return new Color(0f, 28f / 85f, 0.4745098f, 1f);
		}
	}

	public void UpdateContent(bool perk)
	{
		inactive.SetActive(!perk);
		textLevel.color = ((!perk) ? InactiveTextColor : ActiveTextColor);
		textPower.color = ((!perk) ? InactiveTextColor : ActiveTextColor);
		imagePerk.sprite = ((!perk) ? UIController.instance.multiplyImages.perks[base.cellIndex].inactive : UIController.instance.multiplyImages.perks[base.cellIndex].active);
		textLevel.font = LanguageManager.instance.currentLanguage.font;
		textLevel.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Level) + " " + (1 + base.cellIndex) * 25;
	}
}
