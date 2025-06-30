using UnityEngine;

namespace Spine.Unity.Modules
{
	public class SkeletonGraphicMirror : MonoBehaviour
	{
		public SkeletonRenderer source;

		public bool mirrorOnStart = true;

		public bool restoreOnDisable = true;

		private SkeletonGraphic skeletonGraphic;

		private Skeleton originalSkeleton;

		private bool originalFreeze;

		private Texture2D overrideTexture;

		private void Awake()
		{
			skeletonGraphic = GetComponent<SkeletonGraphic>();
		}

		private void Start()
		{
			if (mirrorOnStart)
			{
				StartMirroring();
			}
		}

		private void LateUpdate()
		{
			skeletonGraphic.UpdateMesh();
		}

		private void OnDisable()
		{
			if (restoreOnDisable)
			{
				RestoreIndependentSkeleton();
			}
		}

		public void StartMirroring()
		{
			if (!(source == null) && !(skeletonGraphic == null))
			{
				skeletonGraphic.startingAnimation = string.Empty;
				if (originalSkeleton == null)
				{
					originalSkeleton = skeletonGraphic.Skeleton;
					originalFreeze = skeletonGraphic.freeze;
				}
				skeletonGraphic.Skeleton = source.skeleton;
				skeletonGraphic.freeze = true;
				if (overrideTexture != null)
				{
					skeletonGraphic.OverrideTexture = overrideTexture;
				}
			}
		}

		public void UpdateTexture(Texture2D newOverrideTexture)
		{
			overrideTexture = newOverrideTexture;
			if (newOverrideTexture != null)
			{
				skeletonGraphic.OverrideTexture = overrideTexture;
			}
		}

		public void RestoreIndependentSkeleton()
		{
			if (originalSkeleton != null)
			{
				skeletonGraphic.Skeleton = originalSkeleton;
				skeletonGraphic.freeze = originalFreeze;
				skeletonGraphic.OverrideTexture = null;
				originalSkeleton = null;
			}
		}
	}
}
