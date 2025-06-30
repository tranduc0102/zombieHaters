using System;
using System.Collections.Generic;
using WoodenSword.ZombieHaters.UI;

public class UIPassiveAbilitiesScrollCellContent : UIBaseBottomScrollCellContent
{
	private AbilityData ability;

	public override void SetNewContent(UIBottomScrollCell cell)
	{
		ability = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilitiesManager.sequence[cell.cellIndex]);
		base.SetNewContent(cell);
		UpdateCellName(ability.GetLocalizedName());
	}

	public override void ButtonBuyLogic()
	{
		if (DataLoader.Instance.passiveAbilitiesManager.UpgradeAbility(ability.abilityType))
		{
			UpdateContent();
			AnalyticsManager.instance.LogEvent("UpgradeAbility", new Dictionary<string, string>
			{
				{
					"Ability",
					ability.abilityType.ToString()
				},
				{
					"level",
					ability.savedAbility.abilityLevel.ToString()
				}
			});
		}
	}

	public override void ButtonInfoLogic()
	{
		base.ButtonInfoLogic();
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.levelScrollPanel.SetContent(1, DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel + 1, ability);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetPopupHeight(1100f);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetContent(true, ability.GetLocalizedName(), "Ability Short Description", "Chance", string.Empty);
	}

	public override void UpdateExtendedInfoVariables()
	{
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetVariables(cell.lbl_shortInfo.text, ability.savedAbility.abilityLevel + 1);
	}

	public override UIBottomScrollCell.TopTextType GetTopTextType()
	{
		return UIBottomScrollCell.TopTextType.Level;
	}

	public override void SetIcon()
	{
		throw new NotImplementedException();
	}

	public override void UpdateShortInfoText()
	{
		float bonusBylevel = ability.GetBonusBylevel(ability.savedAbility.abilityLevel);
		if (ability.IsMaxLevel())
		{
			cell.lbl_shortInfo.text = bonusBylevel * 100f + "%";
		}
		else
		{
			float bonusBylevel2 = ability.GetBonusBylevel(ability.savedAbility.abilityLevel + 1);
			cell.lbl_shortInfo.text = bonusBylevel * 100f + "% + " + (bonusBylevel2 - bonusBylevel) * 100f + "%";
		}
		cell.lbl_shortInfo.text = cell.lbl_shortInfo.text.AddHexColor("FFFFFF");
	}

	public override void UpdateTopContent()
	{
		cell.lbl_level.text = (ability.savedAbility.abilityLevel + 1).ToString();
	}

	public override void UpdateButtonBuyText()
	{
		if (ability.IsMaxLevel())
		{
			cell.lbl_buy.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Max_Maximum);
		}
		else
		{
			cell.lbl_buy.text = ability.GetCurrentPrice().ToString();
		}
	}
}
