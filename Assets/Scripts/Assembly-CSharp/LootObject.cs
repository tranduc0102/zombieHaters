using System.Collections;
using UnityEngine;

public class LootObject : ZombiesSpawn
{
	[SerializeField]
	private ZombieHuman prefabZombie;

	[SerializeField]
	private Collider myCollider;

	[SerializeField]
	private Rigidbody rigid;

	[SerializeField]
	private GameObject pizzaObject;

	[SerializeField]
	private ParticleSystem pickUpFx;

	[Header("SpawnSettings")]
	[SerializeField]
	private float zombiesSpawnsPowerMultiplier = 0.5f;

	[SerializeField]
	private float zombieSpawnChance = 0.5f;

	[SerializeField]
	private float respawnTime = 7f;

	[HideInInspector]
	public bool isPooled;

	private bool lootAdded;

	private bool lastWereZombie;

	private bool isRefreshedNearPlayer;

	private ParticleSystem idleFx;

	private float distanceToPlayer;

	private void Start()
	{
		base.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
		Invoke("EnableObject", Random.Range(0f, 1f));
	}

	private void OnEnable()
	{
		lootAdded = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.transform.tag == "Survivor") || lootAdded)
		{
			return;
		}
		if (PVPManager.Instance.AddLoot(collision.gameObject))
		{
			lootAdded = true;
			if (distanceToPlayer < 30f)
			{
				pickUpFx = ObjectPooler.Instance.GetLootPickUp();
				pickUpFx.transform.position = base.transform.position;
				pickUpFx.Play();
			}
			base.gameObject.SetActive(false);
		}
		if (!isPooled)
		{
			Invoke("EnableObject", respawnTime);
		}
		else
		{
			CancelInvoke();
		}
	}

	public void MoveToPosition(Vector3 fromPosotion, Vector3 toPosition, float timeToJump)
	{
		base.transform.position = fromPosotion;
		rigid.velocity = Vector3.zero;
		Vector3 force = (fromPosotion - toPosition) * 85f;
		rigid.AddForce(force);
		StartCoroutine(PsFollowObject(toPosition));
	}

	private IEnumerator PsFollowObject(Vector3 position)
	{
		while (base.transform.position != position)
		{
			yield return new WaitForEndOfFrame();
			if (idleFx != null)
			{
				idleFx.transform.position = base.transform.position;
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Update()
	{
		distanceToPlayer = Vector3.Distance(base.transform.position, CameraTarget.instance.transform.position);
		if (!isPooled && !isRefreshedNearPlayer && distanceToPlayer < 30f)
		{
			EnableObject();
			isRefreshedNearPlayer = true;
		}
	}

	private void EnableObject()
	{
		isRefreshedNearPlayer = false;
		if (lastWereZombie)
		{
			LayerMask layerMask = 1 << LayerMask.NameToLayer("Zombie");
			Collider[] array = Physics.OverlapSphere(base.transform.position, 2f, layerMask.value);
			if (array.Length > 0)
			{
				Invoke("EnableObject", respawnTime / 2f);
				return;
			}
		}
		base.gameObject.SetActive(true);
		if (!isPooled && distanceToPlayer > 25f && distanceToPlayer < 30f && Random.Range(0f, 1f) < zombieSpawnChance)
		{
			Spawn(PVPManager.Instance.zombiesGroupPower, true);
			lastWereZombie = true;
			Invoke("EnableObject", respawnTime);
			isRefreshedNearPlayer = true;
		}
		else
		{
			lootAdded = false;
			lastWereZombie = false;
		}
		myCollider.enabled = !lastWereZombie;
		rigid.isKinematic = lastWereZombie;
	}

	private void OnBecameVisible()
	{
		if (!lastWereZombie)
		{
			if (idleFx == null)
			{
				idleFx = ObjectPooler.Instance.GetIdleLoot();
			}
			Transform obj = idleFx.transform;
			Vector3 position = base.transform.position;
			base.transform.position = position;
			obj.position = position;
			idleFx.Play();
		}
	}

	private void OnBecameInvisible()
	{
		if (idleFx != null)
		{
			idleFx.gameObject.SetActive(false);
			idleFx = null;
		}
	}

	private void OnDestroy()
	{
		if (pickUpFx != null)
		{
			pickUpFx.gameObject.SetActive(false);
			pickUpFx = null;
		}
	}
}
