using System.Collections.Generic;
using UnityEngine;

public class FuelCanister : ZombiesSpawn
{
	private bool isEmpty;

	[SerializeField]
	private List<ZombiesSpawn> zombiesSpawns;

	[SerializeField]
	private int countSpawnsAtStart = 2;

	[Space]
	[SerializeField]
	private float onCanisterPowerMultiplier = 1f;

	[SerializeField]
	private float otherSpawnsPowerMultiplier = 1f;

	[SerializeField]
	private AudioClip getFuelClip;

	[SerializeField]
	private ParticleSystem pickUpFx;

	private void Start()
	{
		Spawn(ArenaManager.instance.GetCurrentStagePower() * onCanisterPowerMultiplier, true);
		while (countSpawnsAtStart > 0 && zombiesSpawns.Count > 0)
		{
			int index = Random.Range(0, zombiesSpawns.Count);
			zombiesSpawns[index].Spawn(ArenaManager.instance.GetCurrentStagePower() * otherSpawnsPowerMultiplier, true);
			zombiesSpawns.RemoveAt(index);
			countSpawnsAtStart--;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isEmpty && other.tag == "Survivor")
		{
			CarControll.countGasInHands++;
			DataLoader.gui.RefreshCarCaravanProgress();
			SoundManager.Instance.PlaySound(getFuelClip, 1f);
			Object.Destroy(base.gameObject);
			isEmpty = true;
			pickUpFx.transform.parent = null;
			pickUpFx.Play();
			Object.Destroy(pickUpFx.gameObject, 2f);
			DataLoader.gui.carPointer.ShowArrow(true);
		}
	}
}
