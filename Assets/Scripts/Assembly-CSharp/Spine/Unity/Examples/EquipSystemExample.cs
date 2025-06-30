using System;
using System.Collections.Generic;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class EquipSystemExample : MonoBehaviour, IHasSkeletonDataAsset
	{
		[Serializable]
		public class EquipHook
		{
			public EquipType type;

			[SpineSlot("", "", false, true, false)]
			public string slot;

			[SpineSkin("", "", true, false)]
			public string templateSkin;

			[SpineAttachment(true, false, false, "", "", "templateSkin", true, false)]
			public string templateAttachment;
		}

		public enum EquipType
		{
			Gun = 0,
			Goggles = 1
		}

		public SkeletonDataAsset skeletonDataAsset;

		public Material sourceMaterial;

		public bool applyPMA = true;

		public List<EquipHook> equippables = new List<EquipHook>();

		public EquipsVisualsComponentExample target;

		public Dictionary<EquipAssetExample, Attachment> cachedAttachments = new Dictionary<EquipAssetExample, Attachment>();

		SkeletonDataAsset IHasSkeletonDataAsset.SkeletonDataAsset
		{
			get
			{
				return skeletonDataAsset;
			}
		}

		public void Equip(EquipAssetExample asset)
		{
			EquipType equipType = asset.equipType;
			EquipHook equipHook = equippables.Find((EquipHook x) => x.type == equipType);
			SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
			int slotIndex = skeletonData.FindSlotIndex(equipHook.slot);
			Attachment attachment = GenerateAttachmentFromEquipAsset(asset, slotIndex, equipHook.templateSkin, equipHook.templateAttachment);
			target.Equip(slotIndex, equipHook.templateAttachment, attachment);
		}

		private Attachment GenerateAttachmentFromEquipAsset(EquipAssetExample asset, int slotIndex, string templateSkinName, string templateAttachmentName)
		{
			Attachment value;
			cachedAttachments.TryGetValue(asset, out value);
			if (value == null)
			{
				SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
				Skin skin = skeletonData.FindSkin(templateSkinName);
				Attachment attachment = skin.GetAttachment(slotIndex, templateAttachmentName);
				value = attachment.GetRemappedClone(asset.sprite, sourceMaterial, applyPMA);
				cachedAttachments.Add(asset, value);
			}
			return value;
		}

		public void Done()
		{
			target.OptimizeSkin();
		}
	}
}
