using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class Lightning : BaseBullet
{
	[SerializeField]
	private float transferDistance = 2.5f;

	[SerializeField]
	private float boltLifetime = 0.1f;

	[SerializeField]
	private float yPosition = 1f;

	[SerializeField]
	private LightningBoltScript[] lightningBolts;

	[HideInInspector]
	public int pvpIndex;

	private new void Start()
	{
	}

	public void DoIt(Transform firstTarget, Transform bulletSpawn)
	{
		StartCoroutine(CreateLightningBolts(bulletSpawn, firstTarget));
	}

	private IEnumerator CreateLightningBolts(Transform currentStartTransform, Transform currentEndTransform)
	{
		List<Transform> alreadyDamaged = new List<Transform>();
		for (int i = 0; i < lightningBolts.Length; i++)
		{
			if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
			{
				BaseHuman component = currentEndTransform.GetComponent<BaseHuman>();
				if (component != null)
				{
					if (component.tag == "Zombie" || component.tag == "ZombieBoss")
					{
						if (component.TakeDamage(damage, isCriticalDamage) < 0f)
						{
							PVPManager.Instance.AddZombieLoot(pvpIndex);
						}
					}
					else
					{
						component.TakeDamage(damage, isCriticalDamage);
					}
				}
				else
				{
					PVPLootBox component2 = currentEndTransform.GetComponent<PVPLootBox>();
					if (component2 != null)
					{
						component2.TakeDamage();
					}
				}
			}
			else
			{
				ZombieHuman component3 = currentEndTransform.GetComponent<ZombieHuman>();
				if (component3 == null)
				{
					Debug.LogError("Can't get ZombieHuman from object named - " + component3.name);
				}
				else
				{
					component3.TakeDamage(damage, isCriticalDamage);
				}
			}
			alreadyDamaged.Add(currentEndTransform);
			lightningBolts[i].gameObject.SetActive(true);
			for (float timer = boltLifetime; timer > 0f; timer -= Time.deltaTime)
			{
				if (currentStartTransform != null)
				{
					if (i == 0)
					{
						lightningBolts[i].StartObject.transform.position = currentStartTransform.position;
					}
					else
					{
						lightningBolts[i].StartObject.transform.position = new Vector3(currentStartTransform.position.x, yPosition, currentStartTransform.position.z);
					}
				}
				if (currentEndTransform != null)
				{
					lightningBolts[i].EndObject.transform.position = new Vector3(currentEndTransform.position.x, yPosition, currentEndTransform.position.z);
				}
				yield return null;
			}
			if (i + 1 < lightningBolts.Length)
			{
				if (currentEndTransform != null)
				{
					currentStartTransform = currentEndTransform;
				}
				else
				{
					currentStartTransform = lightningBolts[i + 1].StartObject.transform;
					currentStartTransform.position = lightningBolts[i].EndObject.transform.position;
				}
				LayerMask layerMask = 1 << LayerMask.NameToLayer("Zombie");
				if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
				{
					layerMask = (int)layerMask | (1 << LayerMask.NameToLayer("Survivor"));
				}
				Collider[] array = Physics.OverlapSphere(currentStartTransform.position, transferDistance, layerMask.value);
				List<Transform> list = new List<Transform>();
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					if (alreadyDamaged.Contains(collider.transform))
					{
						continue;
					}
					if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
					{
						if (!PVPManager.Instance.IsInMyTeam(collider.gameObject, pvpIndex))
						{
							list.Add(collider.transform);
						}
					}
					else
					{
						list.Add(collider.transform);
					}
				}
				if (list.Count > 0)
				{
					currentEndTransform = list[Random.Range(0, list.Count)];
				}
				if (list.Count <= 0 || currentStartTransform == currentEndTransform)
				{
					lightningBolts[i].StartObject.transform.position = default(Vector3);
					lightningBolts[i].EndObject.transform.position = default(Vector3);
					lightningBolts[i].gameObject.SetActive(false);
					break;
				}
			}
			lightningBolts[i].StartObject.transform.position = default(Vector3);
			lightningBolts[i].EndObject.transform.position = default(Vector3);
			lightningBolts[i].gameObject.SetActive(false);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
	}
}
