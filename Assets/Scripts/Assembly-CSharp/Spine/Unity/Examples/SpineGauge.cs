using UnityEngine;

namespace Spine.Unity.Examples
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SkeletonRenderer))]
	public class SpineGauge : MonoBehaviour
	{
		[Range(0f, 1f)]
		public float fillPercent;

		public AnimationReferenceAsset fillAnimation;

		private SkeletonRenderer skeletonRenderer;

		private void Awake()
		{
			skeletonRenderer = GetComponent<SkeletonRenderer>();
		}

		private void Update()
		{
			SetGaugePercent(fillPercent);
		}

		public void SetGaugePercent(float percent)
		{
			if (!(skeletonRenderer == null))
			{
				Skeleton skeleton = skeletonRenderer.skeleton;
				if (skeleton != null)
				{
					fillAnimation.Animation.Apply(skeleton, 0f, percent, false, null, 1f, MixPose.Setup, MixDirection.In);
					skeleton.Update(Time.deltaTime);
					skeleton.UpdateWorldTransform();
				}
			}
		}
	}
}
