using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesSpawn : PointOnMap
{
	public bool isIdleSpawn;

	private bool spawnInactiveZombies;

	private void Start()
	{
		if (!isIdleSpawn)
		{
			WavesManager.instance.zombiesSpawns.Add(this);
		}
	}

	public void Spawn()
	{
		if (GameManager.instance.zombies.Count < WavesManager.instance.maxCountZombies)
		{
			spawnInactiveZombies = false;
			StartCoroutine(SpawnProcess(WavesManager.instance.maxCurrentWavePower));
		}
	}

	public void Spawn(float power, bool inactiveZombies = false)
	{
		spawnInactiveZombies = inactiveZombies;
		StartCoroutine(SpawnProcess(power));
	}

	private IEnumerator SpawnProcess(float maxCurrentWavePower)
	{		
		GameManager.GameModes startOnMode = GameManager.instance.currentGameMode;
		List<WavesManager.ZombieRank> zombies = new List<WavesManager.ZombieRank>();
		List<int> chances = new List<int>(WavesManager.instance.chances);
		for (int i = 0; i < WavesManager.instance.zombies.Count; i++)
		{
			zombies.Add(new WavesManager.ZombieRank());
			zombies[i].zombies = new List<WavesManager.ZombieVariants>(WavesManager.instance.zombies[i].zombies);
			zombies[i].chances = new List<int>(WavesManager.instance.zombies[i].chances);
		}
		float currentWavePower = 0f;
		int countZombiesToMax = WavesManager.instance.maxCountZombies - GameManager.instance.zombies.Count;
		while (currentWavePower < maxCurrentWavePower && (countZombiesToMax > 0 || spawnInactiveZombies))
		{
			ZombieHuman zombie2 = null;
			do
			{
				int balanceNum = GetBalanceNum(chances);
				zombie2 = zombies[balanceNum].GetZombie(maxCurrentWavePower - currentWavePower);
				if (zombie2 == null)
				{
					zombies.RemoveAt(balanceNum);
					chances.RemoveAt(balanceNum);
				}
				if (zombies.Count <= 0)
				{
					yield break;
				}
			}
			while (zombie2 == null);
			ZombieHuman zh = Object.Instantiate(zombie2, new Vector3(base.transform.position.x + Random.Range(-1f, 2f), 0f, base.transform.position.z + Random.Range(-1f, 2f)), default(Quaternion), TransformParentManager.Instance.zombies);
			zh.isActive = !spawnInactiveZombies;
			currentWavePower += zh.power;
			countZombiesToMax--;
			yield return new WaitForSeconds(WavesManager.instance.spawnSmooth);
			if (GameManager.instance.currentGameMode != startOnMode)
			{
				break;
			}
		}
	}

	private int GetBalanceNum(List<int> chances)
	{
		int num = 0;
		foreach (int chance in chances)
		{
			num += chance;
		}
		int num2 = Random.Range(0, num);
		int num3 = 0;
		for (int i = 0; i < chances.Count; i++)
		{
			num3 += chances[i];
			if (num2 < num3)
			{
				return i;
			}
		}
		return 0;
	}
}
