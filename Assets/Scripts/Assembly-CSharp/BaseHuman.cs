using UnityEngine;

public class BaseHuman : MonoBehaviour
{
	protected Rigidbody rigid;

	protected Transform targetMove;

	protected Transform targetRotation;

	[SerializeField]
	public Transform body;

	[SerializeField]
	public Animator animator;

	[SerializeField]
	protected Transform healthBar;

	public float countHealth = 100f;

	[HideInInspector]
	public float maxCountHealth;

	protected bool isLookAtTarget;

	protected ParticleSystem takeDamageFx;

	protected Animation CritFx;

	[SerializeField]
	protected AudioClip[] deathSounds;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		maxCountHealth = countHealth;
		HideHpbar();
		CritFx = Object.Instantiate(GameManager.instance.prefabCritFx, animator.transform.position, default(Quaternion));
		CritFx.transform.parent = animator.transform;
	}

	protected virtual void Update()
	{
		CalculateBodyRotation();
	}

	public virtual float TakeDamage(float damage, bool isCritical = false)
	{
		if (countHealth <= 0f)
		{
			return countHealth;
		}
		DecreaseHp(damage);
		if (countHealth > maxCountHealth)
		{
			countHealth = maxCountHealth;
		}
		if (isCritical && !CritFx.isPlaying)
		{
			CritFx.Play();
		}
		if (healthBar != null)
		{
			CancelInvoke("HideHpbar");
			healthBar.gameObject.SetActive(true);
			Invoke("HideHpbar", 2f);
			healthBar.localScale = new Vector3(countHealth / maxCountHealth, healthBar.localScale.y, healthBar.localScale.z);
		}
		if (damage > 0f && takeDamageFx != null && !takeDamageFx.isPlaying && GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			takeDamageFx.Play();
		}
		return countHealth;
	}

	protected virtual void DecreaseHp(float damage)
	{
		countHealth -= damage;
	}

	protected void HideHpbar()
	{
		if (healthBar != null)
		{
			healthBar.gameObject.SetActive(false);
		}
	}

	public virtual void CalculateBodyRotation()
	{
		isLookAtTarget = false;
		if (!(targetRotation == null))
		{
			Vector3 vector = targetRotation.position - base.transform.position;
			if (rigid.velocity == Vector3.zero && targetRotation == targetMove)
			{
				vector = -vector;
			}
			float num = Vector3.Angle(vector, base.transform.forward);
			if (Vector3.Angle(vector, base.transform.right) > 90f)
			{
				num *= -1f;
			}
			body.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(body.transform.eulerAngles.y, num, 0.5f), 0f);
			if (Mathf.Abs(Mathf.DeltaAngle(body.transform.eulerAngles.y, num)) < 2f)
			{
				isLookAtTarget = true;
			}
		}
	}
}
