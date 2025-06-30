using UnityEngine;

namespace EPPZ.Cloud.Plugin
{
	public class Cloud
	{
		protected ICloud cloudObject;

		public static Cloud NativePluginInstance(ICloud cloudObject)
		{
			Cloud cloud = ((!Application.isEditor) ? ((Cloud)new Cloud_Android()) : ((Cloud)new Cloud_Editor()));
			cloud.cloudObject = cloudObject;
			return cloud;
		}

		public virtual void InitializeWithGameObjectName(string gameObjectName)
		{
		}

		public virtual void Synchronize()
		{
		}

		public virtual string StringForKey(string key)
		{
			return string.Empty;
		}

		public virtual void SetStringForKey(string value, string key)
		{
		}

		public virtual float FloatForKey(string key)
		{
			return 0f;
		}

		public virtual void SetFloatForKey(float value, string key)
		{
		}

		public virtual int IntForKey(string key)
		{
			return 0;
		}

		public virtual void SetIntForKey(int value, string key)
		{
		}

		public virtual bool BoolForKey(string key)
		{
			return false;
		}

		public virtual void SetBoolForKey(bool value, string key)
		{
		}

		public virtual void CloudDidChange(string message)
		{
		}

		protected void Log(string message)
		{
			EPPZ.Cloud.Cloud.Log(message);
		}
	}
}
