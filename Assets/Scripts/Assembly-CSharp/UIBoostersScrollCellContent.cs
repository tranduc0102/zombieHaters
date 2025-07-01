using System;
using System.Collections.Generic;
using ACEPlay.Bridge;
using GuiInGame;
using UnityEngine.Events;
using WoodenSword.ZombieHaters.UI;

public class UIBoostersScrollCellContent : UIBaseBottomScrollCellContent
{
	public enum CellBoosters
	{
		TimeWarp = 0,
		Survivor = 1,
		KillAll = 2,
		Gold = 3
	}

	private CellBoosters boosterType;

	public override void SetNewContent(UIBottomScrollCell cell)
	{
		base.SetNewContent(cell);
		boosterType = (CellBoosters)cell.cellIndex;
		switch (boosterType)
		{
		case CellBoosters.TimeWarp:
			UpdateCellName("TimeWarp");
			break;
		case CellBoosters.Survivor:
			UpdateCellName("Survivor");
			break;
		case CellBoosters.KillAll:
			UpdateCellName("KillAll");
			break;
		case CellBoosters.Gold:
			UpdateCellName("Gold");
			break;
		}
	}

	public override void ButtonBuyLogic()
	{
		if (boosterType == CellBoosters.KillAll || boosterType == CellBoosters.Survivor)
		{
			SaveData.BoostersData.BoosterType b = ((boosterType == CellBoosters.KillAll) ? SaveData.BoostersData.BoosterType.KillAll : SaveData.BoostersData.BoosterType.NewSurvivor);
			UnityEvent onDone = new UnityEvent();
			onDone.AddListener(delegate
			{
				DataLoader.Instance.BuyBoosters(b);
				UpdateContent();
				AnalyticsManager.instance.LogEvent("BuyBoosterVideo", new Dictionary<string, string> {
				{
					"Type",
					boosterType.ToString()
				} });
			});
			BridgeController.instance.ShowRewarded($"Show reward:{((b != SaveData.BoostersData.BoosterType.KillAll) ? AdName.RewardMoreSurvival : AdName.RewardKillAll)}", onDone);
       
         
            /*AdsManager.instance.ShowRewarded(delegate
			{
				
			}, ;*/
		}
	}

	public override void ButtonInfoLogic()
	{
		base.ButtonInfoLogic();
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetPopupHeight(1100f);
		UIController.instance.scrollControllers.bottomScrollController.extendedInfo.SetContent(false, "Booster Name", "Booster Short Description", "Amount", string.Empty);
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

	public override void UpdateButtonBuyText()
	{
		if (boosterType == CellBoosters.KillAll || boosterType == CellBoosters.Survivor)
		{
			cell.lbl_buy.text = "Video";
		}
		else
		{
			cell.lbl_buy.text = "?";
		}
	}

	public override void UpdateShortInfoText()
	{
		if (boosterType == CellBoosters.KillAll)
		{
			cell.lbl_shortInfo.text = "+1";
		}
		else if (boosterType == CellBoosters.Survivor)
		{
			cell.lbl_shortInfo.text = "+1";
		}
		else if (boosterType == CellBoosters.TimeWarp)
		{
			cell.lbl_shortInfo.text = "+??? gold";
		}
		else
		{
			cell.lbl_shortInfo.text = "99:99:99";
		}
		cell.lbl_shortInfo.text = cell.lbl_shortInfo.text.AddHexColor("FFFFFF");
	}

	public override void UpdateTopContent()
	{
		if (boosterType == CellBoosters.KillAll)
		{
			cell.lbl_amount.text = DataLoader.playerData.boosters.Find((SaveData.BoostersData item) => item.type == SaveData.BoostersData.BoosterType.KillAll).count.ToString();
		}
		else if (boosterType == CellBoosters.Survivor)
		{
			cell.lbl_amount.text = DataLoader.playerData.boosters.Find((SaveData.BoostersData item) => item.type == SaveData.BoostersData.BoosterType.NewSurvivor).count.ToString();
		}
		else
		{
			cell.lbl_amount.transform.parent.gameObject.SetActive(false);
		}
	}
}
