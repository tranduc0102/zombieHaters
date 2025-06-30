using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class EquipsVisualsComponentExample : MonoBehaviour
	{
		public SkeletonAnimation skeletonAnimation;

		[SpineSkin("", "", true, false)]
		public string templateSkinName;

		private Skin equipsSkin;

		private Skin collectedSkin;

		public Material runtimeMaterial;

		public Texture2D runtimeAtlas;

		private void Start()
		{
			equipsSkin = new Skin("Equips");
			Skin skin = skeletonAnimation.Skeleton.Data.FindSkin(templateSkinName);
			if (skin != null)
			{
				equipsSkin.Append(skin);
			}
			skeletonAnimation.Skeleton.Skin = equipsSkin;
			RefreshSkeletonAttachments();
		}

		public void Equip(int slotIndex, string attachmentName, Attachment attachment)
		{
			equipsSkin.AddAttachment(slotIndex, attachmentName, attachment);
			skeletonAnimation.Skeleton.SetSkin(equipsSkin);
			RefreshSkeletonAttachments();
		}

		public void OptimizeSkin()
		{
			collectedSkin = collectedSkin ?? new Skin("Collected skin");
			collectedSkin.Clear();
			collectedSkin.Append(skeletonAnimation.Skeleton.Data.DefaultSkin);
			collectedSkin.Append(equipsSkin);
			Skin repackedSkin = collectedSkin.GetRepackedSkin("Repacked skin", skeletonAnimation.SkeletonDataAsset.atlasAssets[0].materials[0], out runtimeMaterial, out runtimeAtlas);
			collectedSkin.Clear();
			skeletonAnimation.Skeleton.Skin = repackedSkin;
			RefreshSkeletonAttachments();
		}

		private void RefreshSkeletonAttachments()
		{
			skeletonAnimation.Skeleton.SetSlotsToSetupPose();
			skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);
		}
	}
}
