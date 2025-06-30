using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour
{
	public float damage = 20f;

	[HideInInspector]
	public int pvpIndex;

	[SerializeField]
	private int bangSize = 4;

	[SerializeField]
	protected float bangTimeout = 2f;

	[SerializeField]
	private GameObject mineObject;

	[SerializeField]
	private GameObject bangFx;

	[SerializeField]
	private AudioClip explosionSound;

	[HideInInspector]
	public bool isCriticalDamage;

	protected Rigidbody rigid;

	private bool bangIs;

	private void Awake()
	{
		rigid = GetComponent<Rigidbody>();
		Invoke("BangNow", bangTimeout);
	}

	private void Start()
	{
		rigid.AddForce(new Vector3(base.transform.right.x, 0f, base.transform.right.z) * 70f);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			return;
		}
		if (other.gameObject.tag == "Zombie" || other.gameObject.tag == "ZombieBoss")
		{
			if (other.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage) < 0f)
			{
				PVPManager.Instance.AddZombieLoot(pvpIndex);
			}
			rigid.velocity = Vector3.zero;
		}
		if (other.gameObject.tag == "Survivor")
		{
			if (!PVPManager.Instance.IsInMyTeam(other.gameObject, pvpIndex))
			{
				other.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage);
				rigid.velocity = Vector3.zero;
				StartCoroutine(Bang());
			}
		}
		else if (!bangIs)
		{
			StartCoroutine(Bang());
		}
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP)
		{
			rigid.velocity = Vector3.zero;
			if (collision.gameObject.tag == "Zombie" || collision.gameObject.tag == "ZombieBoss")
			{
				collision.gameObject.GetComponent<BaseHuman>().TakeDamage(damage, isCriticalDamage);
			}
			if (!bangIs && collision.gameObject.tag != "Ground")
			{
				StartCoroutine(Bang());
			}
		}
	}

	private void BangNow()
	{
		StartCoroutine(Bang());
	}

	private IEnumerator Bang()
	{
		CancelInvoke("BangNow");
		bangIs = true;
		mineObject.SetActive(false);
		bangFx.SetActive(true);
		SphereCollider sphereCollider = GetComponent<SphereCollider>();
		for (int i = 0; i < bangSize; i++)
		{
			sphereCollider.radius += 1f;
			yield return null;
		}
		sphereCollider.enabled = false;
		SoundManager.Instance.PlaySound(explosionSound);
		Object.Destroy(base.gameObject, 1f);
	}
}
