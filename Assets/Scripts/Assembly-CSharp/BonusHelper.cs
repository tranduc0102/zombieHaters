public class BonusHelper
{
	private AbilityData attackSpeedAbility;

	private AbilityData criticalHitAbility;

	private AbilityData damageAbility;

	private AbilityData goldAbility;

	public float AttackSpeedBonus
	{
		get
		{
			return attackSpeedAbility.GetCurrentBonus();
		}
	}

	public float CriticalHitBonus
	{
		get
		{
			return criticalHitAbility.GetCurrentBonus() + 1f;
		}
	}

	public float GoldBonus
	{
		get
		{
			return goldAbility.GetCurrentBonus() + (float)(DataLoader.Instance.moneyMultiplier - 1);
		}
	}

	public float DamageBonus
	{
		get
		{
			return damageAbility.GetCurrentBonus();
		}
	}

	public BonusHelper()
	{
		attackSpeedAbility = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilityTypes.AttackSpeed);
		criticalHitAbility = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilityTypes.CriticalHit);
		damageAbility = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilityTypes.Damage);
		goldAbility = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilityTypes.Gold);
	}

	public float GetCriticalHitChance()
	{
		return criticalHitAbility.bonus[criticalHitAbility.savedAbility.abilityLevel];
	}

	public float GetHeroAddedPower()
	{
		return (1f + GetCriticalHitChance()) * (1f + AttackSpeedBonus) * (1f + DamageBonus);
	}
}
