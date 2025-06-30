using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
	private static bool debug = true;

	public static void Save<T>(T data, string path)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (FileStream serializationStream = new FileStream(path, FileMode.OpenOrCreate))
		{
			binaryFormatter.Serialize(serializationStream, data);
			if (debug)
			{
				Debug.Log("Data Saved");
			}
		}
	}

	public static bool Load<T>(string path, ref T data)
	{
		try
		{
			if (File.Exists(path))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using (FileStream serializationStream = new FileStream(path, FileMode.OpenOrCreate))
				{
					data = (T)binaryFormatter.Deserialize(serializationStream);
					if (debug)
					{
						Debug.Log("Data Loaded");
					}
				}
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			return false;
		}
	}

	public static void SaveJson<T>(T data, string path)
	{
		File.WriteAllText(path, JsonUtility.ToJson(data, true));
	}

	public static bool LoadJson<T>(string path, ref T data)
	{
		try
		{
			if (File.Exists(path))
			{
				data = JsonUtility.FromJson<T>(File.ReadAllText(path));
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			return false;
		}
	}

	public static void RemoveData(string path)
	{
		File.Delete(path);
		if (debug)
		{
			Debug.Log("Data Removed");
		}
	}
}
