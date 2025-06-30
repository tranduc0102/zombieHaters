using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineBeginnerTwo : MonoBehaviour
	{
		[SpineAnimation("", "", true, false)]
		public string runAnimationName;

		[SpineAnimation("", "", true, false)]
		public string idleAnimationName;

		[SpineAnimation("", "", true, false)]
		public string walkAnimationName;

		[SpineAnimation("", "", true, false)]
		public string shootAnimationName;

		[Header("Transitions")]
		[SpineAnimation("", "", true, false)]
		public string idleTurnAnimationName;

		[SpineAnimation("", "", true, false)]
		public string runToIdleAnimationName;

		public float runWalkDuration = 1.5f;

		private SkeletonAnimation skeletonAnimation;

		public AnimationState spineAnimationState;

		public Skeleton skeleton;

		private void Start()
		{
			skeletonAnimation = GetComponent<SkeletonAnimation>();
			spineAnimationState = skeletonAnimation.AnimationState;
			skeleton = skeletonAnimation.Skeleton;
			StartCoroutine(DoDemoRoutine());
		}

		private IEnumerator DoDemoRoutine()
		{
			while (true)
			{
				spineAnimationState.SetAnimation(0, walkAnimationName, true);
				yield return new WaitForSeconds(runWalkDuration);
				spineAnimationState.SetAnimation(0, runAnimationName, true);
				yield return new WaitForSeconds(runWalkDuration);
				spineAnimationState.SetAnimation(0, runToIdleAnimationName, false);
				spineAnimationState.AddAnimation(0, idleAnimationName, true, 0f);
				yield return new WaitForSeconds(1f);
				skeleton.FlipX = true;
				spineAnimationState.SetAnimation(0, idleTurnAnimationName, false);
				spineAnimationState.AddAnimation(0, idleAnimationName, true, 0f);
				yield return new WaitForSeconds(0.5f);
				skeleton.FlipX = false;
				spineAnimationState.SetAnimation(0, idleTurnAnimationName, false);
				spineAnimationState.AddAnimation(0, idleAnimationName, true, 0f);
				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
