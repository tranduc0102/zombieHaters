using System.Collections.Generic;
using Spine.Unity.Modules.AttachmentTools;
using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SpriteAttacher : MonoBehaviour
	{
		public const string DefaultPMAShader = "Spine/Skeleton";

		public const string DefaultStraightAlphaShader = "Sprites/Default";

		public bool attachOnStart = true;

		public bool overrideAnimation = true;

		public Sprite sprite;

		[SpineSlot("", "", false, true, false)]
		public string slot;

		private RegionAttachment attachment;

		private Slot spineSlot;

		private bool applyPMA;

		private static Dictionary<Texture, AtlasPage> atlasPageCache;

		private static AtlasPage GetPageFor(Texture texture, Shader shader)
		{
			if (atlasPageCache == null)
			{
				atlasPageCache = new Dictionary<Texture, AtlasPage>();
			}
			AtlasPage value;
			atlasPageCache.TryGetValue(texture, out value);
			if (value == null)
			{
				Material m = new Material(shader);
				value = m.ToSpineAtlasPage();
				atlasPageCache[texture] = value;
			}
			return value;
		}

		private void Start()
		{
			Initialize(false);
			if (attachOnStart)
			{
				Attach();
			}
		}

		private void AnimationOverrideSpriteAttach(ISkeletonAnimation animated)
		{
			if (overrideAnimation && base.isActiveAndEnabled)
			{
				Attach();
			}
		}

		public void Initialize(bool overwrite = true)
		{
			if (!overwrite && attachment != null)
			{
				return;
			}
			ISkeletonComponent component = GetComponent<ISkeletonComponent>();
			SkeletonRenderer skeletonRenderer = component as SkeletonRenderer;
			if (skeletonRenderer != null)
			{
				applyPMA = skeletonRenderer.pmaVertexColors;
			}
			else
			{
				SkeletonGraphic skeletonGraphic = component as SkeletonGraphic;
				if (skeletonGraphic != null)
				{
					applyPMA = skeletonGraphic.MeshGenerator.settings.pmaVertexColors;
				}
			}
			if (overrideAnimation)
			{
				ISkeletonAnimation skeletonAnimation = component as ISkeletonAnimation;
				if (skeletonAnimation != null)
				{
					skeletonAnimation.UpdateComplete -= AnimationOverrideSpriteAttach;
					skeletonAnimation.UpdateComplete += AnimationOverrideSpriteAttach;
				}
			}
			spineSlot = spineSlot ?? component.Skeleton.FindSlot(slot);
			Shader shader = ((!applyPMA) ? Shader.Find("Sprites/Default") : Shader.Find("Spine/Skeleton"));
			attachment = ((!applyPMA) ? sprite.ToRegionAttachment(GetPageFor(sprite.texture, shader)) : sprite.ToRegionAttachmentPMAClone(shader));
		}

		private void OnDestroy()
		{
			ISkeletonAnimation component = GetComponent<ISkeletonAnimation>();
			if (component != null)
			{
				component.UpdateComplete -= AnimationOverrideSpriteAttach;
			}
		}

		public void Attach()
		{
			if (spineSlot != null)
			{
				spineSlot.Attachment = attachment;
			}
		}
	}
}
