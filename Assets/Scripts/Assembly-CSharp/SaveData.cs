using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
	[Serializable]
	public class HeroData : IDataInitializer<HeroData, HeroData.HeroType>
	{
		public enum HeroType
		{
			AUTOMATIC = 0,
			SHOTGUN = 1,
			MEDIC = 2,
			SNIPER = 3,
			COOK = 4,
			MINER = 5,
			RPG = 6,
			SMG = 7,
			PISTOL = 8,
			AUTIOMATIC1 = 9
		}

		public HeroType heroType;

		public int currentLevel;

		public int pickedUpCount;

		public int diedCount;

		public bool isOpened;

		public HeroData Initialize(HeroType type)
		{
			heroType = type;
			currentLevel = 1;
			pickedUpCount = 0;
			diedCount = 0;
			return this;
		}

		public HeroType GetInitializedType()
		{
			return heroType;
		}

		public bool HasType(HeroType type)
		{
			return heroType == type;
		}
	}

	[Serializable]
	public struct ZombieData : IDataInitializer<ZombieData, ZombieData.ZombieType>
	{
		public enum ZombieType
		{
			NORMAL = 0,
			COP = 1,
			FLAG = 2,
			BARBELL = 3,
			CLOWN = 4,
			BOSS = 5,
			DAILYBOSS = 6
		}

		public ZombieType zombieType;

		public int killedByCapsule;

		public int totalTimesKilled;

		public ZombieData Initialize(ZombieType type)
		{
			zombieType = type;
			killedByCapsule = 0;
			totalTimesKilled = 0;
			return this;
		}

		public ZombieType GetInitializedType()
		{
			return zombieType;
		}

		public bool HasType(ZombieType type)
		{
			return zombieType == type;
		}
	}

	[Serializable]
	public struct BoostersData : IDataInitializer<BoostersData, BoostersData.BoosterType>
	{
		public enum BoosterType
		{
			NewSurvivor = 0,
			IncSpeed = 1,
			KillAll = 2
		}

		public BoosterType type;

		public int count;

		public BoostersData Initialize(BoosterType type)
		{
			this.type = type;
			count = 3;
			return this;
		}

		public BoosterType GetInitializedType()
		{
			return type;
		}

		public bool HasType(BoosterType type)
		{
			return this.type == type;
		}
	}

	[Serializable]
	public class PassiveAbilityData : IDataInitializer<PassiveAbilityData, PassiveAbilityTypes>
	{
		public PassiveAbilityTypes abilityType;

		public int abilityLevel;

		public PassiveAbilityTypes GetInitializedType()
		{
			return abilityType;
		}

		public bool HasType(PassiveAbilityTypes type)
		{
			return abilityType == type;
		}

		public PassiveAbilityData Initialize(PassiveAbilityTypes type)
		{
			abilityType = type;
			abilityLevel = 0;
			return this;
		}

		public void Reset()
		{
			abilityLevel = 0;
		}
	}

	[Serializable]
	public struct AchievementsCompleted
	{
		public int typeID;

		public int localID;
	}

	public int survivorMaxLevel;

	public double money;

	public double experience;

	public float totalDamage;

	public int totalDaysInRow;

	public int moneyBoxPicked;

	public long bestScore;

	public int arenaRating;

	public int gamesPlayed;

	public int gems;

	public DateTime firstEnterDate;

	public List<HeroData> heroData;

	public List<ZombieData> zombieData;

	public List<AchievementsCompleted> achievementsCompleted;

	public List<BoostersData> boosters;

	public List<KilledBosses> killedBosses;

	public List<PassiveAbilityData> passiveAbilities;

	private List<T> InitializeDataByEnum<T, U>() where T : IDataInitializer<T, U> where U : struct, IConvertible, IComparable, IFormattable
	{
		List<T> list = new List<T>();
		U[] array = (U[])Enum.GetValues(typeof(U));
		foreach (U type in array)
		{
			list.Add(((T)Activator.CreateInstance(typeof(T))).Initialize(type));
		}
		return list;
	}

	private void InitializeSimpleData()
	{
		money = 0.0;
		gems = 0;
		bestScore = 0L;
		experience = 0.0;
		totalDamage = 0f;
		moneyBoxPicked = 0;
		totalDaysInRow = -1;
		survivorMaxLevel = 150;
		gamesPlayed = 0;
		arenaRating = 0;
	}

	public void Init()
	{
		InitializeSimpleData();
		boosters = InitializeDataByEnum<BoostersData, BoostersData.BoosterType>();
		heroData = InitializeDataByEnum<HeroData, HeroData.HeroType>();
		zombieData = InitializeDataByEnum<ZombieData, ZombieData.ZombieType>();
		passiveAbilities = InitializeDataByEnum<PassiveAbilityData, PassiveAbilityTypes>();
		achievementsCompleted = new List<AchievementsCompleted>();
		killedBosses = new List<KilledBosses>();
		CheckNewData();
		Debug.Log("Data initialized");
	}

	public bool CheckEnumData<T, U>(ref List<T> dataList) where T : IDataInitializer<T, U> where U : struct, IConvertible, IComparable, IFormattable
	{
		bool result = false;
		if (dataList == null)
		{
			dataList = InitializeDataByEnum<T, U>();
			return true;
		}
		int length = Enum.GetValues(typeof(U)).Length;
		if (dataList.Count < length)
		{
			result = true;
			U[] array = (U[])Enum.GetValues(typeof(U));
			foreach (U val in array)
			{
				bool flag = false;
				for (int j = 0; j < dataList.Count; j++)
				{
					if (dataList[j].HasType(val))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					T val2 = default(T).Initialize(val);
					dataList.Add(default(T).Initialize(val));
					Debug.Log("New Data:" + val);
				}
			}
		}
		else if (dataList.Count > length)
		{
			result = true;
			for (int k = 0; k < dataList.Count; k++)
			{
				if (!Enum.IsDefined(typeof(U), dataList[k].GetInitializedType()))
				{
					dataList.Remove(dataList[k]);
					Debug.Log(string.Concat(typeof(U), " Data Removed"));
				}
			}
		}
		return result;
	}

	public bool CheckNewData()
	{
		bool result = false;
		try
		{
			if (CheckEnumData<HeroData, HeroData.HeroType>(ref heroData))
			{
				result = true;
			}
			if (CheckEnumData<ZombieData, ZombieData.ZombieType>(ref zombieData))
			{
				result = true;
			}
			if (CheckEnumData<BoostersData, BoostersData.BoosterType>(ref boosters))
			{
				result = true;
			}
			if (CheckEnumData<PassiveAbilityData, PassiveAbilityTypes>(ref passiveAbilities))
			{
				result = true;
			}
			if (survivorMaxLevel != 150)
			{
				survivorMaxLevel = 150;
				result = true;
			}
			if (killedBosses == null)
			{
				killedBosses = new List<KilledBosses>();
				result = true;
			}
			if (firstEnterDate == DateTime.MinValue)
			{
				TimeManager.instance.SetFirstEnterDate(this);
				result = true;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Something wrong with CheckNewData(). Message: " + ex.Message);
		}
		return result;
	}

	public bool IsHeroOpened(HeroData.HeroType type)
	{
		return heroData.FirstOrDefault((HeroData hData) => hData.heroType == type).isOpened;
	}

	public ZombieData GetZombieByType(ZombieData.ZombieType type)
	{
		return zombieData.First((ZombieData zData) => zData.zombieType == type);
	}

	public PassiveAbilityData GetPassiveAbilityByType(PassiveAbilityTypes type)
	{
		return passiveAbilities.First((PassiveAbilityData paData) => paData.abilityType == type);
	}

	public bool GetTimeInGameCount(out TimeSpan timeSpan)
	{
		timeSpan = default(TimeSpan);
		if (TimeManager.gotDateTime && firstEnterDate != DateTime.MinValue)
		{
			timeSpan = TimeManager.CurrentDateTime.Subtract(firstEnterDate);
			if (timeSpan.TotalSeconds < 0.0)
			{
				TimeManager.instance.SetFirstEnterDate(this);
				timeSpan = TimeManager.CurrentDateTime.Date.Subtract(firstEnterDate);
			}
			return true;
		}
		return false;
	}
}
