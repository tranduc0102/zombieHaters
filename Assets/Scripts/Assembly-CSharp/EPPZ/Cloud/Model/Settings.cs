using UnityEngine;

namespace EPPZ.Cloud.Model
{
	[CreateAssetMenu(fileName = "Cloud settings", menuName = "eppz!/Cloud/Settings")]
	public class Settings : ScriptableObject
	{
		public KeyValuePair[] keyValuePairs;

		public bool initializeOnStart = true;

		public bool log = true;

		public KeyValuePair KeyValuePairForKey(string key)
		{
			KeyValuePair[] array = keyValuePairs;
			foreach (KeyValuePair keyValuePair in array)
			{
				if (keyValuePair.key == key)
				{
					return keyValuePair;
				}
			}
			Debug.LogWarning("eppz! Cloud: Cannot find registered key for `" + key + "`.");
			return null;
		}
	}
}
