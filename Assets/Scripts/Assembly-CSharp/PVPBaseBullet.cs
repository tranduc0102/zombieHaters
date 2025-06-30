using UnityEngine;

public class PVPBaseBullet : BaseBullet
{
	[SerializeField]
	private TrailRenderer trailRenderer;

	[SerializeField]
	protected float lifetime = 1.5f;

	private ParticleSystem psHitFx;

	protected int pvpIndex;

	private bool disabled;

	protected override void Start()
	{
		if ((bool)hitFx)
		{
			psHitFx = hitFx.GetComponent<ParticleSystem>();
		}
	}

	protected virtual void OnEnable()
	{
		disabled = false;
		if ((bool)trailRenderer)
		{
			trailRenderer.Clear();
		}
		if ((bool)bulletObject)
		{
			bulletObject.SetActive(true);
		}
		if ((bool)hitFx)
		{
			hitFx.SetActive(false);
		}
		if ((bool)rigid)
		{
			rigid.velocity = Vector3.zero;
		}
		Invoke("DisableObject", lifetime);
	}

	protected virtual void OnDisable()
	{
		CancelInvoke();
	}

	protected virtual void DisableBullet()
	{
		if (disabled)
		{
			return;
		}
		disabled = true;
		rigid.velocity = Vector3.zero;
		if ((bool)bulletObject)
		{
			bulletObject.SetActive(false);
		}
		if (Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position) < 25f)
		{
			if ((bool)hitFx)
			{
				hitFx.SetActive(true);
			}
			if ((bool)psHitFx)
			{
				psHitFx.Play();
			}
			Invoke("DisableObject", psHitFx.main.duration);
		}
		else
		{
			DisableObject();
		}
	}

	protected virtual void DisableObject()
	{
		if ((bool)psHitFx)
		{
			psHitFx.Stop();
		}
		base.gameObject.SetActive(false);
	}

	public virtual void SetInfo(float damage, int pvpPlayerIndex, Vector3 position, Quaternion rotation)
	{
		base.damage = damage * PassiveAbilitiesManager.bonusHelper.CriticalHitBonus;
		pvpIndex = pvpPlayerIndex;
		base.transform.SetPositionAndRotation(position, rotation);
		SetVelocity();
	}

	public virtual void SetVelocity()
	{
		rigid.velocity = new Vector3(base.transform.right.x, 0f, base.transform.right.z) * speed;
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (rigid.velocity == Vector3.zero)
		{
			return;
		}
		if (other.gameObject.tag == "Survivor")
		{
			if (!PVPManager.Instance.IsInMyTeam(other.gameObject, pvpIndex))
			{
				other.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage);
				rigid.velocity = Vector3.zero;
				DisableBullet();
			}
		}
		else
		{
			if ((other.gameObject.tag == "Zombie" || other.gameObject.tag == "ZombieBoss") && other.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage) <= 0f)
			{
				PVPManager.Instance.AddZombieLoot(pvpIndex);
			}
			DisableBullet();
		}
	}
}
