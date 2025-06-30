using System;
using System.Reflection;

namespace IngameDebugConsole
{
	public class ConsoleMethodInfo
	{
		public readonly MethodInfo method;

		public readonly Type[] parameterTypes;

		public readonly object instance;

		public readonly string signature;

		public ConsoleMethodInfo(MethodInfo method, Type[] parameterTypes, object instance, string signature)
		{
			this.method = method;
			this.parameterTypes = parameterTypes;
			this.instance = instance;
			this.signature = signature;
		}

		public bool IsValid()
		{
			if (!method.IsStatic && (instance == null || instance.Equals(null)))
			{
				return false;
			}
			return true;
		}
	}
}
