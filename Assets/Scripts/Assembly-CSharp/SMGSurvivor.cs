using UnityEngine;

public class SMGSurvivor : SoldierSurvivor
{
	[SerializeField]
	private float spread = 7f;

	[SerializeField]
	private float spreadSpeed = 2f;

	private float currentSpreadDirection;

	protected override void AddBullet()
	{
		GameObject gameObject = Object.Instantiate(bulletPrefab, bulletsSpawn.transform.position, bulletsSpawn.transform.rotation);
		SetBulletDamage(gameObject);
		if (currentSpreadDirection >= spread || currentSpreadDirection <= 0f - spread)
		{
			spreadSpeed *= -1f;
		}
		currentSpreadDirection += spreadSpeed;
		gameObject.transform.Rotate(0f, currentSpreadDirection, 0f);
		bulletsSpawn.Play();
		SoundManager.Instance.PlaySound(shotSounds[Random.Range(0, shotSounds.Length)]);
		if ((bool)animator && shootExists)
		{
			animator.SetBool("Shoot", true);
		}
	}
}
