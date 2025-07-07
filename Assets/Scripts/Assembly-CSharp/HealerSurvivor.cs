using UnityEngine;

public class HealerSurvivor : SoldierSurvivor
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

	private Lightning lightning;

	private SurvivorHuman targetSurvivor;

	public override void Start()
	{
		base.Start();
		Invoke("DoIt", healDelay);
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
			SoundManager.Instance.PlayHealSound();
			healFx.Play();
		}
		Invoke("DoIt", healDelay);
	}

	private SurvivorHuman FindTargetSurvivor()
	{
		SurvivorHuman survivorHuman = null;
		foreach (SurvivorHuman survivor in GameManager.instance.survivors)
		{
			if (survivor == null) continue;
			if (survivor != this && Vector3.Distance(survivor.transform.position, base.transform.position) <= healDistance && (survivorHuman == null || Vector3.Distance(survivor.transform.position, base.transform.position) < Vector3.Distance(survivorHuman.transform.position, base.transform.position)))
			{
				SurvivorHuman survivorHuman2 = survivor;
				if (survivorHuman2.ReadyToHeal())
				{
					survivorHuman = survivorHuman2;
				}
			}
		}
		return survivorHuman;
	}

	protected override void AddBullet()
	{
		if (lightning == null)
		{
			lightning = Object.Instantiate(bulletPrefab, Vector3.zero, default(Quaternion)).GetComponent<Lightning>();
			SetBulletDamage(lightning.gameObject);
		}
		lightning.DoIt(targetZombie, bulletsSpawn.transform);
		SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}

	private void OnDestroy()
	{
		if (lightning != null)
		{
			Object.Destroy(lightning.gameObject, shootDelay);
		}
	}
}
