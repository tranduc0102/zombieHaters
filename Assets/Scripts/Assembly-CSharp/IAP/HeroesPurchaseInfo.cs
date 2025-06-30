using System;

namespace IAP
{
	[Serializable]
	public class HeroesPurchaseInfo : PurchaseInfo
	{
		public SaveData.HeroData.HeroType heroType;
	}
}
