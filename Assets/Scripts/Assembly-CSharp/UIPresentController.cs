using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPresentController : MonoBehaviour
{
	[Serializable]
	public struct PresentInfo
	{
		public UIPresent present;

		public int weight;

		[NonSerialized]
		public int hitsInRow;
	}

	[SerializeField]
	private List<PresentInfo> presents;

	private int maxHitsInRow = 2;

	public void TryToShowPresent(int money)
	{
		if (GameManager.instance.inGameTime > 30 && GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay && money > 50)
		{
			DataLoader.gui.ChangeAnimationState("Present");
			for (int i = 0; i < presents.Count; i++)
			{
				presents[i].present.gameObject.SetActive(false);
			}
			ChoosePresent().present.SetContent(money);
		}
		else
		{
			DataLoader.gui.ChangeAnimationState("GameOver");
		}
	}

	public PresentInfo ChoosePresent()
	{
		List<PresentInfo> list = new List<PresentInfo>();
		for (int i = 0; i < presents.Count; i++)
		{
			if (presents[i].hitsInRow < maxHitsInRow)
			{
				list.Add(presents[i]);
				continue;
			}
			PresentInfo value = presents[i];
			value.hitsInRow = 0;
			presents[i] = value;
		}
		int max = list.Sum((PresentInfo c) => c.weight);
		int num = UnityEngine.Random.Range(0, max);
		int num2 = 0;
		for (int j = 0; j < list.Count; j++)
		{
			for (int k = num2; k < list[j].weight + num2; k++)
			{
				if (k < num)
				{
					continue;
				}
				PresentInfo value = list[j];
				value.hitsInRow++;
				for (int l = 0; l < presents.Count; l++)
				{
					if (presents[l].present.name == list[j].present.name)
					{
						presents[l] = value;
					}
				}
				return list[j];
			}
			num2 += list[j].weight;
		}
		Debug.LogWarning("Something Wrong With Present Weights");
		return presents.First();
	}
}
