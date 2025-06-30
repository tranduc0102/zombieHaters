using System;
using UnityEngine.UI;

[Serializable]
public class AbilityUpgrade
{
	public PassiveAbilityTypes abilityType;

	public Text dataText;

	public Button upgradeButton;

	public Button buttonReset;

	public void UpdateText()
	{
		int abilityLevel = DataLoader.playerData.GetPassiveAbilityByType(abilityType).abilityLevel;
		AbilityData ability = DataLoader.Instance.passiveAbilitiesManager.GetAbility(abilityType);
		if (abilityLevel == DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel)
		{
			dataText.text = "Level: " + DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel + " Bonus: " + ability.GetCurrentBonus() + " Price: MAX";
		}
		else if (abilityLevel == 0)
		{
			dataText.text = "CLOSED OPEN FOR: " + ability.GetCurrentPrice();
		}
		else
		{
			dataText.text = "Level: " + abilityLevel + " Bonus: " + ability.GetCurrentBonus() + " Price: " + ability.GetCurrentPrice();
		}
	}
}
