using System;
using UnityEngine;

[Serializable]
public class ZombiePrefabData
{
	public SaveData.ZombieData.ZombieType type;

	public GameObject[] zombiePrefabs;

	public int health;

	public int damage;

	[HideInInspector]
	public float power;

	public void SetPrefabData()
	{
		for (int i = 0; i < zombiePrefabs.Length; i++)
		{
			ZombieHuman component = zombiePrefabs[i].GetComponent<ZombieHuman>();
			component.zombieType = type;
			component.countHealth = health;
			component.damage = damage;
			component.power = power;
		}
	}
}
