using System;
using WoodenSword.ZombieHaters.UI;

public class UIWorldsScrollCellContent : UIBaseBottomScrollCellContent
{
	public override void SetNewContent(UIBottomScrollCell cell)
	{
		base.SetNewContent(cell);
		UpdateCellName(LanguageManager.instance.GetLocalizedText(GameManager.instance.worldNames[cell.cellIndex]));
	}

	public override void ButtonBuyLogic()
	{
		GameManager.instance.GoToWorld(cell.cellIndex);
	}

	public override void ButtonInfoLogic()
	{
		base.ButtonInfoLogic();
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetPopupHeight(1100f);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetContent(false, LanguageManager.instance.GetLocalizedText(GameManager.instance.worldNames[cell.cellIndex]), "World Short Description", "Passive Income", string.Empty);
	}

	public override void UpdateExtendedInfoVariables()
	{
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetVariables(cell.lbl_shortInfo.text);
	}

	public override UIBottomScrollCell.TopTextType GetTopTextType()
	{
		return UIBottomScrollCell.TopTextType.FullSize;
	}

	public override void UpdateButtonBuyText()
	{
		cell.lbl_buy.text = "Go";
	}

	public override void SetIcon()
	{
		throw new NotImplementedException();
	}

	public override void SetShortInfoIcon()
	{
		cell.img_shortInfo.gameObject.SetActive(true);
		cell.img_shortInfo.sprite = UIController.instance.multiplyImages.bottomPanelShortInfoIcons[1];
	}

	public override void UpdateShortInfoText()
	{
		cell.lbl_shortInfo.text = "???/c".AddHexColor("C8BF11");
	}

	public override void UpdateTopContent()
	{
	}
}
