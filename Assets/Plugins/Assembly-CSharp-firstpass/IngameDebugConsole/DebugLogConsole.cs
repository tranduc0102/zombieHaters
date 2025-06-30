using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace IngameDebugConsole
{
	public static class DebugLogConsole
	{
		public delegate bool ParseFunction(string input, out object output);

		private static Dictionary<string, ConsoleMethodInfo> methods;

		private static Dictionary<Type, ParseFunction> parseFunctions;

		private static Dictionary<Type, string> typeReadableNames;

		private static List<string> commandArguments;

		private static readonly string[] inputDelimiters;

		static DebugLogConsole()
		{
			methods = new Dictionary<string, ConsoleMethodInfo>();
			commandArguments = new List<string>(8);
			inputDelimiters = new string[4] { "\"\"", "{}", "()", "[]" };
			parseFunctions = new Dictionary<Type, ParseFunction>
			{
				{
					typeof(string),
					ParseString
				},
				{
					typeof(bool),
					ParseBool
				},
				{
					typeof(int),
					ParseInt
				},
				{
					typeof(uint),
					ParseUInt
				},
				{
					typeof(long),
					ParseLong
				},
				{
					typeof(ulong),
					ParseULong
				},
				{
					typeof(byte),
					ParseByte
				},
				{
					typeof(sbyte),
					ParseSByte
				},
				{
					typeof(short),
					ParseShort
				},
				{
					typeof(ushort),
					ParseUShort
				},
				{
					typeof(char),
					ParseChar
				},
				{
					typeof(float),
					ParseFloat
				},
				{
					typeof(double),
					ParseDouble
				},
				{
					typeof(decimal),
					ParseDecimal
				},
				{
					typeof(Vector2),
					ParseVector2
				},
				{
					typeof(Vector3),
					ParseVector3
				},
				{
					typeof(Vector4),
					ParseVector4
				},
				{
					typeof(GameObject),
					ParseGameObject
				}
			};
			typeReadableNames = new Dictionary<Type, string>
			{
				{
					typeof(string),
					"String"
				},
				{
					typeof(bool),
					"Boolean"
				},
				{
					typeof(int),
					"Integer"
				},
				{
					typeof(uint),
					"Unsigned Integer"
				},
				{
					typeof(long),
					"Long"
				},
				{
					typeof(ulong),
					"Unsigned Long"
				},
				{
					typeof(byte),
					"Byte"
				},
				{
					typeof(sbyte),
					"Short Byte"
				},
				{
					typeof(short),
					"Short"
				},
				{
					typeof(ushort),
					"Unsigned Short"
				},
				{
					typeof(char),
					"Char"
				},
				{
					typeof(float),
					"Float"
				},
				{
					typeof(double),
					"Double"
				},
				{
					typeof(decimal),
					"Decimal"
				},
				{
					typeof(Vector2),
					"Vector2"
				},
				{
					typeof(Vector3),
					"Vector3"
				},
				{
					typeof(Vector4),
					"Vector4"
				},
				{
					typeof(GameObject),
					"GameObject"
				}
			};
			HashSet<Assembly> hashSet = new HashSet<Assembly> { Assembly.GetAssembly(typeof(DebugLogConsole)) };
			try
			{
				hashSet.Add(Assembly.Load("Assembly-CSharp"));
			}
			catch
			{
			}
			foreach (Assembly item in hashSet)
			{
				Type[] exportedTypes = item.GetExportedTypes();
				foreach (Type type in exportedTypes)
				{
					MethodInfo[] array = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
					foreach (MethodInfo methodInfo in array)
					{
						object[] customAttributes = methodInfo.GetCustomAttributes(typeof(ConsoleMethodAttribute), false);
						foreach (object obj2 in customAttributes)
						{
							ConsoleMethodAttribute consoleMethodAttribute = obj2 as ConsoleMethodAttribute;
							if (consoleMethodAttribute != null)
							{
								AddCommand(consoleMethodAttribute.Command, consoleMethodAttribute.Description, methodInfo);
							}
						}
					}
				}
			}
		}

		[ConsoleMethod("help", "Prints all commands")]
		public static void LogAllCommands()
		{
			int num = 20;
			foreach (KeyValuePair<string, ConsoleMethodInfo> method in methods)
			{
				if (method.Value.IsValid())
				{
					num += 3 + method.Value.signature.Length;
				}
			}
			StringBuilder stringBuilder = new StringBuilder(num);
			stringBuilder.Append("Available commands:");
			foreach (KeyValuePair<string, ConsoleMethodInfo> method2 in methods)
			{
				if (method2.Value.IsValid())
				{
					stringBuilder.Append("\n- ").Append(method2.Value.signature);
				}
			}
			Debug.Log(stringBuilder.Append("\n").ToString());
		}

		[ConsoleMethod("sysinfo", "Prints system information")]
		public static void LogSystemInfo()
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append("Rig: ").AppendSysInfoIfPresent(SystemInfo.deviceModel).AppendSysInfoIfPresent(SystemInfo.processorType)
				.AppendSysInfoIfPresent(SystemInfo.systemMemorySize, "MB RAM")
				.Append(SystemInfo.processorCount)
				.Append(" cores\n");
			stringBuilder.Append("OS: ").Append(SystemInfo.operatingSystem).Append("\n");
			stringBuilder.Append("GPU: ").Append(SystemInfo.graphicsDeviceName).Append(" ")
				.Append(SystemInfo.graphicsMemorySize)
				.Append("MB ")
				.Append(SystemInfo.graphicsDeviceVersion)
				.Append((!SystemInfo.graphicsMultiThreaded) ? "\n" : " multi-threaded\n");
			stringBuilder.Append("Data Path: ").Append(Application.dataPath).Append("\n");
			stringBuilder.Append("Persistent Data Path: ").Append(Application.persistentDataPath).Append("\n");
			stringBuilder.Append("StreamingAssets Path: ").Append(Application.streamingAssetsPath).Append("\n");
			stringBuilder.Append("Temporary Cache Path: ").Append(Application.temporaryCachePath).Append("\n");
			stringBuilder.Append("Device ID: ").Append(SystemInfo.deviceUniqueIdentifier).Append("\n");
			stringBuilder.Append("Max Texture Size: ").Append(SystemInfo.maxTextureSize).Append("\n");
			stringBuilder.Append("Max Cubemap Size: ").Append(SystemInfo.maxCubemapSize).Append("\n");
			stringBuilder.Append("Accelerometer: ").Append((!SystemInfo.supportsAccelerometer) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Gyro: ").Append((!SystemInfo.supportsGyroscope) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Location Service: ").Append((!SystemInfo.supportsLocationService) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Image Effects: ").Append((!SystemInfo.supportsImageEffects) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Compute Shaders: ").Append((!SystemInfo.supportsComputeShaders) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Shadows: ").Append((!SystemInfo.supportsShadows) ? "not supported\n" : "supported\n");
			stringBuilder.Append("RenderToCubemap: ").Append((!SystemInfo.supportsRenderToCubemap) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Instancing: ").Append((!SystemInfo.supportsInstancing) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Motion Vectors: ").Append((!SystemInfo.supportsMotionVectors) ? "not supported\n" : "supported\n");
			stringBuilder.Append("3D Textures: ").Append((!SystemInfo.supports3DTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("3D Render Textures: ").Append((!SystemInfo.supports3DRenderTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("2D Array Textures: ").Append((!SystemInfo.supports2DArrayTextures) ? "not supported\n" : "supported\n");
			stringBuilder.Append("Cubemap Array Textures: ").Append((!SystemInfo.supportsCubemapArrayTextures) ? "not supported" : "supported");
			Debug.Log(stringBuilder.Append("\n").ToString());
		}

		private static StringBuilder AppendSysInfoIfPresent(this StringBuilder sb, string info, string postfix = null)
		{
			if (info != "n/a")
			{
				sb.Append(info);
				if (postfix != null)
				{
					sb.Append(postfix);
				}
				sb.Append(" ");
			}
			return sb;
		}

		private static StringBuilder AppendSysInfoIfPresent(this StringBuilder sb, int info, string postfix = null)
		{
			if (info > 0)
			{
				sb.Append(info);
				if (postfix != null)
				{
					sb.Append(postfix);
				}
				sb.Append(" ");
			}
			return sb;
		}

		public static void AddCommandInstance(string command, string description, string methodName, object instance)
		{
			if (instance == null)
			{
				Debug.LogError("Instance can't be null!");
			}
			else
			{
				AddCommand(command, description, methodName, instance.GetType(), instance);
			}
		}

		public static void AddCommandStatic(string command, string description, string methodName, Type ownerType)
		{
			AddCommand(command, description, methodName, ownerType);
		}

		public static void RemoveCommand(string command)
		{
			if (!string.IsNullOrEmpty(command))
			{
				methods.Remove(command);
			}
		}

		private static void AddCommand(string command, string description, string methodName, Type ownerType, object instance = null)
		{
			if (string.IsNullOrEmpty(command))
			{
				Debug.LogError("Command name can't be empty!");
				return;
			}
			command = command.Trim();
			if (command.IndexOf(' ') >= 0)
			{
				Debug.LogError("Command name can't contain whitespace: " + command);
				return;
			}
			MethodInfo method = ownerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				Debug.LogError(methodName + " does not exist in " + ownerType);
			}
			else
			{
				AddCommand(command, description, method, instance);
			}
		}

		private static void AddCommand(string command, string description, MethodInfo method, object instance = null)
		{
			ParameterInfo[] array = method.GetParameters();
			if (array == null)
			{
				array = new ParameterInfo[0];
			}
			bool flag = true;
			Type[] array2 = new Type[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Type parameterType = array[i].ParameterType;
				if (parseFunctions.ContainsKey(parameterType))
				{
					array2[i] = parameterType;
					continue;
				}
				flag = false;
				break;
			}
			if (!flag)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.Append(command).Append(": ");
			if (!string.IsNullOrEmpty(description))
			{
				stringBuilder.Append(description).Append(" -> ");
			}
			stringBuilder.Append(method.DeclaringType.ToString()).Append(".").Append(method.Name)
				.Append("(");
			for (int j = 0; j < array2.Length; j++)
			{
				Type type = array2[j];
				string value;
				if (!typeReadableNames.TryGetValue(type, out value))
				{
					value = type.Name;
				}
				stringBuilder.Append(value);
				if (j < array2.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(")");
			Type returnType = method.ReturnType;
			if (returnType != typeof(void))
			{
				string value2;
				if (!typeReadableNames.TryGetValue(returnType, out value2))
				{
					value2 = returnType.Name;
				}
				stringBuilder.Append(" : ").Append(value2);
			}
			methods[command] = new ConsoleMethodInfo(method, array2, instance, stringBuilder.ToString());
		}

		public static void ExecuteCommand(string command)
		{
			if (command == null)
			{
				return;
			}
			command = command.Trim();
			if (command.Length == 0)
			{
				return;
			}
			commandArguments.Clear();
			int num = IndexOfChar(command, ' ', 0);
			commandArguments.Add(command.Substring(0, num));
			for (int i = num + 1; i < command.Length; i++)
			{
				if (command[i] != ' ')
				{
					int num2 = IndexOfDelimiter(command[i]);
					if (num2 >= 0)
					{
						num = IndexOfChar(command, inputDelimiters[num2][1], i + 1);
						commandArguments.Add(command.Substring(i + 1, num - i - 1));
					}
					else
					{
						num = IndexOfChar(command, ' ', i + 1);
						commandArguments.Add(command.Substring(i, num - i));
					}
					i = num;
				}
			}
			ConsoleMethodInfo value;
			if (!methods.TryGetValue(commandArguments[0], out value))
			{
				Debug.LogWarning("Can't find command: " + commandArguments[0]);
				return;
			}
			if (!value.IsValid())
			{
				Debug.LogWarning("Method no longer valid (instance dead): " + commandArguments[0]);
				return;
			}
			if (value.parameterTypes.Length != commandArguments.Count - 1)
			{
				Debug.LogWarning("Parameter count mismatch: " + value.parameterTypes.Length + " parameters are needed");
				return;
			}
			Debug.Log("Executing command: " + commandArguments[0]);
			object[] array = new object[value.parameterTypes.Length];
			for (int j = 0; j < value.parameterTypes.Length; j++)
			{
				string text = commandArguments[j + 1];
				Type type = value.parameterTypes[j];
				ParseFunction value2;
				if (!parseFunctions.TryGetValue(type, out value2))
				{
					Debug.LogError("Unsupported parameter type: " + type.Name);
					return;
				}
				object output;
				if (!value2(text, out output))
				{
					Debug.LogError("Couldn't parse " + text + " to " + type.Name);
					return;
				}
				array[j] = output;
			}
			object obj = value.method.Invoke(value.instance, array);
			if (value.method.ReturnType != typeof(void))
			{
				if (obj == null || obj.Equals(null))
				{
					Debug.Log("Value returned: null");
				}
				else
				{
					Debug.Log("Value returned: " + obj.ToString());
				}
			}
		}

		private static int IndexOfDelimiter(char c)
		{
			for (int i = 0; i < inputDelimiters.Length; i++)
			{
				if (c == inputDelimiters[i][0])
				{
					return i;
				}
			}
			return -1;
		}

		private static int IndexOfChar(string command, char c, int startIndex)
		{
			int num = command.IndexOf(c, startIndex);
			if (num < 0)
			{
				num = command.Length;
			}
			return num;
		}

		private static bool ParseString(string input, out object output)
		{
			output = input;
			return input.Length > 0;
		}

		private static bool ParseBool(string input, out object output)
		{
			if (input == "1" || input.ToLowerInvariant() == "true")
			{
				output = true;
				return true;
			}
			if (input == "0" || input.ToLowerInvariant() == "false")
			{
				output = false;
				return true;
			}
			output = false;
			return false;
		}

		private static bool ParseInt(string input, out object output)
		{
			int result;
			bool result2 = int.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseUInt(string input, out object output)
		{
			uint result;
			bool result2 = uint.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseLong(string input, out object output)
		{
			long result;
			bool result2 = long.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseULong(string input, out object output)
		{
			ulong result;
			bool result2 = ulong.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseByte(string input, out object output)
		{
			byte result;
			bool result2 = byte.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseSByte(string input, out object output)
		{
			sbyte result;
			bool result2 = sbyte.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseShort(string input, out object output)
		{
			short result;
			bool result2 = short.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseUShort(string input, out object output)
		{
			ushort result;
			bool result2 = ushort.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseChar(string input, out object output)
		{
			char result;
			bool result2 = char.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseFloat(string input, out object output)
		{
			float result;
			bool result2 = float.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseDouble(string input, out object output)
		{
			double result;
			bool result2 = double.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseDecimal(string input, out object output)
		{
			decimal result;
			bool result2 = decimal.TryParse(input, out result);
			output = result;
			return result2;
		}

		private static bool ParseVector2(string input, out object output)
		{
			return CreateVectorFromInput(input, typeof(Vector2), out output);
		}

		private static bool ParseVector3(string input, out object output)
		{
			return CreateVectorFromInput(input, typeof(Vector3), out output);
		}

		private static bool ParseVector4(string input, out object output)
		{
			return CreateVectorFromInput(input, typeof(Vector4), out output);
		}

		private static bool ParseGameObject(string input, out object output)
		{
			output = GameObject.Find(input);
			return true;
		}

		private static bool CreateVectorFromInput(string input, Type vectorType, out object output)
		{
			List<string> list = new List<string>(input.Replace(',', ' ').Trim().Split(' '));
			for (int num = list.Count - 1; num >= 0; num--)
			{
				list[num] = list[num].Trim();
				if (list[num].Length == 0)
				{
					list.RemoveAt(num);
				}
			}
			float[] array = new float[list.Count];
			for (int num = 0; num < list.Count; num++)
			{
				float result;
				if (!float.TryParse(list[num], out result))
				{
					if (vectorType == typeof(Vector3))
					{
						output = default(Vector3);
					}
					else if (vectorType == typeof(Vector2))
					{
						output = default(Vector2);
					}
					else
					{
						output = default(Vector4);
					}
					return false;
				}
				array[num] = result;
			}
			if (vectorType == typeof(Vector3))
			{
				Vector3 vector = default(Vector3);
				int num;
				for (num = 0; num < array.Length && num < 3; num++)
				{
					vector[num] = array[num];
				}
				for (; num < 3; num++)
				{
					vector[num] = 0f;
				}
				output = vector;
			}
			else if (vectorType == typeof(Vector2))
			{
				Vector2 vector2 = default(Vector2);
				int num;
				for (num = 0; num < array.Length && num < 2; num++)
				{
					vector2[num] = array[num];
				}
				for (; num < 2; num++)
				{
					vector2[num] = 0f;
				}
				output = vector2;
			}
			else
			{
				Vector4 vector3 = default(Vector4);
				int num;
				for (num = 0; num < array.Length && num < 4; num++)
				{
					vector3[num] = array[num];
				}
				for (; num < 4; num++)
				{
					vector3[num] = 0f;
				}
				output = vector3;
			}
			return true;
		}
	}
}
