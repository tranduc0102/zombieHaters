using UnityEngine;

public class ShadowBullet : BaseBullet
{
	[SerializeField]
	private float lifeTime = 2f;

	protected override void Start()
	{
		base.Start();
		Invoke("DestroyBullet", lifeTime);
	}

	private void DestroyBullet()
	{
		bulletObject.SetActive(false);
		if (Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position) < 25f)
		{
			hitFx.SetActive(true);
		}
		Object.Destroy(base.gameObject, 1f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Zombie" || other.gameObject.tag == "ZombieBoss")
		{
			other.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage);
			Object.Destroy(Object.Instantiate(hitFx, base.transform.position, hitFx.transform.rotation), 1f);
		}
	}
}
