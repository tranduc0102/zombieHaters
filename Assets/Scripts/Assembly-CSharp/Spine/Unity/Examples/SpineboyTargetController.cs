using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineboyTargetController : MonoBehaviour
	{
		public SkeletonAnimation skeletonAnimation;

		[SpineBone("", "skeletonAnimation", true, false)]
		public string boneName;

		public Camera camera;

		private Bone bone;

		private void OnValidate()
		{
			if (skeletonAnimation == null)
			{
				skeletonAnimation = GetComponent<SkeletonAnimation>();
			}
		}

		private void Start()
		{
			bone = skeletonAnimation.Skeleton.FindBone(boneName);
		}

		private void Update()
		{
			Vector3 mousePosition = Input.mousePosition;
			Vector3 position = camera.ScreenToWorldPoint(mousePosition);
			Vector3 position2 = skeletonAnimation.transform.InverseTransformPoint(position);
			if (skeletonAnimation.Skeleton.FlipX)
			{
				position2.x *= -1f;
			}
			bone.SetPosition(position2);
		}
	}
}
