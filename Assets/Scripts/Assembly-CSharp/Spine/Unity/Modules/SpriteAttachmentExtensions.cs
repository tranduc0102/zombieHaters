using System;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public static class SpriteAttachmentExtensions
	{
		[Obsolete]
		public static RegionAttachment AttachUnitySprite(this Skeleton skeleton, string slotName, Sprite sprite, string shaderName = "Spine/Skeleton", bool applyPMA = true, float rotation = 0f)
		{
			return skeleton.AttachUnitySprite(slotName, sprite, Shader.Find(shaderName), applyPMA, rotation);
		}

		[Obsolete]
		public static RegionAttachment AddUnitySprite(this SkeletonData skeletonData, string slotName, Sprite sprite, string skinName = "", string shaderName = "Spine/Skeleton", bool applyPMA = true, float rotation = 0f)
		{
			return skeletonData.AddUnitySprite(slotName, sprite, skinName, Shader.Find(shaderName), applyPMA, rotation);
		}

		[Obsolete]
		public static RegionAttachment AttachUnitySprite(this Skeleton skeleton, string slotName, Sprite sprite, Shader shader, bool applyPMA, float rotation = 0f)
		{
			RegionAttachment regionAttachment;
			if (applyPMA)
			{
				regionAttachment = sprite.ToRegionAttachmentPMAClone(shader, TextureFormat.RGBA32, false, null, rotation);
			}
			else
			{
				regionAttachment = sprite.ToRegionAttachment(new Material(shader), rotation);
			}
			RegionAttachment regionAttachment2 = regionAttachment;
			skeleton.FindSlot(slotName).Attachment = regionAttachment2;
			return regionAttachment2;
		}

		[Obsolete]
		public static RegionAttachment AddUnitySprite(this SkeletonData skeletonData, string slotName, Sprite sprite, string skinName, Shader shader, bool applyPMA, float rotation = 0f)
		{
			RegionAttachment regionAttachment;
			if (applyPMA)
			{
				regionAttachment = sprite.ToRegionAttachmentPMAClone(shader, TextureFormat.RGBA32, false, null, rotation);
			}
			else
			{
				regionAttachment = sprite.ToRegionAttachment(new Material(shader), rotation);
			}
			RegionAttachment regionAttachment2 = regionAttachment;
			int slotIndex = skeletonData.FindSlotIndex(slotName);
			Skin skin = skeletonData.DefaultSkin;
			if (skinName != string.Empty)
			{
				skin = skeletonData.FindSkin(skinName);
			}
			skin.AddAttachment(slotIndex, regionAttachment2.Name, regionAttachment2);
			return regionAttachment2;
		}
	}
}
