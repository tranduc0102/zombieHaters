using UnityEngine;

namespace Daily
{
	public abstract class DailyContent
	{
		public abstract Sprite GetBoosterSprite(int boosterIndex);

		public abstract Sprite GetDailyPresentSprite();

		public abstract bool NeedToEnableHalfTransparent();

		public abstract Color GetTextColor();
	}
}
