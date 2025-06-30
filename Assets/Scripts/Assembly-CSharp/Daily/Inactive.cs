using UnityEngine;

namespace Daily
{
	public class Inactive : DailyContent
	{
		public override Sprite GetBoosterSprite(int boosterIndex)
		{
			return UIController.instance.multiplyImages.inactiveBoosters[boosterIndex];
		}

		public override Sprite GetDailyPresentSprite()
		{
			return UIController.instance.multiplyImages.dailyPresent[3];
		}

		public override bool NeedToEnableHalfTransparent()
		{
			return true;
		}

		public override Color GetTextColor()
		{
			return new Color(42f / 85f, 66f / 85f, 0.8392157f, 1f);
		}
	}
}
