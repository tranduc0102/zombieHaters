using UnityEngine;

public class SoldierSurvivor : SurvivorHuman
{
	[SerializeField]
	protected ParticleSystem bulletsSpawn;

	[SerializeField]
	protected GameObject bulletPrefab;

	[SerializeField]
	protected float aimDistance = 10f;

	[SerializeField]
	protected LayerMask maskAimSurvivors;

	protected float reload;

	protected float refindTargetTime = 1.5f;

	protected float lastFindTime;

	protected LayerMask findShootMask;

	public override void Start()
	{
		base.Start();
		findShootMask = 1 << LayerMask.NameToLayer("Zombie");
		refindTargetTime += Random.Range(-0.2f, 0.2f);
	}

	private new void Update()
	{
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
					reload = shootDelay;
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
			Invoke("RotateForward", Mathf.Max(1f, shootDelay + 0.3f));
		}
		else
		{
			reload -= Time.deltaTime;
			base.Update();
		}
	}

	protected virtual void AddBullet()
	{
		SetBulletDamage(Object.Instantiate(bulletPrefab, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation));
		bulletsSpawn.Play();
		SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}

	protected virtual void FindTargetShoot()
	{
		Vector3 direction;
		RaycastHit hitInfo;
		if (Time.time - lastFindTime < refindTargetTime)
		{
			if (targetZombie != null && Vector3.Distance(base.transform.position, targetZombie.position) < aimDistance)
			{
				direction = new Vector3(targetZombie.position.x - body.position.x, 1f, targetZombie.position.z - body.position.z);
				if (Physics.Raycast(body.position, direction, out hitInfo, aimDistance, maskAimSurvivors) && (hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieBoss"))
				{
					return;
				}
			}
			targetZombie = null;
			foreach (SurvivorHuman survivor in GameManager.instance.survivors)
			{
				if (!(survivor == this))
				{
					Transform targetShoot = survivor.GetTargetShoot();
					if (targetShoot != null && (targetZombie == null || (targetZombie != targetShoot && Vector3.Distance(base.transform.position, targetShoot.position) < Vector3.Distance(base.transform.position, targetZombie.position))) && Vector3.Distance(base.transform.position, targetShoot.position) < aimDistance)
					{
						targetZombie = targetShoot;
					}
				}
			}
			if (targetZombie != null)
			{
				return;
			}
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, aimDistance, findShootMask.value);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			direction = new Vector3(collider.transform.position.x - body.position.x, 1f, collider.transform.position.z - body.position.z);
			if (Physics.Raycast(body.position, direction, out hitInfo, aimDistance, maskAimSurvivors))
			{
				CheckHitTarget(hitInfo);
			}
		}
		lastFindTime = Time.time;
	}

	protected virtual bool CheckHitTarget(RaycastHit hitInfo)
	{
		if ((hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieBoss") && (targetZombie == null || Vector3.Distance(base.transform.position, hitInfo.transform.position) < Vector3.Distance(base.transform.position, targetZombie.position)))
		{
			targetZombie = hitInfo.transform;
			return true;
		}
		return false;
	}

	public void SetBulletDamage(GameObject bullet)
	{
		float criticalHitBonus = PassiveAbilitiesManager.bonusHelper.CriticalHitBonus;
		BaseBullet component;
		Mine component2;
		if ((component = bullet.GetComponent<BaseBullet>()) != null)
		{
			component.damage = heroDamage * criticalHitBonus;
			component.isCriticalDamage = criticalHitBonus > 1f;
		}
		else if ((component2 = bullet.GetComponent<Mine>()) != null)
		{
			component2.damage = heroDamage * criticalHitBonus;
			component2.isCriticalDamage = criticalHitBonus > 1f;
		}
		else
		{
			ShotGunBullet component3 = bullet.GetComponent<ShotGunBullet>();
			component3.damage = heroDamage * criticalHitBonus;
			component3.isCriticalDamage = criticalHitBonus > 1f;
		}
	}
}
