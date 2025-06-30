using UnityEngine;

public class FuelPointer : BasePointer
{
	private FuelCanister[] canisters;

	private new void Start()
	{
		base.Start();
		pointerImage.gameObject.SetActive(false);
		distanceText.enabled = false;
		base.enabled = false;
	}

	public void StartPointer()
	{
		base.enabled = true;
		canisters = Object.FindObjectsOfType<FuelCanister>();
	}

	private new void FixedUpdate()
	{
		targets.Clear();
		FuelCanister[] array = canisters;
		foreach (FuelCanister fuelCanister in array)
		{
			if (fuelCanister != null)
			{
				targets.Add(fuelCanister.transform.position);
			}
		}
		if (targets.Count <= 0)
		{
			base.enabled = false;
		}
		base.FixedUpdate();
	}
}
