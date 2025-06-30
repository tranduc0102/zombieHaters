using UnityEngine;

public class PVPMine : PVPBaseBullet
{
	[SerializeField]
	private int bangSize = 4;

	[SerializeField]
	private AudioClip explosionSound;

	private bool bangIs;

	protected override void OnEnable()
	{
		base.OnEnable();
		bangIs = false;
	}

	protected override void DisableBullet()
	{
		base.DisableBullet();
		if (!bangIs)
		{
			Bang();
		}
	}

	protected override void DisableObject()
	{
		if (!bangIs)
		{
			DisableBullet();
		}
		else
		{
			base.DisableObject();
		}
	}

	public override void SetVelocity()
	{
		rigid.AddForce(new Vector3(base.transform.right.x, 0f, base.transform.right.z) * speed);
	}

	private void Bang()
	{
		bangIs = true;
		Collider[] array = Physics.OverlapSphere(base.transform.position, bangSize);
		for (int i = 0; i < array.Length; i++)
		{
			OnTriggerEnter(array[i]);
		}
		SoundManager.Instance.PlaySound(explosionSound);
	}
}
