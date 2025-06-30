using System.Collections;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class MixAndMatch : MonoBehaviour
	{
		[SpineSkin("", "", true, false)]
		public string templateAttachmentsSkin = "base";

		public Material sourceMaterial;

		[Header("Visor")]
		public Sprite visorSprite;

		[SpineSlot("", "", false, true, false)]
		public string visorSlot;

		[SpineAttachment(true, false, false, "visorSlot", "", "baseSkinName", true, false)]
		public string visorKey = "goggles";

		[Header("Gun")]
		public Sprite gunSprite;

		[SpineSlot("", "", false, true, false)]
		public string gunSlot;

		[SpineAttachment(true, false, false, "gunSlot", "", "baseSkinName", true, false)]
		public string gunKey = "gun";

		[Header("Runtime Repack")]
		public bool repack = true;

		public BoundingBoxFollower bbFollower;

		[Header("Do not assign")]
		public Texture2D runtimeAtlas;

		public Material runtimeMaterial;

		private Skin customSkin;

		private void OnValidate()
		{
			if (sourceMaterial == null)
			{
				SkeletonAnimation component = GetComponent<SkeletonAnimation>();
				if (component != null)
				{
					sourceMaterial = component.SkeletonDataAsset.atlasAssets[0].materials[0];
				}
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);
			Apply();
		}

		private void Apply()
		{
			SkeletonAnimation component = GetComponent<SkeletonAnimation>();
			Skeleton skeleton = component.Skeleton;
			customSkin = customSkin ?? new Skin("custom skin");
			Skin skin = skeleton.Data.FindSkin(templateAttachmentsSkin);
			int slotIndex = skeleton.FindSlotIndex(visorSlot);
			Attachment attachment = skin.GetAttachment(slotIndex, visorKey);
			Attachment remappedClone = attachment.GetRemappedClone(visorSprite, sourceMaterial);
			customSkin.SetAttachment(slotIndex, visorKey, remappedClone);
			int slotIndex2 = skeleton.FindSlotIndex(gunSlot);
			Attachment attachment2 = skin.GetAttachment(slotIndex2, gunKey);
			Attachment remappedClone2 = attachment2.GetRemappedClone(gunSprite, sourceMaterial);
			if (remappedClone2 != null)
			{
				customSkin.SetAttachment(slotIndex2, gunKey, remappedClone2);
			}
			if (repack)
			{
				Skin skin2 = new Skin("repacked skin");
				skin2.Append(skeleton.Data.DefaultSkin);
				skin2.Append(customSkin);
				skin2 = skin2.GetRepackedSkin("repacked skin", sourceMaterial, out runtimeMaterial, out runtimeAtlas);
				skeleton.SetSkin(skin2);
				if (bbFollower != null)
				{
					bbFollower.Initialize(true);
				}
			}
			else
			{
				skeleton.SetSkin(customSkin);
			}
			skeleton.SetSlotsToSetupPose();
			component.Update(0f);
			Resources.UnloadUnusedAssets();
		}
	}
}
