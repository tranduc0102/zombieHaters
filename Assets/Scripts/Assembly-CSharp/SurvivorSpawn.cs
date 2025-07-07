using UnityEngine;

public class SurvivorSpawn : PointOnMap
{
	[HideInInspector]
	public NewSurvivor newSurvivor;

	private void Start()
	{
		SpawnManager.instance.survivorSpawns.Add(this);
	}

	public void Spawn()
	{
		newSurvivor = Object.Instantiate(SpawnManager.instance.GetNewSurvivor(), base.transform.position, default(Quaternion));
    }

    public bool isReady()
	{
		if (newSurvivor == null && openAtLevel <= DataLoader.Instance.GetCurrentPlayerLevel() && worldNumber == GameManager.instance.currentWorldNumber)
		{
			return true;
		}
		return false;
	}
}
