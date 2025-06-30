using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
	public enum WeatherType
	{
		Rain = 0,
		Dust = 1,
		Empty = 2
	}

	[Serializable]
	public class WeatherObject
	{
		public WeatherType type;

		public GameObject obj;

		public int weight;

		public void SetObjectActive(bool active)
		{
			if (obj != null)
			{
				obj.SetActive(active);
			}
		}
	}

	[SerializeField]
	private List<WeatherObject> weatherWeights;

	public static WeatherManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void EnableRandomWeather()
	{
		weatherWeights.ForEach(delegate(WeatherObject item)
		{
			item.SetObjectActive(false);
		});
		List<WeatherObject> list = weatherWeights;
		int max = list.Sum((WeatherObject c) => c.weight);
		int num = UnityEngine.Random.Range(0, max);
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = num2; j < list[i].weight + num2; j++)
			{
				if (j >= num)
				{
					list[i].SetObjectActive(true);
					return;
				}
			}
			num2 += list[i].weight;
		}
		Debug.LogWarning("Something Wrong With Weather Weights");
		weatherWeights.First().SetObjectActive(true);
	}
}
