using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityData
{
	public PassiveAbilityTypes abilityType;

	public List<int> price = new List<int>();

	public List<float> bonus = new List<float>();

	public SaveData.PassiveAbilityData savedAbility;

	public AbilityData(PassiveAbilityTypes abilityType)
	{
		this.abilityType = abilityType;
		savedAbility = DataLoader.playerData.GetPassiveAbilityByType(abilityType);
		if (savedAbility.abilityLevel > DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel)
		{
			savedAbility.abilityLevel = DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel;
		}
	}

	public bool IsOpened()
	{
		return savedAbility.abilityLevel > 0;
	}

	public int GetCurrentPrice()
	{
		return price[savedAbility.abilityLevel];
	}

	public float GetCurrentBonus()
	{
		if (IsOpened())
		{
			PassiveAbilityTypes passiveAbilityTypes = abilityType;
			if (passiveAbilityTypes == PassiveAbilityTypes.CriticalHit)
			{
				return (Random.value < bonus[savedAbility.abilityLevel]) ? 1 : 0;
			}
			return bonus[savedAbility.abilityLevel];
		}
		return 0f;
	}

	public float GetBonusBylevel(int level)
	{
		if (level < bonus.Count)
		{
			return bonus[level];
		}
		return bonus.Last();
	}

	public bool IsMaxLevel()
	{
		return savedAbility.abilityLevel >= DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel;
	}

	public string GetLocalizedName()
	{
		switch (abilityType)
		{
		case PassiveAbilityTypes.CriticalHit:
			return LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Critical_shot);
		case PassiveAbilityTypes.AttackSpeed:
			return LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Quick_shot);
		case PassiveAbilityTypes.Damage:
			return LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Amplified_shot);
		case PassiveAbilityTypes.Gold:
			return LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Bonus_to_gold);
		default:
			Debug.LogError("Unknown ability type: " + abilityType);
			return string.Empty;
		}
	}
}
