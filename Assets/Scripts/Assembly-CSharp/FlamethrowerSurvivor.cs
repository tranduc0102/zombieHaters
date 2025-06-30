using UnityEngine;

public class FlamethrowerSurvivor : SoldierSurvivor
{
	[SerializeField]
	private Flame flame;

	public float addedRotationAngle = 15f;

	public override void Start()
	{
		base.Start();
		findShootMask = 1 << LayerMask.NameToLayer("Zombie");
		refindTargetTime += Random.Range(-0.2f, 0.2f);
		SetFlame(false);
	}

	private new void Update()
	{
		if (reload <= 0f)
		{
			FindTargetShoot();
			if (targetZombie != null)
			{
				targetRotation = targetZombie;
			}
			base.Update();
			SetFlame(targetZombie != null);
			if (targetZombie != null)
			{
				Debug.DrawLine(base.transform.position, targetZombie.transform.position, Color.red);
				CancelInvoke("RotateForward");
			}
			Invoke("RotateForward", Mathf.Max(1f, shootDelay + 0.3f));
		}
		else
		{
			reload -= Time.deltaTime;
			base.Update();
		}
	}

	public void SetFlame(bool active)
	{
		flame.gameObject.SetActive(active);
	}

	public override void CalculateBodyRotation()
	{
		isLookAtTarget = false;
		if (!(targetRotation == null))
		{
			Vector3 vector = targetRotation.position - base.transform.position;
			if (rigid.velocity == Vector3.zero && targetRotation == targetMove)
			{
				vector = -vector;
			}
			float num = Vector3.Angle(vector, base.transform.forward);
			if (Vector3.Angle(vector, base.transform.right) > 90f)
			{
				num *= -1f;
			}
			num += addedRotationAngle;
			body.transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(body.transform.eulerAngles.y, num, 0.5f), 0f);
			if (Mathf.Abs(Mathf.DeltaAngle(body.transform.eulerAngles.y, num)) < 2f)
			{
				isLookAtTarget = true;
			}
		}
	}
}
