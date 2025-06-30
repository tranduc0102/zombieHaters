using UnityEngine;

public class PVPShotGunBullet : PVPBaseBullet
{
	[SerializeField]
	private PVPBaseBullet[] bullets;

	[SerializeField]
	private float spread = 25f;

	private float angleStep;

	private float currentAngle;

	protected override void Start()
	{
		base.Start();
		angleStep = spread * 2f / (float)bullets.Length;
		currentAngle = (0f - angleStep) * (float)(bullets.Length / 2);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		for (int i = 0; i < bullets.Length; i++)
		{
			bullets[i].transform.localPosition = Vector3.zero;
			bullets[i].transform.localRotation = Quaternion.identity;
		}
	}

	public override void SetInfo(float damage, int pvpPlayerIndex, Vector3 position, Quaternion rotation)
	{
		base.damage = damage;
		pvpIndex = pvpPlayerIndex;
		base.transform.SetPositionAndRotation(position, rotation);
		for (int i = 0; i < bullets.Length; i++)
		{
			bullets[i].SetInfo(damage, pvpIndex, position, rotation * Quaternion.Euler(0f, currentAngle + angleStep * (float)i + Random.Range(-3f, 3f), 0f));
			bullets[i].gameObject.SetActive(true);
		}
	}

	public override void SetVelocity()
	{
	}

	public void DisableGameobject()
	{
		base.gameObject.SetActive(false);
	}
}
