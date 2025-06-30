using UnityEngine;

public class PVPBufferSurvivor : PVPSoldierSurvivor
{
	[SerializeField]
	private float laserDelay = 1f;

	private bool readyToRotateForward;

	private Laser laser;

	public override void Start()
	{
		base.Start();
		laser = Object.Instantiate(bulletPrefab, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation).GetComponent<Laser>();
		SetBulletDamage(laser.gameObject);
		laser.pvpIndex = groupController.pvpPlayerIndex;
		laser.gameObject.SetActive(false);
	}

	private new void Update()
	{
		if (!laser.gameObject.activeSelf)
		{
			if (readyToRotateForward)
			{
				Invoke("RotateForward", Mathf.Max(1f, shootDelay + 0.3f));
			}
			readyToRotateForward = false;
			if (reload <= 0f)
			{
				FindTargetShoot();
				if (targetZombie != null)
				{
					targetRotation = targetZombie;
				}
				base.Update();
				if (targetZombie != null)
				{
					CancelInvoke("RotateForward");
					if (isLookAtTarget)
					{
						reload = laserDelay;
						if (heroType == SaveData.HeroData.HeroType.MINER)
						{
							Invoke("AddBullet", 0.3f);
							animator.SetTrigger("Throw");
						}
						else
						{
							AddBullet();
						}
					}
				}
				readyToRotateForward = true;
			}
			else
			{
				reload -= Time.deltaTime;
				base.Update();
			}
		}
		else
		{
			base.Update();
		}
	}

	protected override void AddBullet()
	{
		laser.DoIt(shootDelay, aimDistance, targetZombie, bulletsSpawn.transform);
		SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}

	private void OnDestroy()
	{
		if (laser != null)
		{
			Object.Destroy(laser.gameObject, shootDelay);
		}
	}
}
