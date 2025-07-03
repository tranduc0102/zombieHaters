using System.Collections;
using System.Linq;
using UnityEngine;

public class SurvivorHuman : BaseHuman
{
	public SaveData.HeroData.HeroType heroType;

	private float baseShootDelay;

	[SerializeField]
	protected float shootDelay = 0.3f;

	[HideInInspector]
	public bool isBaffed;

	protected ParticleSystem healingFx;

	private ParticleSystem takeBafFx;

	public float heroDamage;

	[SerializeField]
	protected AudioClip[] shotSounds;

	[SerializeField]
	protected SkinnedMeshRenderer skinnedMeshRenderer;

	[Space]
	[SerializeField]
	protected TrailRenderer overspeedTrail;

	private Coroutine takeDamage;

	protected bool shootExists;

	protected bool isMoving;

	protected float moveForce = 5.15f;

	protected Vector3 previousPosition;

	private int framesStay;

	private Vector3 previousTargetPos;

	private int framesTargetStay;

	public static float moveForceArenaMultiplier = 1f;

	public static float moveForceAbilityMultiplier = 1f;

	protected Transform targetZombie;

	private Test survivorsTarget;

	public Transform GetTargetShoot()
	{
		return targetZombie;
	}

	public virtual void Start()
	{
		animator.SetBool("Rest", false);
		if (healingFx == null)
		{
			healingFx = Object.Instantiate(GameManager.instance.prefabHealingFx, base.transform).GetComponent<ParticleSystem>();
		}
		if (takeDamageFx == null)
		{
			takeDamageFx = Object.Instantiate(GameManager.instance.prefabTakeDamageSurvivor, animator.transform).GetComponent<ParticleSystem>();
		}
		if (takeBafFx == null)
		{
			takeBafFx = Object.Instantiate(GameManager.instance.prefabBafFx, animator.transform).GetComponent<ParticleSystem>();
		}
		baseShootDelay = shootDelay;
		CalculateShootDelay();
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			survivorsTarget = Object.FindObjectOfType<Test>();
			targetRotation = (targetMove = survivorsTarget.transform);
		}
		GameManager.instance.survivors.Add(this);
		heroDamage = CalculateDamage();
		if (GameManager.instance.isTutorialNow)
		{
			heroDamage *= 2.5f;
		}
		shootExists = animator.parameters.Any((AnimatorControllerParameter a) => a.name == "Shoot");
		previousPosition = base.transform.position;
	}

	public virtual float CalculateDamage()
	{
		return DataLoader.Instance.GetHeroDamage(heroType) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus);
	}

	public virtual void SetTarget(Transform target = null)
	{
		if (!target)
		{
			survivorsTarget = Object.FindObjectOfType<Test>();
			targetRotation = (targetMove = survivorsTarget.transform);
		}
		else
		{
			targetRotation = (targetMove = target);
		}
	}

	public void PlayWakeUpFx()
	{
		if (healingFx == null)
		{
			healingFx = Object.Instantiate(GameManager.instance.prefabHealingFx, base.transform).GetComponent<ParticleSystem>();
		}
		healingFx.Play();
		Invoke("EndWakeUpFx", 0.8f);
	}

	private void EndWakeUpFx()
	{
		healingFx.Stop();
	}

	public bool ReadyToHeal()
	{
		if (maxCountHealth <= countHealth)
		{
			return false;
		}
		return true;
	}

	protected void RotateForward()
	{
		targetRotation = targetMove;
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", false);
		}
	}

	public void TakeBaf(float multiplier)
	{
		shootDelay /= multiplier;
		isBaffed = true;
		Invoke("EndBaf", 4f);
		takeBafFx.Play();
	}

	private void EndBaf()
	{
		shootDelay = baseShootDelay;
		isBaffed = false;
		Invoke("HideHpbar", 2f);
		takeBafFx.Stop();
	}

	public override float TakeDamage(float damage, bool isCritical = false)
	{
		if (countHealth <= 0f)
		{
			return countHealth;
		}
		if (GameManager.instance.currentGameMode == GameManager.GameModes.Idle)
		{
			return countHealth;
		}
		if (GameManager.instance.currentGameMode == GameManager.GameModes.Arena && GameManager.instance.GoToFinishLine)
		{
			return countHealth;
		}
		if (GameManager.instance.isTutorialNow && countHealth <= maxCountHealth / 2f)
		{
			return countHealth;
		}
		base.TakeDamage(damage, isCritical);
		if (damage > 0f && takeDamage == null)
		{
			takeDamage = StartCoroutine(takeDamageColorFx());
		}
		if (countHealth <= 0f)
		{
			if ((bool)animator)
			{
				animator.SetTrigger("Death");
			}
			if (GameManager.instance.prefabHeroDeathFx != null)
			{
				Object.Destroy(Object.Instantiate(GameManager.instance.prefabHeroDeathFx, base.transform.position, Quaternion.identity), GameManager.instance.prefabHeroDeathFx.main.duration);
			}
			PlayDeathSound();
			animator.transform.parent = null;
			Object.Destroy(animator.gameObject, 2f);
			Object.Destroy(base.gameObject);
			if (skinnedMeshRenderer.materials.Length > 1)
			{
				skinnedMeshRenderer.materials[1].color = new Color(0f, 0f, 0f, 0f);
			}
			GameManager.instance.DecreaseSurvivor(this);
		}
		if (damage < 0f)
		{
			if (!healingFx.isPlaying)
			{
				healingFx.Play();
			}
			CancelInvoke("StopHealAnim");
			Invoke("StopHealAnim", 1f);
		}
		else
		{
			PlayTakeDamageSound();
		}
		if (healingFx.isPlaying && countHealth >= maxCountHealth)
		{
			healingFx.Stop();
		}
		return countHealth;
	}

	protected virtual void PlayDeathSound()
	{
		if (deathSounds.Length > 0)
		{
			SoundManager.Instance.PlaySound(deathSounds[Random.Range(0, deathSounds.Length)]);
		}
	}

	protected virtual void PlayTakeDamageSound()
	{
		SoundManager.Instance.PlaySurvivorTakeDamage(heroType);
	}

	private void StopHealAnim()
	{
		healingFx.Stop();
	}

	private IEnumerator takeDamageColorFx()
	{
		if (skinnedMeshRenderer.materials.Length <= 1)
		{
			yield break;
		}

		var mat = skinnedMeshRenderer.materials[1];

		while (mat.color.a < 0.5f)
		{
			Color c = mat.color;
			c.a += Time.deltaTime * 2f;
			mat.color = c;
			yield return null;
		}

		while (mat.color.a > 0f)
		{
			Color c = mat.color;
			c.a -= Time.deltaTime * 2f;
			mat.color = c;
			yield return null;
		}

		takeDamage = null;
	}

	protected virtual void FixedUpdate()
	{
		CheckMoving();
		if (targetMove == null)
		{
			return;
		}
		if (!isMoving && !IsTargetMoving())
		{
			rigid.velocity = Vector3.zero;
		}
		else
		{
			rigid.velocity = Vector3.MoveTowards(Vector3.zero, targetMove.transform.position - base.transform.position, 1f) * moveForce * moveForceArenaMultiplier * moveForceAbilityMultiplier;
		}
		overspeedTrail.emitting = moveForceAbilityMultiplier > 1f;
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

	protected void CheckMoving()
	{
		if (Vector3.Distance(previousPosition, base.transform.position) < 0.1f)
		{
			if (framesStay < 3)
			{
				framesStay++;
			}
			if (framesStay >= 3)
			{
				isMoving = false;
				SetStepsSound();
			}
		}
		else
		{
			if (framesStay > 0)
			{
				framesStay--;
			}
			if (framesStay <= 0)
			{
				isMoving = true;
				SetStepsSound();
			}
		}
		previousPosition = base.transform.position;
	}

	protected virtual void SetStepsSound()
	{
		SoundManager.Instance.PlayStepsSound(isMoving);
	}

	public void CalculateShootDelay()
	{
		shootDelay = baseShootDelay;
		shootDelay /= PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus + 1f;
	}

	protected bool IsTargetMoving()
	{
		if (survivorsTarget != null && survivorsTarget.joystickDirection != Vector3.zero)
		{
			return true;
		}
		if (Vector3.Distance(previousTargetPos, targetMove.position) < 0.1f)
		{
			if (framesTargetStay < 3)
			{
				framesTargetStay++;
			}
		}
		else if (framesTargetStay > 0)
		{
			framesTargetStay--;
		}
		previousTargetPos = targetMove.position;
		if (framesTargetStay >= 3)
		{
			return false;
		}
		return true;
	}
}
