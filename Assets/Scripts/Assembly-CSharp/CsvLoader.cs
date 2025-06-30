using System.ComponentModel;
using UnityEngine;

public static class CsvLoader
{
	public static void SplitText<T>(TextAsset textAsset, char separator, ref T[,] array)
	{
		string[] array2 = SplitLines(textAsset);
		for (int i = 0; i < array2.GetLength(0); i++)
		{
			string[] array3 = array2[i].Split(separator);
			for (int j = 0; j < array3.Length; j++)
			{
				CustomTryParse<T>(array3[j], out array[i, j]);
			}
		}
	}

	public static void SplitText<T>(TextAsset textAsset, char separator, out T[] array)
	{
		string[] array2 = SplitLines(textAsset);
		array = new T[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(separator);
			CustomTryParse<T>(array3[1], out array[i]);
		}
	}

	public static string[] SplitLines(TextAsset textAsset)
	{
		return textAsset.text.Split('\n');
	}

	private static bool CustomTryParse<T>(string input, out T output)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
		if (converter != null && converter.IsValid(input))
		{
			output = (T)converter.ConvertFromString(input);
			return true;
		}
		output = default(T);
		return false;
	}
}
