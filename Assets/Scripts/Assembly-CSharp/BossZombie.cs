using UnityEngine;

public class BossZombie : ZombieHuman
{
	private static ParticleSystem deadFx;

	[SerializeField]
	public string myNameIs;

	protected override void Start()
	{
		base.Start();
		DataLoader.gui.RefreshBossHealth(maxCountHealth, countHealth, myNameIs);
	}

	protected override void CreateTakeDamageFx()
	{
		if (takeDamageFx == null)
		{
			takeDamageFx = Object.Instantiate(GameManager.instance.prefabTakeDamageZombieBoss, animator.transform).GetComponent<ParticleSystem>();
		}
	}

	public override float TakeDamage(float damage, bool isCritical = false)
	{
		if (countHealth <= 0f)
		{
			return countHealth;
		}
		base.TakeDamage(damage, isCritical);
		if (countHealth <= 0f)
		{
			if (GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay)
			{
				WavesManager.instance.BossDead();
			}
			if (GameManager.instance.currentGameMode == GameManager.GameModes.DailyBoss)
			{
				WavesManager.instance.StopIt();
			}
			double number = DataLoader.Instance.SaveKilledBoss(power, myNameIs);
			FloatingText floatingText = Object.Instantiate(GameManager.instance.floatingTextPrefab, base.transform.position, Quaternion.identity, TransformParentManager.Instance.fx);
			floatingText.StartBossAnimation(AbbreviationUtility.AbbreviateNumber(number));
			if (deadFx == null)
			{
				deadFx = Object.Instantiate(GameManager.instance.prefabBossDeathFx);
			}
			deadFx.transform.position = new Vector3(base.transform.position.x, deadFx.transform.position.y, base.transform.position.z);
			deadFx.Play();
		}
		DataLoader.gui.RefreshBossHealth(maxCountHealth, countHealth, myNameIs);
		return countHealth;
	}
}
