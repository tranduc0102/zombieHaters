using System.Collections;
using DigitalRuby.LightningBolt;
using UnityEngine;

public class Laser : BaseBullet
{
	[SerializeField]
	private float yPosition = 1.35f;

	[SerializeField]
	private LightningBoltScript lightningBolt;

	[SerializeField]
	private ParticleSystem[] startEndParticles;

	[SerializeField]
	private float dampingSpeed = 2f;

	private Transform currentStartTransform;

	private Transform currentEndTransform;

	private float damageFrequency;

	private float shootDistance;

	private Coroutine damagingCor;

	private bool startInProgress;

	private bool endInProgress;

	[HideInInspector]
	public int pvpIndex;

	private new void Start()
	{
	}

	public void DoIt(float damageFrequency, float shootDistance, Transform firstTarget, Transform bulletSpawn)
	{
		this.damageFrequency = damageFrequency;
		this.shootDistance = shootDistance;
		currentStartTransform = bulletSpawn;
		currentEndTransform = firstTarget;
		Update();
		base.gameObject.SetActive(true);
		StartCoroutine(StartEnd(true));
		damagingCor = StartCoroutine(Damaging());
	}

	private void Update()
	{
		if (currentStartTransform != null)
		{
			lightningBolt.StartObject.transform.position = currentStartTransform.position;
		}
		if (currentEndTransform != null)
		{
			lightningBolt.EndObject.transform.position = new Vector3(currentEndTransform.position.x, yPosition, currentEndTransform.position.z);
		}
		if (currentStartTransform == null || currentEndTransform == null)
		{
			StopIt();
			return;
		}
		float num = Vector3.Distance(currentStartTransform.position, currentEndTransform.position);
		if (num <= shootDistance)
		{
			RaycastHit hitInfo = default(RaycastHit);
			Vector3 direction = new Vector3(currentEndTransform.position.x - currentStartTransform.position.x, 1f, currentEndTransform.position.z - currentStartTransform.position.z);
			if (Physics.Raycast(currentStartTransform.position, direction, out hitInfo, num, LayerMask.GetMask("Obstacle", "Car")))
			{
				StopIt();
			}
		}
		else
		{
			StopIt();
		}
	}

	private IEnumerator Damaging()
	{
		if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
		{
			BaseHuman human = currentEndTransform.GetComponent<BaseHuman>();
			while (human != null)
			{
				if (human.tag == "Zombie" || human.tag == "ZombieBoss")
				{
					if (human.TakeDamage(damage, isCriticalDamage) < 0f)
					{
						PVPManager.Instance.AddZombieLoot(pvpIndex);
					}
				}
				else
				{
					human.TakeDamage(damage, isCriticalDamage);
				}
				yield return new WaitForSeconds(damageFrequency);
			}
		}
		else
		{
			ZombieHuman zombie = currentEndTransform.GetComponent<ZombieHuman>();
			if (zombie == null)
			{
				Debug.LogError("Can't get ZombieHuman from object named - " + zombie.name);
			}
			while (zombie != null)
			{
				zombie.TakeDamage(damage, isCriticalDamage);
				yield return new WaitForSeconds(damageFrequency);
			}
		}
		StopIt();
	}

	private void StopIt()
	{
		if (!endInProgress && base.gameObject.activeSelf)
		{
			StopCoroutine(damagingCor);
			StartCoroutine(StartEnd(false));
		}
	}

	private IEnumerator StartEnd(bool isStart)
	{
		while (startInProgress)
		{
			yield return null;
		}
		endInProgress = (startInProgress = isStart);
		while (!lightningBolt.lineRenderer)
		{
			yield return null;
		}
		float targetValue = 0f;
		if (isStart)
		{
			targetValue = 1f;
		}
		ParticleSystem[] array = startEndParticles;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.loop = isStart;
		}
		lightningBolt.lineRenderer.startColor = new Color(lightningBolt.lineRenderer.startColor.r, lightningBolt.lineRenderer.startColor.g, lightningBolt.lineRenderer.startColor.b, targetValue);
		while (lightningBolt.lineRenderer.endColor.a != targetValue)
		{
			lightningBolt.lineRenderer.endColor = new Color(lightningBolt.lineRenderer.endColor.r, lightningBolt.lineRenderer.endColor.g, lightningBolt.lineRenderer.endColor.b, Mathf.MoveTowards(lightningBolt.lineRenderer.endColor.a, targetValue, dampingSpeed * Time.deltaTime));
			yield return null;
		}
		if (!isStart)
		{
			lightningBolt.StartObject.transform.position = default(Vector3);
			lightningBolt.EndObject.transform.position = default(Vector3);
			base.gameObject.SetActive(false);
		}
		Laser laser = this;
		Laser laser2 = this;
		bool flag = false;
		laser2.startInProgress = false;
		laser.endInProgress = flag;
	}

	private void OnCollisionEnter(Collision collision)
	{
	}
}
