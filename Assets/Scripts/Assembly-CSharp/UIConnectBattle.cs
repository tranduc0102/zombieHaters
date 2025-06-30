using System.Collections.Generic;
using UnityEngine;

public class UIConnectBattle : MonoBehaviour
{
	[SerializeField]
	private UIPVPPlayerRow[] playerRows;

	public Color[] playersColors;

	private string[] botNames;

	private void OnEnable()
	{
		botNames = Resources.Load<TextAsset>("Names").text.Split('\n');
		PVPManager.pvpPlayers = new List<PVPPlayerInfo>();
		List<Color> list = new List<Color>();
		for (int i = 1; i < playersColors.Length; i++)
		{
			list.Add(playersColors[i]);
		}
		List<string> list2 = new List<string>();
		string[] array = botNames;
		foreach (string item in array)
		{
			list2.Add(item);
		}
		float num = 0f;
		float[] array2 = new float[playerRows.Length];
		float num2 = 0f;
		for (int k = 0; k < DataLoader.playerData.heroData.Count; k++)
		{
			num2 += DataLoader.Instance.GetHeroPowerByLevel(DataLoader.playerData.heroData[k].heroType, DataLoader.playerData.survivorMaxLevel);
		}
		for (int l = 0; l < playerRows.Length; l++)
		{
			PVPManager.pvpPlayers.Add(new PVPPlayerInfo
			{
				survivors = new List<SurvivorHuman>(),
				controller = new BasePVPGroupController(),
				groupLootCount = 0f
			});
			if (l == 0)
			{
				PVPManager.pvpPlayers[0].name = LeaderboardManager.instance.GetPlayerName();
				PVPManager.pvpPlayers[0].color = playersColors[0];
				float num3 = 0f;
				for (int m = 0; m < DataLoader.playerData.heroData.Count; m++)
				{
					if (DataLoader.playerData.IsHeroOpened(DataLoader.playerData.heroData[m].heroType))
					{
						num3 += DataLoader.Instance.GetHeroPower(DataLoader.playerData.heroData[m].heroType);
					}
				}
				array2[l] = num3;
			}
			else
			{
				string item2 = list2[Random.Range(0, list2.Count)];
				Color color = list[Random.Range(0, list.Count)];
				PVPManager.pvpPlayers[l].name = item2;
				PVPManager.pvpPlayers[l].color = color;
				list2.Remove(item2);
				list.Remove(color);
				array2[l] = array2[0] * Random.Range(0.8f, 1.2f);
				if (array2[l] > num2)
				{
					array2[l] = num2;
				}
			}
			num += array2[l];
		}
		float averagePowerValue = num / (float)playerRows.Length;
		for (int n = 0; n < playerRows.Length; n++)
		{
			playerRows[n].SetValues(PVPManager.pvpPlayers[n].name, array2[n], PVPManager.pvpPlayers[n].color, averagePowerValue);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(false);
		}
	}

	private int CalculateCurrentArenaIndex()
	{
		for (int i = 0; i < DataLoader.Instance.botsData.arenaRating.Count - 1; i++)
		{
			if (DataLoader.playerData.arenaRating < DataLoader.Instance.botsData.arenaRating[i + 1])
			{
				return i;
			}
		}
		return DataLoader.Instance.botsData.arenaRating.Count - 1;
	}
}
