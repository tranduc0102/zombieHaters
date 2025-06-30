using System;
using System.Collections.Generic;

public class UIBossesScrollCellContent : UIBaseBottomScrollCellContent
{
	private KilledBosses previousBoss;

	private KilledBosses killedBoss;

	private WavesManager.Boss waveBoss;

	public override void SetNewContent(UIBottomScrollCell cell)
	{
		waveBoss = WavesManager.instance.bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[cell.cellIndex];
		killedBoss = DataLoader.playerData.killedBosses.Find((KilledBosses item) => item.name == waveBoss.prefabBoss.myNameIs);
		if (cell.cellIndex > 0)
		{
			previousBoss = DataLoader.playerData.killedBosses.Find((KilledBosses item) => item.name == WavesManager.instance.bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses[cell.cellIndex - 1].prefabBoss.myNameIs);
		}
		else
		{
			previousBoss = null;
		}
		base.SetNewContent(cell);
		UpdateCellName(LanguageManager.instance.GetLocalizedText(waveBoss.prefabBoss.myNameIs));
		cell.lbl_unlock.text = "Info";
	}

	public override void UpdateCellName(string text)
	{
		if (previousBoss == null && cell.cellIndex != 0)
		{
			base.UpdateCellName("???");
		}
		else
		{
			base.UpdateCellName(text);
		}
	}

	public override void ButtonBuyLogic()
	{
		if (IsReadyToClaim())
		{
			DataLoader.Instance.RefreshGems(GetGemsRewardAmount());
			killedBoss.rewardedStage++;
			killedBoss.count = 0;
			DataLoader.Instance.SavePlayerData();
			SoundManager.Instance.PlayClickSound();
			AnalyticsManager.instance.LogEvent("ClaimBossReward", new Dictionary<string, string>
			{
				{
					"BossName",
					waveBoss.prefabBoss.myNameIs
				},
				{
					"Gems",
					GetGemsRewardAmount().ToString()
				}
			});
			UpdateContent();
		}
	}

	public bool IsReadyToClaim()
	{
		if (killedBoss == null)
		{
			return false;
		}
		if (killedBoss.rewardedStage >= StaticConstants.bossStages.Length)
		{
			return false;
		}
		return killedBoss.count >= StaticConstants.bossStages[killedBoss.rewardedStage];
	}

	public override void ButtonInfoLogic()
	{
		base.ButtonInfoLogic();
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetPopupHeight(1100f);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetContent(false, LanguageManager.instance.GetLocalizedText(waveBoss.prefabBoss.myNameIs), "Boss Short Description", "Pizza Reward", string.Empty);
	}

	public override void UpdateExtendedInfoVariables()
	{
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetVariables(cell.lbl_shortInfo.text);
	}

	public override UIBottomScrollCell.TopTextType GetTopTextType()
	{
		return UIBottomScrollCell.TopTextType.FullSize;
	}

	public override void SetIcon()
	{
		throw new NotImplementedException();
	}

	public override void SetShortInfoIcon()
	{
		cell.img_shortInfo.gameObject.SetActive(true);
		cell.img_shortInfo.sprite = UIController.instance.multiplyImages.bottomPanelShortInfoIcons[2];
	}

	public override void UpdateShortInfoText()
	{
		cell.lbl_shortInfo.text = GetGemsRewardAmount().ToString();
	}

	public override void UpdateTopContent()
	{
		cell.btn_unlock.gameObject.SetActive(previousBoss == null && cell.cellIndex != 0);
	}

	public override void UpdateButtonBuyText()
	{
		if (killedBoss == null)
		{
			cell.lbl_buy.text = "0/" + StaticConstants.bossStages[0];
		}
		else if (killedBoss.rewardedStage >= StaticConstants.bossStages.Length)
		{
			cell.lbl_buy.text = "Completed";
		}
		else
		{
			cell.lbl_buy.text = StaticConstants.bossStages[killedBoss.rewardedStage] - (StaticConstants.bossStages[killedBoss.rewardedStage] - killedBoss.count) + "/" + StaticConstants.bossStages[killedBoss.rewardedStage];
		}
	}

	private int GetGemsRewardAmount()
	{
		if (killedBoss == null)
		{
			return waveBoss.gemsReward;
		}
		switch (killedBoss.rewardedStage)
		{
		case 0:
			return waveBoss.gemsReward;
		case 1:
			return 1;
		case 2:
			return 1;
		case 3:
			return 1;
		default:
			return 0;
		}
	}
}
