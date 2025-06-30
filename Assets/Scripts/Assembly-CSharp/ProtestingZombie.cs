using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestingZombie : ZombieHuman
{
	[SerializeField]
	private float spawnDistance = 5f;

	[SerializeField]
	private float spawnDelay = 1.5f;

	[SerializeField]
	private float waitTime = 1f;

	[SerializeField]
	private int spawnCount = 3;

	private ParticleSystem protestingZombieFx;

	private bool readyToSpawn = true;

	private new void FixedUpdate()
	{
		base.FixedUpdate();
		if (targetMove != null && readyToSpawn && Vector3.Distance(base.transform.position, targetMove.position) <= spawnDistance)
		{
			readyToSpawn = false;
			Invoke("ReadyToSpawn", spawnDelay);
			StartCoroutine(SpawnZombie());
		}
	}

	private void ReadyToSpawn()
	{
		readyToSpawn = true;
	}

	private IEnumerator SpawnZombie()
	{
		if (protestingZombieFx == null)
		{
			protestingZombieFx = Object.Instantiate(GameManager.instance.prefabProtestingZombieFx, base.transform);
		}
		protestingZombieFx.Play();
		float tempSpeed = defaultSpeed;
		defaultSpeed = 0f;
		yield return new WaitForSeconds(waitTime / 2f);
		WavesManager.ZombieRank zombies = new WavesManager.ZombieRank
		{
			zombies = new List<WavesManager.ZombieVariants>(WavesManager.instance.zombies[0].zombies),
			chances = new List<int>(WavesManager.instance.zombies[0].chances)
		};
		for (int i = 0; i < spawnCount; i++)
		{
			Object.Instantiate(GameManager.instance.prefabZombieSpawnFx, Object.Instantiate(zombies.GetZombie(99999f), new Vector3(base.transform.position.x + Random.Range(-0.5f, 0.6f), base.transform.position.y, base.transform.position.z + Random.Range(-0.5f, 0.6f)), base.transform.rotation).transform).Play();
		}
		yield return new WaitForSeconds(waitTime / 2f);
		defaultSpeed = tempSpeed;
	}
}
