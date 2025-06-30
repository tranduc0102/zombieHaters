using System;
using System.Collections.Generic;

namespace IAP
{
	[Serializable]
	public class StarterPackPurchaseInfo : CoinsPurchaseInfo
	{
		[Serializable]
		public struct Boosters
		{
			public SaveData.BoostersData.BoosterType boosterType;

			public int amount;
		}

		public PackType starterType;

		public List<Boosters> boosters;

		public string displayedPackName = "Starter Pack";
	}
}
