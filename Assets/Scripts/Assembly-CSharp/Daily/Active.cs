using UnityEngine;

namespace Daily
{
	public class Active : DailyContent
	{
		public override Sprite GetBoosterSprite(int boosterIndex)
		{
			return UIController.instance.multiplyImages.activeBoosters[boosterIndex];
		}

		public override Sprite GetDailyPresentSprite()
		{
			return UIController.instance.multiplyImages.dailyPresent[0];
		}

		public override bool NeedToEnableHalfTransparent()
		{
			return false;
		}

		public override Color GetTextColor()
		{
			return Color.white;
		}
	}
}
