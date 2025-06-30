using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHuman : BaseHuman
{
	public enum ZombieSize
	{
		Small = 0,
		Middle = 1,
		Big = 2
	}

	[HideInInspector]
	public bool isActive = true;

	[SerializeField]
	private float damageDelay = 0.5f;

	[SerializeField]
	public ZombieSize size;

	protected NavMeshAgent navAgent;

	private bool canDamage = true;

	private float refindTargetTime = 1.5f;

	private float lastFindTime;

	[HideInInspector]
	public SaveData.ZombieData.ZombieType zombieType;

	[HideInInspector]
	public int damage;

	public float power;

	public float defaultSpeed;

	[SerializeField]
	private AudioClip attackSound;

	[SerializeField]
	private AudioClip[] roarSounds;

	private CameraTarget cameraTarget;

	protected virtual void Start()
	{
		cameraTarget = Object.FindObjectOfType<CameraTarget>();
		navAgent = GetComponent<NavMeshAgent>();
		CreateTakeDamageFx();
		if (zombieType == SaveData.ZombieData.ZombieType.FLAG || zombieType == SaveData.ZombieData.ZombieType.CLOWN)
		{
			Object.Instantiate(GameManager.instance.prefabRareGlowFx, base.transform);
		}
		if (GameManager.instance.currentGameMode == GameManager.GameModes.Idle)
		{
			defaultSpeed = navAgent.speed / 1.5f;
		}
		else
		{
			defaultSpeed = navAgent.speed;
		}
		GameManager.instance.zombies.Add(this);
		if (!isActive)
		{
			navAgent.isStopped = true;
			animator.enabled = false;
		}
		else
		{
			StartCoroutine(Roar());
		}
	}

	protected virtual void CreateTakeDamageFx()
	{
		if (takeDamageFx == null)
		{
			takeDamageFx = Object.Instantiate(GameManager.instance.prefabTakeDamageZombie, animator.transform).GetComponent<ParticleSystem>();
		}
	}

	private IEnumerator Roar()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(2, 7));
			if (roarSounds.Length > 0 && !SoundManager.Instance.soundIsMuted && (targetMove == null || Vector3.Distance(base.transform.position, targetMove.position) < 20f) && Random.value < 0.5f)
			{
				AudioSource.PlayClipAtPoint(roarSounds[Random.Range(0, roarSounds.Length)], base.transform.position, SoundManager.Instance.soundVolume);
			}
		}
	}

	private void ActivateMe()
	{
		if (!isActive)
		{
			isActive = true;
			StartCoroutine(Roar());
			animator.enabled = true;
		}
	}

	protected virtual void FixedUpdate()
	{
		if (!isActive && cameraTarget.enabled && Vector3.Distance(base.transform.position, cameraTarget.transform.position) < 20f)
		{
			ActivateMe();
		}
		if (!isActive)
		{
			return;
		}
		if (targetMove == null || Time.time - lastFindTime > refindTargetTime)
		{
			foreach (SurvivorHuman survivor in GameManager.instance.survivors)
			{
				if (!(survivor == null) && (targetMove == null || Vector3.Distance(base.transform.position, targetMove.position) > Vector3.Distance(base.transform.position, survivor.transform.position)))
				{
					targetMove = survivor.transform;
				}
			}
			lastFindTime = Time.time;
		}
		if (targetMove != null)
		{
			navAgent.isStopped = false;
			float num = Vector3.Distance(base.transform.position, targetMove.position);
			if (num > 20f)
			{
				navAgent.speed = defaultSpeed * 5f;
			}
			else
			{
				navAgent.speed = defaultSpeed;
			}
			if (canDamage && num < 3f + base.transform.localScale.x / 2f)
			{
				animator.SetTrigger("Attack");
				canDamage = false;
				Invoke("TryDamageSurvivor", 0.45f);
			}
			navAgent.destination = targetMove.position;
		}
		else
		{
			navAgent.isStopped = true;
		}
		if ((bool)animator)
		{
			if (targetMove != null)
			{
				animator.SetBool("Run", true);
			}
			else
			{
				animator.SetBool("Run", false);
			}
		}
	}

	private void TryDamageSurvivor()
	{
		if (targetMove != null)
		{
			float num = Vector3.Distance(base.transform.position, targetMove.position);
			if (num < 2f + base.transform.localScale.x / 2f)
			{
				SoundManager.Instance.PlaySound(attackSound);
				targetMove.GetComponent<BaseHuman>().TakeDamage(damage);
				Invoke("ReadyToDamage", damageDelay);
			}
			else
			{
				canDamage = true;
			}
		}
		else
		{
			canDamage = true;
		}
	}

	private void ReadyToDamage()
	{
		canDamage = true;
	}

	public override float TakeDamage(float damage, bool isCritical = false)
	{
		if (countHealth <= 0f)
		{
			return countHealth;
		}
		base.TakeDamage(damage, isCritical);
		if (countHealth <= 0f)
		{
			GameManager.instance.DecreaseZombie(this);
			if (isActive && Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position) < 30f)
			{
				if ((bool)animator)
				{
					animator.SetTrigger("Death_" + Random.Range(1, 4));
				}
				if (deathSounds.Length > 0)
				{
					SoundManager.Instance.PlaySound(deathSounds[Random.Range(0, deathSounds.Length)], 0.25f);
				}
				if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP && Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position) < 30f)
				{
					SkinnedMeshRenderer componentInChildren = GetComponentInChildren<SkinnedMeshRenderer>();
					if (componentInChildren != null && componentInChildren.isVisible)
					{
						ParticleSystem zombieEnergy = ObjectPooler.Instance.GetZombieEnergy();
						zombieEnergy.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
						zombieEnergy.Play();
					}
				}
				animator.transform.parent = null;
				Object.Destroy(animator.gameObject, Random.Range(2f, 3f));
				GameObject obj = Object.Instantiate(GameManager.instance.prefabZombieBlood, new Vector3(base.transform.position.x, GameManager.instance.prefabZombieBlood.transform.position.y, base.transform.position.z), GameManager.instance.prefabZombieBlood.transform.rotation, TransformParentManager.Instance.fx);
				Object.Destroy(obj, 3.7f);
			}
			Object.Destroy(base.gameObject);
			if (GameManager.instance.currentGameMode == GameManager.GameModes.Idle)
			{
				DataLoader.Instance.RefreshIdleZombieGold(power * StaticConstants.IdleGoldConst * 10f * (1f + PassiveAbilitiesManager.bonusHelper.GoldBonus));
			}
		}
		return countHealth;
	}
}
