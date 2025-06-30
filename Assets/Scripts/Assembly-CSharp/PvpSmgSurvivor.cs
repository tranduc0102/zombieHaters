using UnityEngine;

public class PvpSmgSurvivor : PVPSoldierSurvivor
{
	[SerializeField]
	private float spread = 7f;

	[SerializeField]
	private float spreadSpeed = 2f;

	private float currentSpreadDirection;

	protected override void AddBullet()
	{
		if (currentSpreadDirection >= spread || currentSpreadDirection <= 0f - spread)
		{
			spreadSpeed *= -1f;
		}
		currentSpreadDirection += spreadSpeed;
		ObjectPooler.Instance.GetBullet(bulletStorageIndex).SetInfo(heroDamage, groupController.pvpPlayerIndex, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation * Quaternion.Euler(0f, currentSpreadDirection, 0f));
		bulletsSpawn.Play();
		PlayShootSound();
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}
}
