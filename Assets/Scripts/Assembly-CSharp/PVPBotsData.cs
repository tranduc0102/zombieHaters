using System.Collections.Generic;
using System.Linq;

public class PVPBotsData
{
	public class BotData
	{
		public SaveData.HeroData.HeroType heroType;

		public List<int> levels = new List<int>();

		public bool IsOpened(int arenaIndex)
		{
			return levels[arenaIndex] != 0;
		}
	}

	public List<int> arenaRating = new List<int>();

	public List<float> displayedPower = new List<float>();

	public List<BotData> botData = new List<BotData>();

	public int GetBotLevel(SaveData.HeroData.HeroType heroType, int arenaIndex)
	{
		return botData.First((BotData bd) => bd.heroType == heroType).levels[arenaIndex];
	}
}
