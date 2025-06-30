using System;
using UnityEngine;

public class CanistersManager : MonoBehaviour
{
	[Serializable]
	private struct CanisterSet
	{
		public FuelCanister[] fuelCanisters;
	}

	[SerializeField]
	private CanisterSet[] canisterSets;

	private void Awake()
	{
		int num = UnityEngine.Random.Range(0, canisterSets.Length);
		FuelCanister[] fuelCanisters = canisterSets[num].fuelCanisters;
		foreach (FuelCanister fuelCanister in fuelCanisters)
		{
			fuelCanister.gameObject.SetActive(true);
		}
	}
}
