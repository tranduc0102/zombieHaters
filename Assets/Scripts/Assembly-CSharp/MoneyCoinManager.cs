using System;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCoinManager : MonoBehaviour
{
	[Serializable]
	private struct CCBL
	{
		public int[] values;
	}

	public static MoneyCoinManager instance;

	private MoneyCoinSpawn[] places;

	[SerializeField]
	private CCBL[] countCoinsByLevel;

	[SerializeField]
	private GameObject prefabMoneyCoin;

	[SerializeField]
	public int coinMultyplier = 30;

	[HideInInspector]
	public double coinMoney;

	private void Awake()
	{
		instance = this;
	}

	public void StartGame()
	{
		places = UnityEngine.Object.FindObjectsOfType<MoneyCoinSpawn>();
		List<MoneyCoinSpawn> list = new List<MoneyCoinSpawn>();
		MoneyCoinSpawn[] array = places;
		foreach (MoneyCoinSpawn moneyCoinSpawn in array)
		{
			if (moneyCoinSpawn.openAtLevel <= DataLoader.Instance.GetCurrentPlayerLevel() && moneyCoinSpawn.worldNumber == GameManager.instance.currentWorldNumber)
			{
				list.Add(moneyCoinSpawn);
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("Compatible places for spawn MoneyCoins Not Found!");
			return;
		}
		for (int j = 0; j < countCoinsByLevel[GameManager.instance.currentWorldNumber - 1].values[DataLoader.Instance.GetCurrentPlayerLevel()]; j++)
		{
			if (list.Count <= 0)
			{
				break;
			}
			int index = UnityEngine.Random.Range(0, list.Count);
			UnityEngine.Object.Instantiate(prefabMoneyCoin, list[index].transform.position, default(Quaternion), TransformParentManager.Instance.moneyBox);
			list.RemoveAt(index);
		}
		UpdateCoinMoney();
	}

	public void EndGame()
	{
		MoneyCoin[] array = UnityEngine.Object.FindObjectsOfType<MoneyCoin>();
		MoneyCoin[] array2 = array;
		foreach (MoneyCoin moneyCoin in array2)
		{
			UnityEngine.Object.Destroy(moneyCoin.gameObject);
		}
	}

	public void UpdateCoinMoney()
	{
		coinMoney = Mathf.Round(Mathf.Pow(1.35f, DataLoader.Instance.GetCurrentPlayerLevel() - 1) * (float)coinMultyplier * (PassiveAbilitiesManager.bonusHelper.GoldBonus + 1f));
	}
}
