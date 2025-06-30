using System;
using UnityEngine;

internal class GooglePurchaseData
{
	[Serializable]
	private struct GooglePurchaseReceipt
	{
		public string Payload;
	}

	[Serializable]
	private struct GooglePurchasePayload
	{
		public string json;

		public string signature;
	}

	[Serializable]
	public struct GooglePurchaseJson
	{
		public string autoRenewing;

		public string orderId;

		public string packageName;

		public string productId;

		public string purchaseTime;

		public string purchaseState;

		public string developerPayload;

		public string purchaseToken;
	}

	public string inAppPurchaseData;

	public string inAppDataSignature;

	public GooglePurchaseJson json;

	public GooglePurchaseData(string receipt)
	{
		try
		{
			GooglePurchasePayload googlePurchasePayload = JsonUtility.FromJson<GooglePurchasePayload>(JsonUtility.FromJson<GooglePurchaseReceipt>(receipt).Payload);
			GooglePurchaseJson googlePurchaseJson = JsonUtility.FromJson<GooglePurchaseJson>(googlePurchasePayload.json);
			inAppPurchaseData = googlePurchasePayload.json;
			inAppDataSignature = googlePurchasePayload.signature;
			json = googlePurchaseJson;
		}
		catch
		{
			Debug.Log("Could not parse receipt: " + receipt);
			inAppPurchaseData = string.Empty;
			inAppDataSignature = string.Empty;
		}
	}
}
