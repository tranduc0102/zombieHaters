using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PVPPlayerInfo
{
	public List<SurvivorHuman> survivors;

	public int groupArmorBonus;

	public int groupDamageBonus;

	public float groupLootCount;

	public BasePVPGroupController controller;

	public string name;

	public Color color;

	public int place;

	public Vector3 GetGroupCenter()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < survivors.Count; i++)
		{
			if (survivors[i] != null)
			{
				zero += survivors[i].transform.position;
			}
		}
		return zero / survivors.Count;
	}

	public bool TryToGetGroupCenter(out Vector3 groupCenter)
	{
		groupCenter = Vector3.zero;
		if (survivors.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < survivors.Count; i++)
		{
			if (survivors[i] != null)
			{
				groupCenter += survivors[i].transform.position;
			}
		}
		groupCenter /= (float)survivors.Count;
		return true;
	}

	public void IncreaseLoot(float count)
	{
		if (controller.level < PVPManager.Instance.levels.Length && groupLootCount + count >= (float)PVPManager.Instance.levels[controller.level] && controller.level < PVPManager.Instance.levels.Length - 1)
		{
			controller.level++;
			if (controller.level > PVPManager.Instance.levels.Length - 1)
			{
				controller.level = PVPManager.Instance.levels.Length - 1;
			}
			controller.AddSurvivor();
			DataLoader.gui.pvpScoreBoard.UpdateLeaderBoard();
		}
		groupLootCount += count;
		if (groupLootCount > (float)PVPManager.Instance.levels.Last())
		{
			groupLootCount = PVPManager.Instance.levels.Last();
		}
		UpdateBar();
		controller.UpdateLootText(groupLootCount, color);
	}

	public bool IsAlive()
	{
		return survivors.Count > 0;
	}

	public void DecreaseLevel()
	{
		if (controller.level > 0)
		{
			groupLootCount -= PVPManager.Instance.levels[controller.level] - PVPManager.Instance.levels[controller.level - 1];
			controller.level--;
			controller.UpdateLootText(groupLootCount, color);
		}
		DataLoader.gui.pvpScoreBoard.UpdateLeaderBoard();
		UpdateBar();
	}

	public void UpdateBar()
	{
		if (controller.pvpPlayerIndex == 0)
		{
			int num = ((controller.level > 0) ? PVPManager.Instance.levels[controller.level - 1] : 0);
			DataLoader.gui.pvpLevelFillBar.FillBar((groupLootCount - (float)num) / (float)(PVPManager.Instance.levels[controller.level] - num));
			DataLoader.gui.lbl_pvpPlayerLevel.text = controller.level.ToString();
		}
	}
}
