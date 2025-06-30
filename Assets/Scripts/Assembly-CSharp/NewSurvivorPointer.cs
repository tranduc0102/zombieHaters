public class NewSurvivorPointer : BasePointer
{
	private new void FixedUpdate()
	{
		targets.Clear();
		foreach (SurvivorSpawn survivorSpawn in SpawnManager.instance.survivorSpawns)
		{
			if (survivorSpawn.newSurvivor != null)
			{
				targets.Add(survivorSpawn.transform.position);
			}
		}
		base.FixedUpdate();
	}
}
