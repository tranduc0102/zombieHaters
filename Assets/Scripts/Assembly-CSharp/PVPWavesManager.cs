using System.Collections.Generic;
using UnityEngine;

public class PVPWavesManager : MonoBehaviour
{
	public static PVPWavesManager instance;

	private ZombiesSpawn[] zombiesSpawns;

	private Transform cameraTarget;

	private float minDistance = 25f;

	[SerializeField]
	private float minDistanceToBots = 28f;

	[SerializeField]
	private float spawnDelay = 1f;

	private float spawnsPowerMultiplier = 1f;

	private List<Transform> bots = new List<Transform>();

	[Space]
	public int zombiesLimiterWaveNumber = 1;

	public void AddBotToList(Transform botObject)
	{
		bots.Add(botObject);
	}

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(instance);
		}
		instance = this;
	}

	private void Start()
	{
		cameraTarget = Object.FindObjectOfType<CameraTarget>().transform;
	}

	public void StartSpawnZombies()
	{
		Debug.Log("ArenaWavesManager.StartSpawnZombies");
		GameManager.instance.DeleteAllInactiveZombies();
		zombiesSpawns = Object.FindObjectsOfType<ZombiesSpawn>();
		SpawnZombies();
	}

	private void SpawnZombies()
	{
		if (zombiesSpawns.Length > 0)
		{
			FindBestSpawnAndDoIt();
			Invoke("SpawnZombies", spawnDelay);
		}
	}

	private void FindBestSpawnAndDoIt()
	{
		float[] array = new float[zombiesSpawns.Length];
		for (int i = 0; i < zombiesSpawns.Length; i++)
		{
			if (zombiesSpawns[i] == null)
			{
				array[i] = 0f;
			}
			else
			{
				array[i] = Vector3.Distance(cameraTarget.position, zombiesSpawns[i].transform.position);
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			for (int k = j; k < array.Length; k++)
			{
				if (array[k] < array[j])
				{
					float num = array[k];
					array[k] = array[j];
					array[j] = num;
					ZombiesSpawn zombiesSpawn = zombiesSpawns[k];
					zombiesSpawns[k] = zombiesSpawns[j];
					zombiesSpawns[j] = zombiesSpawn;
				}
			}
		}
		for (int l = 0; l < array.Length; l++)
		{
			if (!(array[l] > minDistance))
			{
				continue;
			}
			bool flag = false;
			foreach (Transform bot in bots)
			{
				if (Vector3.Distance(zombiesSpawns[l].transform.position, bot.position) < minDistanceToBots)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				zombiesSpawns[l].Spawn(ArenaManager.instance.GetCurrentStagePower() * spawnsPowerMultiplier);
				break;
			}
		}
	}

	public void StopSpawnZombies()
	{
		Debug.Log("ArenaWavesManager.StopSpawnZombies");
		CancelInvoke("SpawnZombies");
	}
}
