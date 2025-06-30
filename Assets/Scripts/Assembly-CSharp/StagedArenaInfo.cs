using System.Collections.Generic;

public class StagedArenaInfo
{
	public int arenaIndex;

	public List<ArenaInfo> stages;

	public StagedArenaInfo(int arenaIndex, ArenaInfo stage)
	{
		this.arenaIndex = arenaIndex;
		stages = new List<ArenaInfo>();
		stages.Add(stage);
	}
}
