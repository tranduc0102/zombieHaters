using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WoodenSword.ZombieHaters.UI;

public class UIBottomScrollCell : UIScrollCell
{
	public enum TopTextType
	{
		FullSize = 0,
		Level = 1,
		LevelProgress = 2
	}

	[Serializable]
	public class TopTextHelper
	{
		public Text textComponent;

		public TopTextType textType;
	}

	[Header("General")]
	[SerializeField]
	private UIBaseBottomScrollCellContent currentContent;

	[SerializeField]
	private Image img_icon;

	[SerializeField]
	private Button btn_buy;

	[SerializeField]
	private Button btn_info;

	[SerializeField]
	private List<TopTextHelper> lbl_name;

	public Text lbl_level;

	public Image img_progressBar;

	public Text lbl_shortInfo;

	public Image img_shortInfo;

	public Text lbl_buy;

	public Button btn_unlock;

	public Text lbl_unlock;

	[Header("Hero")]
	public Image img_perk;

	[Header("Booster")]
	public Text lbl_amount;

	public void SetNewContent<T>() where T : UIBaseBottomScrollCellContent
	{
		currentContent = Activator.CreateInstance<T>();
		currentContent.SetNewContent(this);
		btn_buy.SetNewOnClickLogic(currentContent.ButtonBuyLogic);
		btn_info.SetNewOnClickLogic(currentContent.ButtonInfoLogic);
		btn_unlock.SetNewOnClickLogic(currentContent.ButtonInfoLogic);
		img_perk.gameObject.SetActive(typeof(T) == typeof(UIHeroesScrollCellContent));
		lbl_amount.transform.parent.gameObject.SetActive(typeof(T) == typeof(UIBoostersScrollCellContent));
		lbl_shortInfo.transform.parent.gameObject.SetActive(true);
		btn_unlock.gameObject.SetActive(false);
		UpdateContent();
	}

	public void UpdateContent()
	{
		currentContent.UpdateContent();
	}

	public void SetTopName(TopTextType textType, string text)
	{
		for (int i = 0; i < lbl_name.Count; i++)
		{
			if (lbl_name[i].textType == textType)
			{
				lbl_name[i].textComponent.gameObject.SetActive(true);
				lbl_name[i].textComponent.text = text;
				lbl_name[i].textComponent.font = LanguageManager.instance.currentLanguage.font;
			}
			else
			{
				lbl_name[i].textComponent.gameObject.SetActive(false);
			}
		}
		lbl_level.transform.parent.gameObject.SetActive(textType != TopTextType.FullSize);
		img_progressBar.transform.parent.gameObject.SetActive(textType == TopTextType.LevelProgress);
	}
}
