using System.Collections;
using UnityEngine;

public class ArenaWavesManager : MonoBehaviour
{
	public static ArenaWavesManager instance;

	private ZombiesSpawn[] zombiesSpawns;

	private Transform cameraTarget;

	private float minDistance = 20f;

	[SerializeField]
	private float spawnDelay = 1f;

	[SerializeField]
	private float spawnsPowerMultiplier = 1f;

	public int timeBeforeStart = 60;

	[Space]
	public int zombiesLimiterWaveNumber = 1;

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
		for (int l = 0; l < array.Length - 2; l++)
		{
			if (array[l] > minDistance)
			{
				int num2 = Random.Range(0, 3);
				if (zombiesSpawns[l + num2] != null)
				{
					zombiesSpawns[l + num2].Spawn(ArenaManager.instance.GetCurrentStagePower() * spawnsPowerMultiplier);
					break;
				}
			}
		}
	}

	public void StartArenaTimer()
	{
		StartCoroutine(ArenaTimer());
	}

	private IEnumerator ArenaTimer()
	{
		int timeRemaining = timeBeforeStart;
		while (timeRemaining > 0)
		{
			DataLoader.gui.RefreshArenaTimeRemaining(timeRemaining);
			timeRemaining--;
			yield return new WaitForSeconds(1f);
		}
		StartSpawnZombies();
		DataLoader.gui.RefreshArenaTimeRemaining(timeRemaining, true);
	}

	public void StopArenaTimer(bool carStart = false)
	{
		StopAllCoroutines();
		DataLoader.gui.RefreshArenaTimeRemaining(0, carStart);
		StopSpawnZombies();
	}

	public void StopSpawnZombies()
	{
		Debug.Log("ArenaWavesManager.StopSpawnZombies");
		CancelInvoke("SpawnZombies");
	}
}
