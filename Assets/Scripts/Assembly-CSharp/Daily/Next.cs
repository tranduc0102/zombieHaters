using UnityEngine;

namespace Daily
{
	public class Next : DailyContent
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
			return true;
		}

		public override Color GetTextColor()
		{
			return Color.white;
		}
	}
}
