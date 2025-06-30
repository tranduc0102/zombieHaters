using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassiveAbilitiesManager : MonoBehaviour
{
	public static BonusHelper bonusHelper;

	public static List<PassiveAbilityTypes> sequence = new List<PassiveAbilityTypes>
	{
		PassiveAbilityTypes.CriticalHit,
		PassiveAbilityTypes.Gold,
		PassiveAbilityTypes.AttackSpeed,
		PassiveAbilityTypes.Damage
	};

	[SerializeField]
	private TextAsset abilityPrice;

	[SerializeField]
	private TextAsset abilityBonus;

	private List<AbilityData> abilityDatas;

	public int maxAbilityLevel;

	public void Initialize()
	{
		abilityDatas = new List<AbilityData>();
		int length = Enum.GetValues(typeof(PassiveAbilityTypes)).Length;
		int[,] array = new int[maxAbilityLevel + 1, length];
		float[,] array2 = new float[maxAbilityLevel + 1, length];
		CsvLoader.SplitText(abilityPrice, ',', ref array);
		CsvLoader.SplitText(abilityBonus, ',', ref array2);
		for (int i = 0; i < length; i++)
		{
			AbilityData abilityData = new AbilityData(sequence[i]);
			abilityData.bonus.Add(0f);
			for (int j = 0; j < array.GetLength(0); j++)
			{
				abilityData.price.Add(array[i, j]);
				abilityData.bonus.Add(array2[i, j]);
			}
			abilityDatas.Add(abilityData);
		}
		bonusHelper = new BonusHelper();
	}

	public bool UpgradeAbility(PassiveAbilityTypes type)
	{
		for (int i = 0; i < abilityDatas.Count; i++)
		{
			if (abilityDatas[i].abilityType == type)
			{
				if (abilityDatas[i].savedAbility.abilityLevel >= maxAbilityLevel)
				{
					return false;
				}
				if (DataLoader.Instance.RefreshGems(-abilityDatas[i].GetCurrentPrice(), true))
				{
					abilityDatas[i].savedAbility.abilityLevel++;
					DataLoader.Instance.UpdateIdleHeroShootDelay();
					return true;
				}
			}
		}
		return false;
	}

	public AbilityData GetAbility(PassiveAbilityTypes abilityType)
	{
		return abilityDatas.First((AbilityData ab) => ab.abilityType == abilityType);
	}
}
