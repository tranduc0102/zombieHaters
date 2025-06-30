using System;

public class UIExtendedInfoScrollPanel : UIBaseScrollPanel<UIExtendedInfoScrollCell>
{
	private int cellsCountCreated;

	public override int GetCellCount()
	{
		return cellsCountCreated;
	}

	public override void UpdateAllContent()
	{
		throw new NotImplementedException();
	}

	public void CreateCells(int countCells)
	{
		cellsCountCreated = countCells;
		base.CreateCells();
	}

	public void SetContent(int startLevel, int cellsCount, string textUpgrade)
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (i < cellsCount)
			{
				dataArray[i].gameObject.SetActive(true);
				dataArray[i].lbl_level.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Level) + " " + (i + 1) * startLevel;
				dataArray[i].lbl_upgrade.text = textUpgrade;
			}
			else
			{
				dataArray[i].gameObject.SetActive(false);
			}
		}
		UIController.instance.ResizeScrollContent(cellPrefab, scrollRect, cellsCount, startBorder, distanceBetweenCells);
	}

	public void SetContent(int startLevel, int cellsCount, AbilityData ability)
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (i < cellsCount)
			{
				dataArray[i].gameObject.SetActive(true);
				dataArray[i].lbl_level.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Level) + " " + (i + 1) * startLevel;
				dataArray[i].lbl_upgrade.text = (ability.GetBonusBylevel(i) * 100f + "%").ToString();
			}
			else
			{
				dataArray[i].gameObject.SetActive(false);
			}
		}
		UIController.instance.ResizeScrollContent(cellPrefab, scrollRect, cellsCount, startBorder, distanceBetweenCells);
	}

	public void OnEnable()
	{
	}
}
