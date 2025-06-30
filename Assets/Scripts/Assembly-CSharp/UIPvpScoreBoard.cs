using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPvpScoreBoard : MonoBehaviour
{
	[Serializable]
	public class ScoreBoardCell
	{
		public GameObject obj;

		public Text place;

		public Text playerName;

		public Text level;

		public void SetCellText(int place, string name, int level, Color color)
		{
			this.place.color = color;
			this.place.text = "#" + place;
			playerName.color = color;
			playerName.text = name;
			this.level.color = color;
			this.level.text = level.ToString();
		}
	}

	[SerializeField]
	private Transform playerBgnd;

	[SerializeField]
	private List<ScoreBoardCell> cells;

	private float lastLeaderboardUpdateTime;

	private Coroutine updateRequest;

	public void UpdateLeaderBoard()
	{
		if (Time.time - lastLeaderboardUpdateTime > 1f)
		{
			lastLeaderboardUpdateTime = Time.time;
			List<PVPPlayerInfo> source = ((!PVPManager.pvpPlayers[0].IsAlive()) ? GetTopBots(cells.Count) : GetTopBots(cells.Count - 1));
			source = source.OrderBy((PVPPlayerInfo tb) => tb.survivors.Count).Reverse().ToList();
			int num = -1;
			if (PVPManager.pvpPlayers[0].IsAlive())
			{
				num = GetPlayerPlace();
			}
			int num2 = 1;
			ResetCells();
			for (int i = 0; i < source.Count; i++)
			{
				if (i == num)
				{
					num2++;
					cells[i].SetCellText(num + 1, PVPManager.pvpPlayers[0].name, PVPManager.pvpPlayers[0].survivors.Count, PVPManager.pvpPlayers[0].color);
					cells[i + 1].SetCellText(i + num2, source[i].name, source[i].controller.level, source[i].color);
					playerBgnd.position = cells[i].obj.transform.position;
				}
				else
				{
					cells[i + num2 - 1].SetCellText(i + num2, source[i].name, source[i].controller.level, source[i].color);
				}
			}
			if (num != -1 && (num > source.Count - 1 || source.Count == 0))
			{
				cells[source.Count].SetCellText(num + 1, PVPManager.pvpPlayers[0].name, PVPManager.pvpPlayers[0].survivors.Count, PVPManager.pvpPlayers[0].color);
				playerBgnd.position = cells[source.Count].obj.transform.position;
			}
		}
		else if (base.gameObject.activeInHierarchy)
		{
			if (updateRequest != null)
			{
				StopCoroutine(updateRequest);
			}
			updateRequest = StartCoroutine(CreateUpdateRequest());
		}
	}

	public IEnumerator CreateUpdateRequest()
	{
		yield return new WaitForSeconds(0.5f);
		UpdateLeaderBoard();
	}

	public string GetBotName(PVPPlayerInfo bot)
	{
		return "lvl " + bot.survivors.Count + " " + bot.name;
	}

	private int GetPlayerPlace()
	{
		List<PVPPlayerInfo> list = PVPManager.pvpPlayers.OrderBy((PVPPlayerInfo tb) => tb.survivors.Count).Reverse().ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].controller.pvpPlayerIndex == 0)
			{
				return i;
			}
		}
		return 0;
	}

	public List<PVPPlayerInfo> GetTopBots(int maxCount)
	{
		List<PVPPlayerInfo> list = new List<PVPPlayerInfo>();
		for (int i = 1; i < PVPManager.pvpPlayers.Count; i++)
		{
			if (PVPManager.pvpPlayers[i].IsAlive())
			{
				list.Add(PVPManager.pvpPlayers[i]);
				if (list.Count == maxCount)
				{
					break;
				}
			}
		}
		if (list.Count < maxCount)
		{
			return list;
		}
		for (int j = 1; j < PVPManager.pvpPlayers.Count; j++)
		{
			if (!PVPManager.pvpPlayers[j].IsAlive() || list.Contains(PVPManager.pvpPlayers[j]))
			{
				continue;
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (CompareSurvivorsCount(list[k], PVPManager.pvpPlayers[j]))
				{
					list[k] = PVPManager.pvpPlayers[j];
					break;
				}
			}
		}
		return list;
	}

	public bool CompareSurvivorsCount(PVPPlayerInfo oldSurv, PVPPlayerInfo newSurv)
	{
		return oldSurv.survivors.Count < newSurv.survivors.Count;
	}

	public void ResetCells()
	{
		for (int i = 0; i < cells.Count; i++)
		{
			cells[i].place.text = string.Empty;
			cells[i].level.text = string.Empty;
			cells[i].playerName.text = string.Empty;
		}
	}
}
