using System;
using System.Collections.Generic;
using System.Linq;

public static class AbbreviationUtility
{
	private static readonly SortedDictionary<long, string> abbrevations = new SortedDictionary<long, string>
	{
		{ 1000L, "K" },
		{ 1000000L, "M" },
		{ 1000000000L, "B" },
		{ 1000000000000L, "T" }
	};

	public static string AbbreviateNumber(double number, int decimalPlaces = 3)
	{
		for (int num = abbrevations.Count - 1; num >= 0; num--)
		{
			KeyValuePair<long, string> keyValuePair = abbrevations.ElementAt(num);
			if (Math.Abs(number) >= (double)keyValuePair.Key)
			{
				return RoundDown(number / (double)keyValuePair.Key, decimalPlaces) + keyValuePair.Value;
			}
		}
		return RoundDown(number, 0).ToString();
	}

	public static double RoundDown(double number, int decimalPlaces)
	{
		return Math.Floor(number * Math.Pow(10.0, decimalPlaces)) / Math.Pow(10.0, decimalPlaces);
	}
}
