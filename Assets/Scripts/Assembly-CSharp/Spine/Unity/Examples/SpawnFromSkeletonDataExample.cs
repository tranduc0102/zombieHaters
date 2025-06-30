using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpawnFromSkeletonDataExample : MonoBehaviour
	{
		public SkeletonDataAsset skeletonDataAsset;

		[Range(0f, 100f)]
		public int count = 20;

		[SpineAnimation("", "skeletonDataAsset", true, false)]
		public string startingAnimation;

		private IEnumerator Start()
		{
			if (!(skeletonDataAsset == null))
			{
				skeletonDataAsset.GetSkeletonData(false);
				yield return new WaitForSeconds(1f);
				Animation spineAnimation = skeletonDataAsset.GetSkeletonData(false).FindAnimation(startingAnimation);
				for (int i = 0; i < count; i++)
				{
					SkeletonAnimation sa = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
					DoExtraStuff(sa, spineAnimation);
					sa.gameObject.name = i.ToString();
					yield return new WaitForSeconds(0.125f);
				}
			}
		}

		private void DoExtraStuff(SkeletonAnimation sa, Animation spineAnimation)
		{
			sa.transform.localPosition = Random.insideUnitCircle * 6f;
			sa.transform.SetParent(base.transform, false);
			if (spineAnimation != null)
			{
				sa.Initialize(false);
				sa.AnimationState.SetAnimation(0, spineAnimation, true);
			}
		}
	}
}
