using UnityEngine;

namespace Spine.Unity.Examples
{
	public class Goblins : MonoBehaviour
	{
		private SkeletonAnimation skeletonAnimation;

		private Bone headBone;

		private bool girlSkin;

		[Range(-360f, 360f)]
		public float extraRotation;

		public void Start()
		{
			skeletonAnimation = GetComponent<SkeletonAnimation>();
			headBone = skeletonAnimation.Skeleton.FindBone("head");
			skeletonAnimation.UpdateLocal += UpdateLocal;
		}

		public void UpdateLocal(ISkeletonAnimation skeletonRenderer)
		{
			headBone.Rotation += extraRotation;
		}

		public void OnMouseDown()
		{
			skeletonAnimation.Skeleton.SetSkin((!girlSkin) ? "goblingirl" : "goblin");
			skeletonAnimation.Skeleton.SetSlotsToSetupPose();
			girlSkin = !girlSkin;
			if (girlSkin)
			{
				skeletonAnimation.Skeleton.SetAttachment("right hand item", null);
				skeletonAnimation.Skeleton.SetAttachment("left hand item", "spear");
			}
			else
			{
				skeletonAnimation.Skeleton.SetAttachment("left hand item", "dagger");
			}
		}
	}
}
