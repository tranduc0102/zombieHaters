using UnityEngine;
using UnityEngine.AI;

public class PVPSoldierSurvivor : SoldierSurvivor
{
	[SerializeField]
	private NavMeshAgent agent;

	[HideInInspector]
	public bool agentMoving;

	[HideInInspector]
	public BasePVPGroupController groupController;

	[SerializeField]
	private float damageIncPercentage;

	protected int bulletStorageIndex = -1;

	public override void Start()
	{
		base.Start();
		agent.updateRotation = false;
		SetHealth();
		findShootMask = (1 << LayerMask.NameToLayer("Zombie")) | (1 << LayerMask.NameToLayer("Survivor")) | (1 << LayerMask.NameToLayer("LootBox"));
		if (heroType != SaveData.HeroData.HeroType.MEDIC && heroType != SaveData.HeroData.HeroType.COOK)
		{
			bulletStorageIndex = ObjectPooler.Instance.AddBulletprefab(bulletPrefab.GetComponent<PVPBaseBullet>());
		}
	}

	public override float CalculateDamage()
	{
		return base.CalculateDamage() * (damageIncPercentage + 1f);
	}

	protected override void FixedUpdate()
	{
		CheckMoving();
		if (targetMove == null)
		{
			return;
		}
		if (!agentMoving)
		{
			if (!isMoving && !IsTargetMoving())
			{
				rigid.velocity = Vector3.zero;
			}
			else
			{
				rigid.velocity = Vector3.MoveTowards(Vector3.zero, targetMove.transform.position - base.transform.position, 1f) * moveForce * ((groupController.pvpPlayerIndex != 0) ? 1f : SurvivorHuman.moveForceAbilityMultiplier);
			}
		}
		if (groupController.pvpPlayerIndex == 0)
		{
			overspeedTrail.emitting = SurvivorHuman.moveForceAbilityMultiplier > 1f;
		}
		if ((bool)animator)
		{
			if (!isMoving)
			{
				animator.SetBool("Run", false);
			}
			else
			{
				animator.SetBool("Run", true);
			}
		}
	}

	public void SetAgentDestination(Vector3 position)
	{
		agent.enabled = true;
		agentMoving = true;
		agent.SetDestination(position);
	}

	public void ResetAgent()
	{
		if (agentMoving)
		{
			agentMoving = false;
			agent.ResetPath();
			agent.enabled = false;
		}
	}

	protected override void DecreaseHp(float damage)
	{
		if (damage > 0f)
		{
			base.DecreaseHp(damage);
			return;
		}
		countHealth -= maxCountHealth * (damage / 100f);
		if (countHealth > maxCountHealth)
		{
			countHealth = maxCountHealth;
		}
	}

	protected override void FindTargetShoot()
	{
		if (targetZombie != null && Time.time - lastFindTime < refindTargetTime && Vector3.Distance(base.transform.position, targetZombie.position) < aimDistance)
		{
			return;
		}
		targetZombie = null;
		Collider[] array = Physics.OverlapSphere(base.transform.position, aimDistance, findShootMask.value);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Vector3 direction = new Vector3(collider.transform.position.x - body.position.x, 1f, collider.transform.position.z - body.position.z);
			RaycastHit[] array3 = Physics.RaycastAll(body.position, direction, aimDistance, maskAimSurvivors);
			if (array3.Length <= 0)
			{
				continue;
			}
			for (int j = 0; j < array3.Length; j++)
			{
				SurvivorHuman survivorHuman = IsInMyteam(array3[j].transform.gameObject);
				if ((bool)survivorHuman)
				{
					Transform targetShoot = survivorHuman.GetTargetShoot();
					if (targetShoot != null && Vector3.Distance(base.transform.position, targetShoot.position) < aimDistance)
					{
						targetZombie = targetShoot;
						break;
					}
					continue;
				}
				CheckHitTarget(array3[j]);
				break;
			}
		}
		lastFindTime = Time.time;
	}

	protected override bool CheckHitTarget(RaycastHit hitInfo)
	{
		if ((hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieBoss" || (hitInfo.transform.tag == "Survivor" && !IsInMyteam(hitInfo.transform.gameObject))) && (targetZombie == null || Vector3.Distance(base.transform.position, hitInfo.transform.position) < Vector3.Distance(base.transform.position, targetZombie.position)))
		{
			targetZombie = hitInfo.transform;
			targetRotation = targetZombie;
			return true;
		}
		return false;
	}

	protected SurvivorHuman IsInMyteam(GameObject other)
	{
		return PVPManager.Instance.IsInMyTeam(other.gameObject, groupController.pvpPlayerIndex);
	}

	public void SetHealth()
	{
		countHealth = DataLoader.Instance.GetHeroHP(heroType);
		maxCountHealth = countHealth;
	}

	protected override void AddBullet()
	{
		if (bulletStorageIndex == -1)
		{
			SetBulletInfo(Object.Instantiate(bulletPrefab, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation));
		}
		else
		{
			ObjectPooler.Instance.GetBullet(bulletStorageIndex).SetInfo(heroDamage, groupController.pvpPlayerIndex, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation);
		}
		bulletsSpawn.Play();
		PlayShootSound();
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}

	public void SetBulletInfo(GameObject bullet)
	{
		PVPBaseBullet component;
		if ((component = bullet.GetComponent<PVPBaseBullet>()) != null)
		{
			component.SetInfo(heroDamage, groupController.pvpPlayerIndex, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation);
		}
	}

	protected override void SetStepsSound()
	{
	}

	protected override void PlayDeathSound()
	{
		if (groupController.pvpPlayerIndex == 0)
		{
			base.PlayDeathSound();
		}
		else if (PVPManager.pvpPlayers[0].IsAlive() && Vector3.Distance(groupController.GetGroupCenter(), PVPManager.pvpPlayers[0].GetGroupCenter()) < 15f && deathSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(deathSounds[Random.Range(0, deathSounds.Length)], base.transform.position);
		}
		groupController.RemoveSurvivor(this);
	}

	protected override void PlayTakeDamageSound()
	{
		if (groupController.pvpPlayerIndex == 0)
		{
			base.PlayTakeDamageSound();
		}
		else if (PVPManager.pvpPlayers[0].IsAlive() && Vector3.Distance(groupController.GetGroupCenter(), PVPManager.pvpPlayers[0].GetGroupCenter()) < 15f)
		{
			SoundManager.Instance.PlaySurvivorTakeDamageAtPoint(heroType, base.transform.position);
		}
	}

	protected void PlayShootSound()
	{
		if (groupController.pvpPlayerIndex == 0)
		{
			SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		}
		else if (PVPManager.pvpPlayers[0].IsAlive() && Vector3.Distance(groupController.GetGroupCenter(), PVPManager.pvpPlayers[0].GetGroupCenter()) < 15f && shotSounds.Length > 0)
		{
			AudioSource.PlayClipAtPoint(shotSounds[Random.Range(0, shotSounds.Length)], base.transform.position);
		}
	}
}
