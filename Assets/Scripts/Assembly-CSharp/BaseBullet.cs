using UnityEngine;

public class BaseBullet : MonoBehaviour
{
	[SerializeField]
	protected Rigidbody rigid;

	[SerializeField]
	protected float speed = 20f;

	public float damage = 10f;

	[SerializeField]
	protected GameObject bulletObject;

	[SerializeField]
	protected GameObject hitFx;

	[HideInInspector]
	public bool isCriticalDamage;

	protected virtual void Start()
	{
		rigid = GetComponent<Rigidbody>();
		rigid.velocity = new Vector3(base.transform.right.x, 0f, base.transform.right.z) * speed;
		Object.Destroy(base.gameObject, 3f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(rigid.velocity == Vector3.zero))
		{
			rigid.velocity = Vector3.zero;
			base.transform.position = collision.contacts[0].point;
			if (collision.gameObject.tag == "Zombie" || collision.gameObject.tag == "ZombieBoss")
			{
				collision.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage);
			}
			bulletObject.SetActive(false);
			if (Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position) < 25f)
			{
				hitFx.SetActive(true);
			}
			Object.Destroy(base.gameObject, 1f);
		}
	}
}
