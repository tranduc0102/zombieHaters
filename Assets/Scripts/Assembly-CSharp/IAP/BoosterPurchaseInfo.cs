using System;

namespace IAP
{
	[Serializable]
	public class BoosterPurchaseInfo : PurchaseInfo
	{
		public SaveData.BoostersData.BoosterType boosterType;

		public int boosterCount;
	}
}
