using System.Runtime.InteropServices;
using EPPZ.Cloud.Plugin.iOS;
using UnityEngine;

namespace EPPZ.Cloud.Plugin
{
	public class Cloud_iOS : Cloud
	{
		[DllImport("__Internal")]
		private static extern void EPPZ_Cloud_InitializeWithGameObjectName(string gameObjectName);

		[DllImport("__Internal")]
		private static extern bool EPPZ_Cloud_Synchronize();

		[DllImport("__Internal")]
		private static extern string EPPZ_Cloud_StringForKey(string key);

		[DllImport("__Internal")]
		private static extern void EPPZ_Cloud_SetStringForKey(string value, string key);

		[DllImport("__Internal")]
		private static extern float EPPZ_Cloud_FloatForKey(string key);

		[DllImport("__Internal")]
		private static extern void EPPZ_Cloud_SetFloatForKey(float value, string key);

		[DllImport("__Internal")]
		private static extern int EPPZ_Cloud_IntForKey(string key);

		[DllImport("__Internal")]
		private static extern void EPPZ_Cloud_SetIntForKey(int value, string key);

		[DllImport("__Internal")]
		private static extern bool EPPZ_Cloud_BoolForKey(string key);

		[DllImport("__Internal")]
		private static extern void EPPZ_Cloud_SetBoolForKey(bool value, string key);

		public override void InitializeWithGameObjectName(string gameObjectName)
		{
			EPPZ_Cloud_InitializeWithGameObjectName(gameObjectName);
		}

		public override void Synchronize()
		{
			EPPZ_Cloud_Synchronize();
		}

		public override void CloudDidChange(string message)
		{
			Log("Cloud_iOS.CloudDidChange(`" + message + "`)");
			UserInfo userInfo = new UserInfo();
			JsonUtility.FromJsonOverwrite(message, userInfo);
			ChangeReason nSUbiquitousKeyValueStoreChangeReasonKey = (ChangeReason)userInfo.NSUbiquitousKeyValueStoreChangeReasonKey;
			string[] nSUbiquitousKeyValueStoreChangedKeysKey = userInfo.NSUbiquitousKeyValueStoreChangedKeysKey;
			Log(string.Concat("Cloud_iOS.CloudDidChange.changeReason: `", nSUbiquitousKeyValueStoreChangeReasonKey, "`"));
			Log(string.Concat("Cloud_iOS.CloudDidChange.changedKeys: `", nSUbiquitousKeyValueStoreChangedKeysKey, "`"));
			cloudObject._OnCloudChange(nSUbiquitousKeyValueStoreChangedKeysKey, nSUbiquitousKeyValueStoreChangeReasonKey);
		}

		public override string StringForKey(string key)
		{
			return EPPZ_Cloud_StringForKey(key);
		}

		public override void SetStringForKey(string value, string key)
		{
			Log("Cloud_iOS.SetStringForKey(`" + value + "`, `" + key + "`)");
			EPPZ_Cloud_SetStringForKey(value, key);
		}

		public override float FloatForKey(string key)
		{
			return EPPZ_Cloud_FloatForKey(key);
		}

		public override void SetFloatForKey(float value, string key)
		{
			Log("Cloud_iOS.SetFloatForKey(`" + value + "`, `" + key + "`)");
			EPPZ_Cloud_SetFloatForKey(value, key);
		}

		public override int IntForKey(string key)
		{
			return EPPZ_Cloud_IntForKey(key);
		}

		public override void SetIntForKey(int value, string key)
		{
			Log("Cloud_iOS.SetIntForKey(`" + value + "`, `" + key + "`)");
			EPPZ_Cloud_SetIntForKey(value, key);
		}

		public override bool BoolForKey(string key)
		{
			return EPPZ_Cloud_BoolForKey(key);
		}

		public override void SetBoolForKey(bool value, string key)
		{
			Log("Cloud_iOS.SetBoolForKey(`" + value + "`, `" + key + "`)");
			EPPZ_Cloud_SetBoolForKey(value, key);
		}
	}
}
