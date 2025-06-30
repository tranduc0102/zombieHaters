using UnityEngine;

public class Turret : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem[] bulletsSpawns;

	[SerializeField]
	protected BaseBullet bulletPrefab;

	[SerializeField]
	private float aimDistance = 10f;

	[SerializeField]
	private float spread = 10f;

	[SerializeField]
	private LayerMask maskAimSurvivors;

	[Header("Lightning")]
	[SerializeField]
	private bool isLightning;

	[SerializeField]
	private float damageScale = 1f;

	private float reload;

	private float refindTargetTime = 1.5f;

	private float lastFindTime;

	private Transform targetZombie;

	private int currentSpawnNum;

	private float damage;

	protected Transform targetRotation;

	[SerializeField]
	protected AudioClip[] shotSounds;

	protected bool isLookAtTarget;

	[SerializeField]
	protected float shootDelay = 0.3f;

	private void Start()
	{
		damage = ArenaManager.instance.GetCurrentArenaInfo().turretDamage;
	}

	private void Update()
	{
		if (reload <= 0f)
		{
			FindTargetShoot();
			if (targetZombie != null)
			{
				targetRotation = targetZombie;
			}
			CalculateBodyRotation();
			if (targetZombie != null && isLookAtTarget)
			{
				reload = shootDelay;
				AddBullet();
			}
		}
		else
		{
			reload -= Time.deltaTime;
			CalculateBodyRotation();
		}
	}

	private void FindTargetShoot()
	{
		Vector3 direction;
		RaycastHit hitInfo;
		if (targetZombie != null && Time.time - lastFindTime < refindTargetTime)
		{
			direction = new Vector3(targetZombie.position.x - base.transform.position.x, 0f, targetZombie.position.z - base.transform.position.z);
			if (Physics.Raycast(new Vector3(base.transform.position.x, 1f, base.transform.position.z), direction, out hitInfo, aimDistance, maskAimSurvivors) && (hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieBoss"))
			{
				return;
			}
		}
		targetZombie = null;
		int layerMask = 1 << LayerMask.NameToLayer("Zombie");
		Collider[] array = Physics.OverlapSphere(base.transform.position, aimDistance, layerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			direction = new Vector3(collider.transform.position.x - base.transform.position.x, 0f, collider.transform.position.z - base.transform.position.z);
			if (Physics.Raycast(new Vector3(base.transform.position.x, 1f, base.transform.position.z), direction, out hitInfo, aimDistance, maskAimSurvivors) && (hitInfo.transform.tag == "Zombie" || hitInfo.transform.tag == "ZombieBoss") && (targetZombie == null || Vector3.Distance(base.transform.position, hitInfo.transform.position) < Vector3.Distance(base.transform.position, targetZombie.position)))
			{
				targetZombie = hitInfo.transform;
			}
		}
		lastFindTime = Time.time;
	}

	private void CalculateBodyRotation()
	{
		isLookAtTarget = false;
		if (!(targetRotation == null))
		{
			Vector3 vector = targetRotation.position - base.transform.position;
			Vector3 eulerAngles = base.transform.eulerAngles;
			base.transform.LookAt(targetRotation);
			float y = base.transform.eulerAngles.y;
			base.transform.eulerAngles = eulerAngles;
			base.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(base.transform.eulerAngles.y, y, 0.5f), 0f);
			if (Mathf.Abs(Mathf.DeltaAngle(base.transform.eulerAngles.y, y)) < 2f)
			{
				isLookAtTarget = true;
			}
		}
	}

	protected virtual void AddBullet()
	{
		if (isLightning)
		{
			Lightning lightning = Object.Instantiate(bulletPrefab, bulletsSpawns[currentSpawnNum].transform.position, bulletsSpawns[currentSpawnNum].transform.rotation) as Lightning;
			lightning.DoIt(targetZombie, bulletsSpawns[currentSpawnNum].transform);
			lightning.damage = damage * damageScale;
		}
		else
		{
			BaseBullet baseBullet = Object.Instantiate(bulletPrefab, bulletsSpawns[currentSpawnNum].transform.position, bulletsSpawns[currentSpawnNum].transform.rotation);
			baseBullet.transform.Rotate(0f, Random.Range(0f - spread, spread), 0f);
			baseBullet.damage = damage;
			bulletsSpawns[currentSpawnNum].Play();
		}
		SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		if (currentSpawnNum + 1 >= bulletsSpawns.Length)
		{
			currentSpawnNum = 0;
		}
		else
		{
			currentSpawnNum++;
		}
	}
}
