using UnityEngine;

public class UIHeroInfoPerkController : UIBaseScrollPanel<UIHeroInfoPerk>
{
	[SerializeField]
	private HeroInfo heroInfo;

	public override void UpdateAllContent()
	{
		int num = -1;
		for (int i = 0; i < dataArray.Length; i++)
		{
			bool flag = heroInfo.GetCurrentHeroLevel() >= (i + 1) * 25;
			dataArray[i].UpdateContent(flag);
			if (flag)
			{
				num = i;
			}
		}
		num -= 2;
		if (num > 0)
		{
			scrollRect.content.anchoredPosition = new Vector2(0f, 104 * num);
		}
	}

	public override int GetCellCount()
	{
		return DataLoader.playerData.survivorMaxLevel / 25;
	}
}
