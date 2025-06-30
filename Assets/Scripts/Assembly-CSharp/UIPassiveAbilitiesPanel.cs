using System;

public class UIPassiveAbilitiesPanel : UIBaseScrollPanel<UIPassiveAbilityCell>
{
	public override void CreateCells()
	{
		base.CreateCells();
		UpdateAllContent();
	}

	public override int GetCellCount()
	{
		return Enum.GetValues(typeof(PassiveAbilityTypes)).Length;
	}

	public override void UpdateAllContent()
	{
		if (dataArray != null)
		{
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].UpdateContent();
			}
		}
	}

	public void OnEnable()
	{
		UpdateAllContent();
	}
}
