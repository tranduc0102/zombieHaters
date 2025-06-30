using System;

[Serializable]
public class DailyBonusData
{
	public enum RewardType
	{
		Money = 0,
		Multiplier = 1,
		Booster = 2
	}

	public RewardType type;

	public SaveData.BoostersData.BoosterType boosterType;

	public int reward;
}
