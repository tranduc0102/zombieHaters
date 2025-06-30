using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBottomScrollPanel : UIBaseScrollPanel<UIBottomScrollCell>
{
	[SerializeField]
	private UIBottomPanelButtons buttonsPanel;

	public UIBottomExtendedInfoPopup extendedInfo;

	public override int GetCellCount()
	{
		return 10;
	}

	public int GetExtendedCellCount()
	{
		List<int> list = new List<int>();
		list.Add(DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel);
		list.Add(DataLoader.playerData.survivorMaxLevel / 25);
		return list.Max();
	}

	public int GetCellsCount<T>() where T : UIBaseBottomScrollCellContent
	{
		if (typeof(T) == typeof(UIHeroesScrollCellContent))
		{
			return DataLoader.playerData.heroData.Count;
		}
		if (typeof(T) == typeof(UIPassiveAbilitiesScrollCellContent))
		{
			return DataLoader.playerData.passiveAbilities.Count;
		}
		if (typeof(T) == typeof(UIBossesScrollCellContent))
		{
			return WavesManager.instance.bossesByWorld[GameManager.instance.currentWorldNumber - 1].bosses.Length;
		}
		if (typeof(T) == typeof(UIWorldsScrollCellContent))
		{
			return GameManager.instance.worldNames.Length;
		}
		if (typeof(T) == typeof(UIBoostersScrollCellContent))
		{
			return Enum.GetValues(typeof(UIBoostersScrollCellContent.CellBoosters)).Length;
		}
		return 0;
	}

	public override void CreateCells()
	{
	}

	public void SetNewContentType<T>() where T : UIBaseBottomScrollCellContent
	{
		int cellsCount = GetCellsCount<T>();
		UIController.instance.ResizeScrollContent(cellPrefab, scrollRect, cellsCount, startBorder, distanceBetweenCells);
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (i < cellsCount)
			{
				dataArray[i].gameObject.SetActive(true);
				dataArray[i].SetNewContent<T>();
			}
			else
			{
				dataArray[i].gameObject.SetActive(false);
			}
		}
		ScrollToTop();
	}

	public void ScrollToTop()
	{
		scrollRect.normalizedPosition = new Vector2(0f, 1f);
	}

	public override void UpdateAllContent()
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (dataArray[i].gameObject.activeInHierarchy)
			{
				dataArray[i].UpdateContent();
			}
		}
	}
}
