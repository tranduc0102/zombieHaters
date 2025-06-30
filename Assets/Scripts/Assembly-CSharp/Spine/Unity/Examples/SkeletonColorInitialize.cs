using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SkeletonColorInitialize : MonoBehaviour
	{
		[Serializable]
		public class SlotSettings
		{
			[SpineSlot("", "", false, true, false)]
			public string slot = string.Empty;

			public Color color = Color.white;
		}

		public Color skeletonColor = Color.white;

		public List<SlotSettings> slotSettings = new List<SlotSettings>();

		private void Start()
		{
			ApplySettings();
		}

		private void ApplySettings()
		{
			ISkeletonComponent component = GetComponent<ISkeletonComponent>();
			if (component == null)
			{
				return;
			}
			Skeleton skeleton = component.Skeleton;
			skeleton.SetColor(skeletonColor);
			foreach (SlotSettings slotSetting in slotSettings)
			{
				Slot slot = skeleton.FindSlot(slotSetting.slot);
				if (slot != null)
				{
					slot.SetColor(slotSetting.color);
				}
			}
		}
	}
}
