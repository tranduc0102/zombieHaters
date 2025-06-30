using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WoodenSword.ZombieHaters.UI;

public class UIBottomExtendedInfoPopup : MonoBehaviour
{
	[SerializeField]
	private RectTransform popupRect;

	[SerializeField]
	private Text lbl_popupName;

	[SerializeField]
	private Text lbl_level;

	[SerializeField]
	private Image img_iconLeft;

	[SerializeField]
	private Image img_iconCenter;

	[SerializeField]
	private Text lbl_shortDescription;

	[SerializeField]
	private Text lbl_amountName;

	[SerializeField]
	private Text lbl_amount;

	[SerializeField]
	private Text lbl_description;

	[SerializeField]
	private Button btn_buy;

	[SerializeField]
	private Button btn_buyMoney;

	[SerializeField]
	private Button buy_video;

	public UIExtendedInfoScrollPanel levelScrollPanel;

	public void SetPopupHeight(float height)
	{
		popupRect.sizeDelta = new Vector2(popupRect.sizeDelta.x, height);
	}

	public void SetContent(bool needScrollPanel, string popupName, string shortDescription, string amountName, string description = "")
	{
		lbl_popupName.text = popupName;
		img_iconCenter.gameObject.SetActive(!needScrollPanel);
		img_iconLeft.gameObject.SetActive(needScrollPanel);
		lbl_shortDescription.text = shortDescription;
		lbl_description.text = description;
		lbl_description.transform.parent.gameObject.SetActive(description != string.Empty);
		lbl_amountName.text = amountName;
	}

	public void SetVariables(string amount, int level = -1)
	{
		lbl_amount.text = amount;
		lbl_level.text = level.ToString();
		lbl_level.transform.parent.gameObject.SetActive(level > 0);
	}

	public void SetButtonBuyLogic(UnityAction updateBottomPanel, UnityAction updateExtendedInfo)
	{
		btn_buy.SetNewOnClickLogic(delegate
		{
			updateBottomPanel();
			updateExtendedInfo();
		});
	}
}
