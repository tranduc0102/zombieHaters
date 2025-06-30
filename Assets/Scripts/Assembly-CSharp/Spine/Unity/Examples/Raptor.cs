using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class Raptor : MonoBehaviour
	{
		public AnimationReferenceAsset walk;

		public AnimationReferenceAsset gungrab;

		public AnimationReferenceAsset gunkeep;

		private SkeletonAnimation skeletonAnimation;

		private void Start()
		{
			skeletonAnimation = GetComponent<SkeletonAnimation>();
			StartCoroutine(GunGrabRoutine());
		}

		private IEnumerator GunGrabRoutine()
		{
			skeletonAnimation.AnimationState.SetAnimation(0, walk, true);
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(0.5f, 3f));
				skeletonAnimation.AnimationState.SetAnimation(1, gungrab, false);
				yield return new WaitForSeconds(Random.Range(0.5f, 3f));
				skeletonAnimation.AnimationState.SetAnimation(1, gunkeep, false);
			}
		}
	}
}
