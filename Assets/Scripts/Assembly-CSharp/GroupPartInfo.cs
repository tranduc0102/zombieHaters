using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupPartInfo
{
	public List<int> indexes = new List<int>();

	public List<PVPSoldierSurvivor> survivors = new List<PVPSoldierSurvivor>();

	public Vector3 GetPartGroupCenter()
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < survivors.Count; i++)
		{
			if (survivors[i] != null)
			{
				zero += survivors[i].transform.position;
			}
		}
		return zero / survivors.Count;
	}
}
