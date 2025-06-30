using System;
using EPPZ.Cloud.Model.Simulation;
using UnityEngine;

namespace EPPZ.Cloud.Plugin
{
	public class Cloud_Editor : Cloud
	{
		private KeyValueStore keyValueStore
		{
			get
			{
				return EPPZ.Cloud.Cloud.SimulationKeyValueStore();
			}
		}

		public override void Synchronize()
		{
			keyValueStore.SimulateCloudDidChange();
		}

		public override string StringForKey(string key)
		{
			return keyValueStore.KeyValuePairForKey(key).stringValue;
		}

		public override void SetStringForKey(string value, string key)
		{
			Log("Cloud_Editor.SetStringForKey(`" + value + "`, `" + key + "`)");
			keyValueStore.KeyValuePairForKey(key).stringValue = value;
		}

		public override float FloatForKey(string key)
		{
			return keyValueStore.KeyValuePairForKey(key).floatValue;
		}

		public override void SetFloatForKey(float value, string key)
		{
			Log("Cloud_Editor.SetFloatForKey(`" + value + "`, `" + key + "`)");
			keyValueStore.KeyValuePairForKey(key).floatValue = value;
		}

		public override int IntForKey(string key)
		{
			int result = 0;
			try
			{
				result = keyValueStore.KeyValuePairForKey(key).intValue;
			}
			catch (Exception)
			{
				Debug.LogWarning(key);
			}
			return result;
		}

		public override void SetIntForKey(int value, string key)
		{
			Log("Cloud_Editor.SetIntForKey(`" + value + "`, `" + key + "`)");
			keyValueStore.KeyValuePairForKey(key).intValue = value;
		}

		public override bool BoolForKey(string key)
		{
			return keyValueStore.KeyValuePairForKey(key).boolValue;
		}

		public override void SetBoolForKey(bool value, string key)
		{
			Log("Cloud_Editor.SetBoolForKey(`" + value + "`, `" + key + "`)");
			keyValueStore.KeyValuePairForKey(key).boolValue = value;
		}
	}
}
