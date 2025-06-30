using UnityEngine;

public class PvpRpg : PVPMine
{
	[SerializeField]
	private ParticleSystem trailPs;

	protected override void OnDisable()
	{
		base.OnDisable();
		trailPs.Stop();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		trailPs.Play();
	}

	public override void SetVelocity()
	{
		rigid.velocity = new Vector3(base.transform.right.x, 0f, base.transform.right.z) * speed;
	}
}
