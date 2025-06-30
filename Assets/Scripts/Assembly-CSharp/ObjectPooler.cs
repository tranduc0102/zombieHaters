using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	private List<PooledObject<PVPBaseBullet>> pvpBulletStorage = new List<PooledObject<PVPBaseBullet>>();

	private PooledObject<LootObject> lootStorage = new PooledObject<LootObject>();

	private PooledObject<ParticleSystem> pvpZombieEnergy = new PooledObject<ParticleSystem>();

	private PooledObject<ParticleSystem> pooledIdleLoot = new PooledObject<ParticleSystem>();

	private PooledObject<ParticleSystem> pooledPickUpLoot = new PooledObject<ParticleSystem>();

	[SerializeField]
	private ParticleSystem zombieEnergyPrefab;

	[SerializeField]
	private ParticleSystem idleLoot;

	[SerializeField]
	private ParticleSystem pickUpLoot;

	public static ObjectPooler Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		pvpZombieEnergy.prefab = zombieEnergyPrefab;
		pooledIdleLoot.prefab = idleLoot;
		pooledPickUpLoot.prefab = pickUpLoot;
	}

	public int AddBulletprefab(PVPBaseBullet bulletPrefab)
	{
		for (int i = 0; i < pvpBulletStorage.Count; i++)
		{
			if (pvpBulletStorage[i].prefab == bulletPrefab)
			{
				return i;
			}
		}
		pvpBulletStorage.Add(new PooledObject<PVPBaseBullet>
		{
			prefab = bulletPrefab
		});
		return pvpBulletStorage.Count - 1;
	}

	public PVPBaseBullet GetBullet(int index)
	{
		return GetItemFromPool(pvpBulletStorage[index]);
	}

	public void AddLootObject(LootObject lootObjectPrefab)
	{
		lootStorage.prefab = lootObjectPrefab;
	}

	public LootObject GetLootObject()
	{
		return GetItemFromPool(lootStorage);
	}

	private T GetItemFromPool<T>(PooledObject<T> pool) where T : Component
	{
		for (int i = 0; i < pool.list.Count; i++)
		{
			T val = pool.list[i];
			if (!val.gameObject.activeInHierarchy)
			{
				T val2 = pool.list[i];
				val2.gameObject.SetActive(true);
				return pool.list[i];
			}
		}
		pool.list.Add(Object.Instantiate(pool.prefab, base.transform));
		return pool.list.Last();
	}

	public ParticleSystem GetZombieEnergy()
	{
		return GetItemFromPool(pvpZombieEnergy);
	}

	public ParticleSystem GetIdleLoot()
	{
		return GetItemFromPool(pooledIdleLoot);
	}

	public ParticleSystem GetLootPickUp()
	{
		return GetItemFromPool(pooledPickUpLoot);
	}

	public void DisablePvpObjects()
	{
		DisablePooledObjects(lootStorage);
		DisablePooledObjects(pooledIdleLoot);
		DisablePooledObjects(pooledPickUpLoot);
		DisablePooledObjects(pvpZombieEnergy);
	}

	private void DisablePooledObjects<T>(PooledObject<T> pool) where T : Component
	{
		for (int i = 0; i < pool.list.Count; i++)
		{
			T val = pool.list[i];
			val.gameObject.SetActive(false);
		}
	}
}
