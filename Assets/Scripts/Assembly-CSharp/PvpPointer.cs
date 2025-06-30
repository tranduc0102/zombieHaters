using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpPointer : BasePointer
{
	private int currentPvpPlayerIndex;

	private List<PVPPlayerInfo> pvpPlayers = new List<PVPPlayerInfo>();

	public Text levelText;

	private new void FixedUpdate()
	{
		targets.Clear();
		pvpPlayers.Clear();
		if ((bool)PVPManager.Instance && PVPManager.pvpPlayers[0].IsAlive())
		{
			for (int i = 1; i < PVPManager.pvpPlayers.Count; i++)
			{
				Vector3 groupCenter;
				if (PVPManager.pvpPlayers[i].TryToGetGroupCenter(out groupCenter))
				{
					targets.Add(groupCenter);
					pvpPlayers.Add(PVPManager.pvpPlayers[i]);
				}
			}
		}
		base.FixedUpdate();
		levelText.gameObject.SetActive(distanceText.gameObject.activeInHierarchy);
	}

	public void StartPointer()
	{
		base.enabled = true;
		pointerImage.gameObject.SetActive(true);
		distanceText.enabled = true;
	}

	protected override void SortTargets()
	{
		for (int i = 0; i < targets.Count; i++)
		{
			for (int j = i; j < targets.Count; j++)
			{
				if (Vector3.Distance(cameraTarget.position, targets[j]) < Vector3.Distance(cameraTarget.position, targets[i]))
				{
					Vector3 value = targets[j];
					targets[j] = targets[i];
					targets[i] = value;
					PVPPlayerInfo value2 = pvpPlayers[j];
					pvpPlayers[j] = pvpPlayers[i];
					pvpPlayers[i] = value2;
				}
			}
		}
	}

	protected override void SetDistanceText()
	{
		base.SetDistanceText();
		levelText.text = pvpPlayers[0].controller.level.ToString();
		levelText.color = pvpPlayers[0].color;
	}
}
