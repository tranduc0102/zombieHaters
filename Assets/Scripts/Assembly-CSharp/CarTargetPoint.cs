using UnityEngine;

public class CarTargetPoint : MonoBehaviour
{
	[SerializeField]
	public CarTargetPoint[] nextPoints;

	[Space]
	[SerializeField]
	private ZombiesSpawn[] zombiesSpawns;

	private float spawnDelay = 0.5f;

	private bool isPassed;

	private Transform cameraTarget;

	private float minDistance = 20f;

	private void Awake()
	{
		cameraTarget = Object.FindObjectOfType<CameraTarget>().transform;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (isPassed)
		{
			return;
		}
		CarControll component = other.GetComponent<CarControll>();
		if (component != null)
		{
			if (nextPoints.Length > 0)
			{
				int num = Random.Range(0, nextPoints.Length);
				component.NewTarget(nextPoints[num], nextPoints[num].nextPoints.Length <= 0);
				Object.Destroy(base.gameObject);
			}
			else
			{
				component.Stop();
				GameManager.instance.WinArena();
			}
			isPassed = true;
			StopSpawnZombies();
		}
	}

	public void SpawnZombies()
	{
		if (zombiesSpawns.Length <= 0)
		{
			return;
		}
		ZombiesSpawn[] array = zombiesSpawns;
		foreach (ZombiesSpawn zombiesSpawn in array)
		{
			if (Vector3.Distance(zombiesSpawn.transform.position, cameraTarget.position) > minDistance)
			{
				zombiesSpawn.Spawn(ArenaManager.instance.GetCurrentStagePower());
				break;
			}
		}
		Invoke("SpawnZombies", spawnDelay);
	}

	private void StopSpawnZombies()
	{
		CancelInvoke("SpawnZombiesGroup");
	}
}
