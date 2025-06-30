using System;
using UnityEngine;

namespace EPPZ.Cloud.Model.Simulation
{
	[Serializable]
	public class KeyValuePair
	{
		public string key;

		public EPPZ.Cloud.Model.KeyValuePair.Type type;

		public bool isChanged;

		private string _stringValue;

		public float floatValue;

		public int intValue;

		public bool boolValue;

		[HideInInspector]
		public bool foldedOut = true;

		public virtual string stringValue
		{
			get
			{
				return _stringValue;
			}
			set
			{
				_stringValue = value;
			}
		}
	}
}
