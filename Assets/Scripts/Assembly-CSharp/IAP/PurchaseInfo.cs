using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IAP
{
	[Serializable]
	public class PurchaseInfo
	{
		[HideInInspector]
		public int index;

		public string purchaseName;

		public ProductType productType;

		public string purchaseAppStore;

		public string purchaseGooglePlay;
	}
}
