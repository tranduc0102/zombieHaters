using System;

namespace IngameDebugConsole
{
	public class DebugLogIndexList
	{
		private int[] indices;

		private int size;

		public int Count
		{
			get
			{
				return size;
			}
		}

		public int this[int index]
		{
			get
			{
				return indices[index];
			}
		}

		public DebugLogIndexList()
		{
			indices = new int[64];
			size = 0;
		}

		public void Add(int index)
		{
			if (size == indices.Length)
			{
				int[] destinationArray = new int[size * 2];
				Array.Copy(indices, 0, destinationArray, 0, size);
				indices = destinationArray;
			}
			indices[size++] = index;
		}

		public void Clear()
		{
			size = 0;
		}
	}
}
