using UnityEngine;

namespace Spine.Unity.Playables
{
	[AddComponentMenu("Spine/Playables/SkeletonAnimation Playable Handle (Playables)")]
	public class SkeletonAnimationPlayableHandle : SpinePlayableHandleBase
	{
		public SkeletonAnimation skeletonAnimation;

		public override Skeleton Skeleton
		{
			get
			{
				return skeletonAnimation.Skeleton;
			}
		}

		public override SkeletonData SkeletonData
		{
			get
			{
				return skeletonAnimation.Skeleton.data;
			}
		}

		private void Awake()
		{
			if (skeletonAnimation == null)
			{
				skeletonAnimation = GetComponent<SkeletonAnimation>();
			}
		}
	}
}
