using System.Collections;
using Spine.Unity.Modules;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineboyPole : MonoBehaviour
	{
		public SkeletonAnimation skeletonAnimation;

		public SkeletonRenderSeparator separator;

		[Space(18f)]
		public AnimationReferenceAsset run;

		public AnimationReferenceAsset pole;

		public float startX;

		public float endX;

		private const float Speed = 18f;

		private const float RunTimeScale = 1.5f;

		private IEnumerator Start()
		{
			AnimationState state = skeletonAnimation.state;
			while (true)
			{
				SetXPosition(startX);
				separator.enabled = false;
				state.SetAnimation(0, run, true);
				state.TimeScale = 1.5f;
				while (base.transform.localPosition.x < endX)
				{
					base.transform.Translate(Vector3.right * 18f * Time.deltaTime);
					yield return null;
				}
				SetXPosition(endX);
				separator.enabled = true;
				TrackEntry poleTrack = state.SetAnimation(0, pole, false);
				yield return new WaitForSpineAnimationComplete(poleTrack);
				yield return new WaitForSeconds(1f);
			}
		}

		private void SetXPosition(float x)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = x;
			base.transform.localPosition = localPosition;
		}
	}
}
