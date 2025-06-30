using System;
using System.Collections.Generic;
using EPPZ.Cloud.Plugin;
using UnityEngine;

namespace EPPZ.Cloud.Model.Simulation
{
	[CreateAssetMenu(fileName = "Key-value store simulation", menuName = "eppz!/Cloud/Key-value store simulation")]
	public class KeyValueStore : ScriptableObject
	{
		public ChangeReason changeReason;

		public KeyValuePair[] keyValuePairs;

		public Cloud_Editor plugin;

		public virtual void EnumerateKeyValuePairs(Action<KeyValuePair> action)
		{
			KeyValuePair[] array = keyValuePairs;
			foreach (KeyValuePair obj in array)
			{
				action(obj);
			}
		}

		public virtual KeyValuePair KeyValuePairForKey(string key)
		{
			KeyValuePair[] array = keyValuePairs;
			foreach (KeyValuePair keyValuePair in array)
			{
				if (keyValuePair.key == key)
				{
					return keyValuePair;
				}
			}
			Debug.LogWarning("eppz! Cloud: No Key-value pair defined for key `" + key + "`");
			return null;
		}

		[ContextMenu("Simulate `CloudDidChange`")]
		public virtual void SimulateCloudDidChange()
		{
			Debug.Log("SimulateCloudDidChange()");
			List<string> changedKeys = new List<string>();
			EnumerateKeyValuePairs(delegate(KeyValuePair eachKeyValuePair)
			{
				if (eachKeyValuePair.isChanged)
				{
					changedKeys.Add(eachKeyValuePair.key);
					eachKeyValuePair.isChanged = false;
				}
			});
			Debug.Log(string.Concat("changedKeys: `", changedKeys.ToArray(), "`"));
			Cloud.InvokeOnKeysChanged(changedKeys.ToArray(), changeReason);
		}
	}
}
