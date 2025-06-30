using System;
using EPPZ.Cloud.Model;
using EPPZ.Cloud.Model.Simulation;
using EPPZ.Cloud.Plugin;
using UnityEngine;

namespace EPPZ.Cloud
{
	public class Cloud : MonoBehaviour, ICloud
	{
		public enum Should
		{
			UpdateKeys = 0,
			StopUpdateKeys = 1
		}

		public delegate Should OnCloudChange(string[] changedKeys, ChangeReason changeReason);

		private static Cloud _instance;

		public Settings settings;

		public KeyValueStore simulationKeyValueStore;

		public static OnCloudChange onCloudChange;

		private ChangeReason latestChangeReason;

		private EPPZ.Cloud.Plugin.Cloud _plugin;

		private EPPZ.Cloud.Plugin.Cloud plugin
		{
			get
			{
				if (_plugin == null)
				{
					_plugin = EPPZ.Cloud.Plugin.Cloud.NativePluginInstance(this);
				}
				return _plugin;
			}
		}

		private void Awake()
		{
			_instance = this;
		}

		private void Start()
		{
			if (settings.initializeOnStart)
			{
				_Initialize();
			}
		}

		private void _Initialize()
		{
			plugin.InitializeWithGameObjectName(base.name);
		}

		private void OnDestroy()
		{
			_RemoveOnKeyChangeActions();
		}

		public static void Initialize()
		{
			_instance._Initialize();
		}

		public static void Synchrnonize()
		{
			_instance.plugin.Synchronize();
		}

		public static void OnKeyChange(string key, Action<string> action, int priority = 0)
		{
			_instance._OnKeyChange(key, action, priority);
		}

		public static void OnKeyChange(string key, Action<float> action, int priority = 0)
		{
			_instance._OnKeyChange(key, action, priority);
		}

		public static void OnKeyChange(string key, Action<int> action, int priority = 0)
		{
			_instance._OnKeyChange(key, action, priority);
		}

		public static void OnKeyChange(string key, Action<bool> action, int priority = 0)
		{
			_instance._OnKeyChange(key, action, priority);
		}

		public static void RemoveOnKeyChangeAction(string key, Action<string> action)
		{
			_instance._RemoveOnKeyChangeAction(key, action);
		}

		public static void RemoveOnKeyChangeAction(string key, Action<float> action)
		{
			_instance._RemoveOnKeyChangeAction(key, action);
		}

		public static void RemoveOnKeyChangeAction(string key, Action<int> action)
		{
			_instance._RemoveOnKeyChangeAction(key, action);
		}

		public static void RemoveOnKeyChangeAction(string key, Action<bool> action)
		{
			_instance._RemoveOnKeyChangeAction(key, action);
		}

		public static void SetStringForKey(string value, string key)
		{
			_instance.plugin.SetStringForKey(value, key);
		}

		public static string StringForKey(string key)
		{
			return _instance.plugin.StringForKey(key);
		}

		public static void SetFloatForKey(float value, string key)
		{
			_instance.plugin.SetFloatForKey(value, key);
		}

		public static float FloatForKey(string key)
		{
			return _instance.plugin.FloatForKey(key);
		}

		public static void SetIntForKey(int value, string key)
		{
			_instance.plugin.SetIntForKey(value, key);
		}

		public static int IntForKey(string key)
		{
			return _instance.plugin.IntForKey(key);
		}

		public static void SetBoolForKey(bool value, string key)
		{
			_instance.plugin.SetBoolForKey(value, key);
		}

		public static bool BoolForKey(string key)
		{
			return _instance.plugin.BoolForKey(key);
		}

		public static ChangeReason LatestChangeReason()
		{
			return _instance.latestChangeReason;
		}

		public static void Log(string message)
		{
			if (_instance.settings.log)
			{
				Debug.Log(message);
			}
		}

		public static KeyValueStore SimulationKeyValueStore()
		{
			return _instance.simulationKeyValueStore;
		}

		public static void InvokeOnKeysChanged(string[] changedKeys, ChangeReason changeReason)
		{
			Log(string.Concat("Cloud.InvokeOnKeysChanged(`", changedKeys, "`, `", changeReason, "`)"));
			_instance._OnCloudChange(changedKeys, changeReason);
		}

		private void _RemoveOnKeyChangeAction(string key, object action)
		{
			EPPZ.Cloud.Model.KeyValuePair keyValuePair = settings.KeyValuePairForKey(key);
			if (keyValuePair != null)
			{
				keyValuePair.RemoveOnChangeAction(action);
			}
		}

		private void _OnKeyChange(string key, object action, int priority)
		{
			EPPZ.Cloud.Model.KeyValuePair keyValuePair = settings.KeyValuePairForKey(key);
			if (keyValuePair != null)
			{
				keyValuePair.AddOnChangeAction(action, priority);
			}
		}

		private void _RemoveOnKeyChangeActions()
		{
			EPPZ.Cloud.Model.KeyValuePair[] keyValuePairs = settings.keyValuePairs;
			foreach (EPPZ.Cloud.Model.KeyValuePair keyValuePair in keyValuePairs)
			{
				keyValuePair.RemoveOnChangeActions();
			}
		}

		public void _CloudDidChange(string message)
		{
			plugin.CloudDidChange(message);
		}

		public void _OnCloudChange(string[] changedKeys, ChangeReason changeReason)
		{
			Log(string.Concat("Cloud._OnCloudChange(`", changedKeys, "`, `", changeReason, "`)"));
			latestChangeReason = changeReason;
			if (onCloudChange != null)
			{
				Should should = onCloudChange(changedKeys, changeReason);
				if (should == Should.StopUpdateKeys)
				{
					return;
				}
			}
			foreach (string text in changedKeys)
			{
				EPPZ.Cloud.Model.KeyValuePair[] keyValuePairs = settings.keyValuePairs;
				foreach (EPPZ.Cloud.Model.KeyValuePair keyValuePair in keyValuePairs)
				{
					if (keyValuePair.key == text)
					{
						keyValuePair.InvokeOnValueChangedAction();
					}
				}
			}
		}
	}
}
