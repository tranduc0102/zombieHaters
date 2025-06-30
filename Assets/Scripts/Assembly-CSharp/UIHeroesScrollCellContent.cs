using System;
using System.Collections.Generic;
using System.Linq;
using WoodenSword.ZombieHaters.UI;

public class UIHeroesScrollCellContent : UIBaseBottomScrollCellContent
{
	private SaveData.HeroData heroData;

	private List<Survivors.SurvivorLevels> levelsInfo;

	public override void SetNewContent(UIBottomScrollCell cell)
	{
		heroData = DataLoader.playerData.heroData.First((SaveData.HeroData item) => item.heroType == DataLoader.Instance.survivors[cell.cellIndex].heroType);
		levelsInfo = DataLoader.Instance.survivors[cell.cellIndex].levels;
		base.SetNewContent(cell);
		UpdateCellName(LanguageManager.instance.GetLocalizedText(DataLoader.Instance.survivors[cell.cellIndex].name));
		cell.lbl_unlock.text = "Unlock";
	}

	public override UIBottomScrollCell.TopTextType GetTopTextType()
	{
		if (heroData.isOpened)
		{
			return UIBottomScrollCell.TopTextType.LevelProgress;
		}
		return UIBottomScrollCell.TopTextType.FullSize;
	}

	public override void SetShortInfoIcon()
	{
		cell.img_shortInfo.gameObject.SetActive(true);
		cell.img_shortInfo.sprite = UIController.instance.multiplyImages.bottomPanelShortInfoIcons[0];
	}

	public override void SetIcon()
	{
		throw new NotImplementedException();
	}

	public override void ButtonInfoLogic()
	{
		base.ButtonInfoLogic();
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.levelScrollPanel.SetContent(25, DataLoader.playerData.survivorMaxLevel / 25, LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Upgrade) + " X2");
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetPopupHeight(1500f);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetContent(true, LanguageManager.instance.GetLocalizedText(DataLoader.Instance.survivors[cell.cellIndex].fullname), LanguageManager.instance.GetLocalizedText(DataLoader.Instance.survivors[cell.cellIndex].shortDescriptionKey), LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Power_Hero), LanguageManager.instance.GetLocalizedText(DataLoader.Instance.survivors[cell.cellIndex].heroStory));
	}

	public override void UpdateExtendedInfoVariables()
	{
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetVariables(cell.lbl_shortInfo.text, heroData.currentLevel);
	}

	public override void UpdateButtonBuyText()
	{
		if (!heroData.isOpened)
		{
			cell.lbl_buy.text = "Closed";
		}
		else if (heroData.currentLevel == DataLoader.playerData.survivorMaxLevel)
		{
			cell.lbl_buy.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Max_Maximum);
			cell.lbl_buy.font = LanguageManager.instance.currentLanguage.font;
		}
		else
		{
			cell.lbl_buy.text = AbbreviationUtility.AbbreviateNumber(levelsInfo[heroData.currentLevel].cost);
		}
	}

	public override void UpdateContent()
	{
		base.UpdateContent();
		cell.btn_unlock.gameObject.SetActive(!heroData.isOpened);
	}

	public override void ButtonBuyLogic()
	{
		if (DataLoader.Instance.RefreshMoney(-levelsInfo[heroData.currentLevel].cost))
		{
			heroData.currentLevel++;
			UIController.instance.scrollControllers.bottomScrollController.UpdateAllContent();
			AnalyticsManager.instance.LogEvent("RewardedHeroUpgrade", new Dictionary<string, string>
			{
				{
					"HeroType",
					DataLoader.playerData.heroData[cell.cellIndex].heroType.ToString()
				},
				{
					"Level",
					DataLoader.playerData.heroData[cell.cellIndex].currentLevel.ToString()
				}
			});
		}
	}

	public override void UpdateTopContent()
	{
		cell.img_perk.gameObject.SetActive(heroData.currentLevel > 24);
		cell.img_progressBar.fillAmount = (float)(heroData.currentLevel % 25) / 25f;
		for (int i = 25; i <= 150; i += 25)
		{
			if (heroData.currentLevel >= i)
			{
				cell.img_perk.sprite = UIController.instance.multiplyImages.perks[(i - 1) / 25].active;
			}
		}
		cell.img_perk.SetNativeSize();
		cell.lbl_level.text = heroData.currentLevel.ToString();
	}

	public override void UpdateShortInfoText()
	{
		if (!heroData.isOpened)
		{
			cell.lbl_shortInfo.transform.parent.gameObject.SetActive(false);
			return;
		}
		cell.lbl_shortInfo.transform.parent.gameObject.SetActive(true);
		cell.lbl_shortInfo.text = AbbreviationUtility.AbbreviateNumber(GetLevelPower(heroData.currentLevel - 1) * PassiveAbilitiesManager.bonusHelper.GetHeroAddedPower(), 2).AddHexColor("FFFFFF");
		if (heroData.currentLevel < DataLoader.playerData.survivorMaxLevel)
		{
			cell.lbl_shortInfo.text += (" + " + AbbreviationUtility.AbbreviateNumber((GetLevelPower(heroData.currentLevel) - GetLevelPower(heroData.currentLevel - 1)) * PassiveAbilitiesManager.bonusHelper.GetHeroAddedPower(), 2)).AddHexColor("FFC200");
		}
	}

	private float GetLevelPower(int level)
	{
		return DataLoader.Instance.survivors[cell.cellIndex].levels[level].power * 10f;
	}
}
