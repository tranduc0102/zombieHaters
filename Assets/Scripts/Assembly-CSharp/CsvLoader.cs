using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

public static class CsvLoader
{
    public static void SplitText<T>(TextAsset textAsset, char separator, ref T[,] array)
    {
        string[] lines = SplitLines(textAsset);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(separator);
            for (int j = 0; j < values.Length; j++)
            {
                CustomTryParse(values[j], out array[i, j]);
            }
        }
    }

    public static void SplitText<T>(TextAsset textAsset, char separator, out T[] array)
    {
        string[] lines = SplitLines(textAsset);
        array = new T[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(separator);
            CustomTryParse(values[1], out array[i]); // đổi cột nếu cần
        }
    }

    public static string[] SplitLines(TextAsset textAsset)
    {
        return textAsset.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static bool CustomTryParse<T>(string input, out T output)
    {
        output = default;
        try
        {
            Type type = typeof(T);
            string normalized = input.Trim().Replace(",", ".");

            if (type == typeof(float))
            {
                output = (T)(object)float.Parse(normalized, CultureInfo.InvariantCulture);
                return true;
            }
            else if (type == typeof(double))
            {
                output = (T)(object)double.Parse(normalized, CultureInfo.InvariantCulture);
                return true;
            }
            else if (type == typeof(int))
            {
                output = (T)(object)int.Parse(normalized, CultureInfo.InvariantCulture);
                return true;
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    output = (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, normalized);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CustomTryParse<{typeof(T)}> failed for input \"{input}\": {ex.Message}");
            Debug.LogError(($"<color={Color.red}>XXX</color>"));
        }

        return false;
    }
}
