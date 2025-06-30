using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EPPZ.Cloud.Model
{
	[Serializable]
	public class KeyValuePair
	{
		public enum Type
		{
			String = 0,
			Float = 1,
			Int = 2,
			Bool = 3
		}

		public string key;

		public Type type;

		private Dictionary<Type, System.Type> actionTypesForTypes = new Dictionary<Type, System.Type>
		{
			{
				Type.String,
				typeof(Action<string>)
			},
			{
				Type.Float,
				typeof(Action<float>)
			},
			{
				Type.Int,
				typeof(Action<int>)
			},
			{
				Type.Bool,
				typeof(Action<bool>)
			}
		};

		private Dictionary<int, object> onChangeActions;

		private Dictionary<Type, Action<object>> invokersForTypes;

		private System.Type actionType
		{
			get
			{
				return actionTypesForTypes[type];
			}
		}

		private Action<object> invoker
		{
			get
			{
				return invokersForTypes[type];
			}
		}

		public string stringValue
		{
			get
			{
				return Cloud.StringForKey(key);
			}
		}

		public float floatValue
		{
			get
			{
				return Cloud.FloatForKey(key);
			}
		}

		public int intValue
		{
			get
			{
				return Cloud.IntForKey(key);
			}
		}

		public bool boolValue
		{
			get
			{
				return Cloud.BoolForKey(key);
			}
		}

		public KeyValuePair()
		{
			onChangeActions = new Dictionary<int, object>();
			invokersForTypes = new Dictionary<Type, Action<object>>
			{
				{
					Type.String,
					delegate(object eachAction)
					{
						((Action<string>)eachAction)(stringValue);
					}
				},
				{
					Type.Float,
					delegate(object eachAction)
					{
						((Action<float>)eachAction)(floatValue);
					}
				},
				{
					Type.Int,
					delegate(object eachAction)
					{
						((Action<int>)eachAction)(intValue);
					}
				},
				{
					Type.Bool,
					delegate(object eachAction)
					{
						((Action<bool>)eachAction)(boolValue);
					}
				}
			};
		}

		public void InvokeOnValueChangedAction()
		{
			Debug.Log("InvokeOnValueChangedAction()");
			Debug.Log("onChangeActions.Count: " + onChangeActions.Count);
			if (onChangeActions.Count == 0)
			{
				return;
			}
			List<int> list = onChangeActions.Keys.ToList();
			list.Sort();
			foreach (int item in list)
			{
				invoker(onChangeActions[item]);
			}
		}

		public void AddOnChangeAction(object action, int priority)
		{
			if (action.GetType() != actionType)
			{
				Debug.LogWarning(string.Concat("eppz! Cloud: Cannot add on change action for key `", key, "` with type `", type, "`. Types mismatched."));
			}
			else
			{
				onChangeActions.Add(priority, action);
			}
		}

		public void RemoveOnChangeAction(object action)
		{
			foreach (KeyValuePair<int, object> item in onChangeActions.Where((KeyValuePair<int, object> keyValuePair) => keyValuePair.Value == action).ToList())
			{
				onChangeActions.Remove(item.Key);
			}
		}

		public void RemoveOnChangeActions()
		{
			onChangeActions.Clear();
		}
	}
}
