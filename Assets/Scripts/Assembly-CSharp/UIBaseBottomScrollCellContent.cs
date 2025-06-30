public abstract class UIBaseBottomScrollCellContent
{
	protected UIBottomScrollCell cell;

	public virtual void SetNewContent(UIBottomScrollCell cell)
	{
		this.cell = cell;
		SetShortInfoIcon();
	}

	public abstract UIBottomScrollCell.TopTextType GetTopTextType();

	public abstract void SetIcon();

	public abstract void ButtonBuyLogic();

	public abstract void UpdateTopContent();

	public abstract void UpdateShortInfoText();

	public abstract void UpdateButtonBuyText();

	public abstract void UpdateExtendedInfoVariables();

	public virtual void ButtonInfoLogic()
	{
		DataLoader.gui.popUpsPanel.gameObject.SetActive(true);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.gameObject.SetActive(true);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetButtonBuyLogic(ButtonBuyLogic, UpdateExtendedInfoVariables);
		UpdateExtendedInfoVariables();
	}

	public virtual void SetShortInfoIcon()
	{
		cell.img_shortInfo.gameObject.SetActive(false);
	}

	public virtual void UpdateCellName(string text)
	{
		cell.SetTopName(GetTopTextType(), text);
	}

	public virtual void UpdateContent()
	{
		UpdateTopContent();
		UpdateButtonBuyText();
		UpdateShortInfoText();
	}
}
