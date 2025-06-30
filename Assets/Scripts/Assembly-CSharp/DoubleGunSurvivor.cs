using System;
using UnityEngine;

public class DoubleGunSurvivor : SoldierSurvivor
{
	[SerializeField]
	private ParticleSystem bulletsSpawn2;

	private bool firstGunNow = true;

	private ParticleSystem[] bulletsSpawns;

	private new void Start()
	{
		base.Start();
		bulletsSpawns = new ParticleSystem[2] { bulletsSpawn, bulletsSpawn2 };
	}

	protected override void AddBullet()
	{
		bulletsSpawn = bulletsSpawns[Convert.ToInt16(firstGunNow)];
		firstGunNow = !firstGunNow;
		base.AddBullet();
	}
}
