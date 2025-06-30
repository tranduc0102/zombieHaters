/*using System.Runtime.InteropServices;
using UnityEngine;

public class DCGAdsClient_iOS : MonoBehaviour
{
	[DllImport("__Internal")]
	internal static extern void DCGAdsInitializeSDK(string gameObjectName, string serverHost, string appKey, string pubKey);

	public static void InitializdeSDK(string gameObjectName, string serverHost, string appKey, string pubKey)
	{
		DCGAdsInitializeSDK(gameObjectName, serverHost, appKey, pubKey);
	}

	public void OCCallbackHandler(string data)
	{
		Debug.Log("回调");
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
*/