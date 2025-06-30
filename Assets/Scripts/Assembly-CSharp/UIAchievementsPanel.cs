using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementsPanel : UIBaseScrollPanel<AchievementContent>
{
	[HideInInspector]
	public int newAchievementsCount;

	[SerializeField]
	private GameObject newAchievementsObj;

	[SerializeField]
	private GameObject newAchievementsInPopupObj;

	[SerializeField]
	private Text newAchievementsCountText;

	[SerializeField]
	private Text newAchievementsCountPopupText;

	public override void CreateCells()
	{
		base.CreateCells();
		UpdateAllContent();
	}

	public override void UpdateAllContent()
	{
		if (dataArray != null)
		{
			newAchievementsObj.SetActive(false);
			newAchievementsInPopupObj.SetActive(false);
			newAchievementsCount = 0;
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].UpdateContent();
			}
			newAchievementsCountText.text = newAchievementsCount.ToString();
			newAchievementsCountPopupText.text = newAchievementsCountText.text;
			newAchievementsObj.SetActive(newAchievementsCount > 0);
			newAchievementsInPopupObj.SetActive(newAchievementsCount > 0);
		}
	}

	public void MarkAchievementsCompleted()
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (dataArray[i].claimType == AchievementContent.ClaimType.READY)
			{
				dataArray[i].MarkComleted();
			}
		}
	}

	public void UpdateLocalization()
	{
		if (dataArray != null)
		{
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].UpdateLocalizaton();
			}
		}
	}

	public void Sort()
	{
		List<RectTransform> list = new List<RectTransform>();
		int num = -1;
		AchievementContent[] array = dataArray.OrderByDescending((AchievementContent a) => a.GetProgressPercentage()).ToArray();
		for (int num2 = array.Length - 1; num2 >= 0; num2--)
		{
			switch (array[num2].claimType)
			{
			case AchievementContent.ClaimType.READY:
				list.Insert(0, array[num2].rectTransform);
				num++;
				break;
			case AchievementContent.ClaimType.NOTREADY:
				list.Insert(num + 1, array[num2].rectTransform);
				break;
			case AchievementContent.ClaimType.CLAIMED:
				list.Add(array[num2].rectTransform);
				break;
			}
		}
		float num3 = cellPrefab.rect.y;
		for (int i = 0; i < list.Count; i++)
		{
			list[i].anchoredPosition = new Vector2(cellPrefab.anchoredPosition.x, num3);
			num3 -= cellPrefab.rect.height * cellPrefab.localScale.y + distanceBetweenCells;
		}
	}

	public override int GetCellCount()
	{
		return DataLoader.Instance.achievements.Count;
	}
}
