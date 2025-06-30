using System;
using UnityEngine;

namespace IngameDebugConsole
{
	public class DebugLogEntry : IEquatable<DebugLogEntry>
	{
		private const int HASH_NOT_CALCULATED = -623218;

		public string logString;

		public string stackTrace;

		private string completeLog;

		public Sprite logTypeSpriteRepresentation;

		public int count;

		private int hashValue = -623218;

		public DebugLogEntry(string logString, string stackTrace, Sprite sprite)
		{
			this.logString = logString;
			this.stackTrace = stackTrace;
			logTypeSpriteRepresentation = sprite;
			count = 1;
		}

		public bool Equals(DebugLogEntry other)
		{
			return logString == other.logString && stackTrace == other.stackTrace;
		}

		public override string ToString()
		{
			if (completeLog == null)
			{
				completeLog = logString + "\n" + stackTrace;
			}
			return completeLog;
		}

		public override int GetHashCode()
		{
			if (hashValue == -623218)
			{
				hashValue = 17;
				hashValue = ((hashValue * 23 + logString != null) ? logString.GetHashCode() : 0);
				hashValue = ((hashValue * 23 + stackTrace != null) ? stackTrace.GetHashCode() : 0);
			}
			return hashValue;
		}
	}
}
