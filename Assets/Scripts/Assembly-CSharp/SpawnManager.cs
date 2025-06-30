using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	[Serializable]
	private struct CRSB
	{
		public int[] values;
	}

	public static SpawnManager instance;

	[HideInInspector]
	public List<SurvivorSpawn> survivorSpawns;

	[SerializeField]
	private CRSB[] countRecumbentSurvivorsByLevel;

	[SerializeField]
	private float dropDelay = 20f;

	[SerializeField]
	public NewSurvivor[] survivors;

	[SerializeField]
	private int[] chances;

	[SerializeField]
	private GameObject prefabParashute;

	[SerializeField]
	private Transform cameraTarget;

	[SerializeField]
	private float parashuteStartHeight = 15f;

	private float minDistance = 20f;

	private void Awake()
	{
		instance = this;
	}

	public void StartGame()
	{
		if (dropDelay > 0f)
		{
			Invoke("DropCage", dropDelay);
		}
		if (GameManager.instance.currentWorldNumber - 1 < countRecumbentSurvivorsByLevel.Length)
		{
			for (int i = 0; i < countRecumbentSurvivorsByLevel[GameManager.instance.currentWorldNumber - 1].values[DataLoader.Instance.GetCurrentPlayerLevel()]; i++)
			{
				OneMoreDrop();
			}
		}
	}

	private void DropCage()
	{
		OneMoreDrop();
		Invoke("DropCage", dropDelay);
	}

	private int GetNumRandSurv()
	{
		int num = 0;
		int[] array = chances;
		foreach (int num2 in array)
		{
			num += num2;
		}
		int num3 = UnityEngine.Random.Range(0, num);
		int num4 = 0;
		for (int j = 0; j < chances.Length; j++)
		{
			num4 += chances[j];
			if (num3 < num4)
			{
				return j;
			}
		}
		return 0;
	}

	public NewSurvivor GetNewSurvivor()
	{
		NewSurvivor newSurvivor = null;
		do
		{
			newSurvivor = survivors[GetNumRandSurv()];
		}
		while (!DataLoader.playerData.IsHeroOpened(newSurvivor.heroType));
		return newSurvivor;
	}

	public SaveData.HeroData.HeroType GetRandomSurvivorType()
	{
		SaveData.HeroData.HeroType heroType;
		do
		{
			heroType = survivors[GetNumRandSurv()].heroType;
		}
		while (!DataLoader.playerData.IsHeroOpened(heroType));
		return heroType;
	}

	public void OneMoreDrop()
	{
		List<SurvivorSpawn> list = new List<SurvivorSpawn>();
		foreach (SurvivorSpawn survivorSpawn in survivorSpawns)
		{
			if (survivorSpawn.isReady() && Vector3.Distance(survivorSpawn.transform.position, cameraTarget.position) > minDistance)
			{
				list.Add(survivorSpawn);
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("Compatible places for spawn NewSurvivor Not Found!");
			return;
		}
		list[UnityEngine.Random.Range(0, list.Count)].Spawn();
		GameManager.instance.newSurvivorsLeft++;
	}

	public void TutorialDrop()
	{
		foreach (SurvivorSpawn survivorSpawn in survivorSpawns)
		{
			if (survivorSpawn.worldNumber < 1)
			{
				survivorSpawn.Spawn();
				break;
			}
		}
		GameManager.instance.newSurvivorsLeft++;
	}

	public void OneMoreParashute()
	{
		UnityEngine.Object.Instantiate(prefabParashute, new Vector3(cameraTarget.position.x, parashuteStartHeight, cameraTarget.position.z), default(Quaternion));
	}

	public void StopIt()
	{
		CancelInvoke("DropCage");
	}

	public void Reset()
	{
		survivorSpawns.Clear();
	}
}
