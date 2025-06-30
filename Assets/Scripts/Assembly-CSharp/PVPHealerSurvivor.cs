using UnityEngine;

public class PVPHealerSurvivor : PVPSoldierSurvivor
{
	[Header("Heal parameters")]
	[SerializeField]
	private float healDistance = 4f;

	[SerializeField]
	private int healValue = 1;

	[SerializeField]
	public float healDelay = 1f;

	[SerializeField]
	private ParticleSystem healFx;

	private SurvivorHuman targetSurvivor;

	private Lightning lightning;

	public override void Start()
	{
		base.Start();
		isBaffed = true;
		Invoke("DoIt", healDelay);
		lightning = Object.Instantiate(bulletPrefab, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation).GetComponent<Lightning>();
		SetBulletDamage(lightning.gameObject);
		lightning.pvpIndex = groupController.pvpPlayerIndex;
	}

	private void DoIt()
	{
		if (targetSurvivor == null || !targetSurvivor.ReadyToHeal())
		{
			targetSurvivor = FindTargetSurvivor();
		}
		if (targetSurvivor != null && targetSurvivor.ReadyToHeal())
		{
			targetSurvivor.TakeDamage(-healValue);
			if (groupController.pvpPlayerIndex == 0)
			{
				SoundManager.Instance.PlayHealSound();
			}
			healFx.Play();
		}
		Invoke("DoIt", healDelay);
	}

	private SurvivorHuman FindTargetSurvivor()
	{
		SurvivorHuman survivorHuman = null;
		foreach (PVPSoldierSurvivor item in groupController.squad)
		{
			if (item != this && Vector3.Distance(item.transform.position, base.transform.position) <= healDistance && (survivorHuman == null || Vector3.Distance(item.transform.position, base.transform.position) < Vector3.Distance(survivorHuman.transform.position, base.transform.position)))
			{
				SurvivorHuman survivorHuman2 = item;
				if (survivorHuman2.ReadyToHeal() && groupController.pvpPlayerIndex == 0)
				{
					survivorHuman = survivorHuman2;
				}
			}
		}
		return survivorHuman;
	}

	protected override void AddBullet()
	{
		lightning.DoIt(targetZombie, bulletsSpawn.transform);
		PlayShootSound();
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}
}
