using UnityEngine;
using UnityEngine.Playables;

namespace Spine.Unity.Playables
{
	public class SpineSkeletonFlipMixerBehaviour : PlayableBehaviour
	{
		private bool defaultFlipX;

		private bool defaultFlipY;

		private SpinePlayableHandleBase playableHandle;

		private bool m_FirstFrameHappened;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			playableHandle = playerData as SpinePlayableHandleBase;
			if (playableHandle == null)
			{
				return;
			}
			Skeleton skeleton = playableHandle.Skeleton;
			if (!m_FirstFrameHappened)
			{
				defaultFlipX = skeleton.flipX;
				defaultFlipY = skeleton.flipY;
				m_FirstFrameHappened = true;
			}
			int inputCount = playable.GetInputCount();
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				SpineSkeletonFlipBehaviour behaviour = ((ScriptPlayable<SpineSkeletonFlipBehaviour>)playable.GetInput(i)).GetBehaviour();
				num += inputWeight;
				if (inputWeight > num2)
				{
					skeleton.flipX = behaviour.flipX;
					skeleton.flipY = behaviour.flipY;
					num2 = inputWeight;
				}
				if (!Mathf.Approximately(inputWeight, 0f))
				{
					num3++;
				}
			}
			if (num3 != 1 && 1f - num > num2)
			{
				skeleton.flipX = defaultFlipX;
				skeleton.flipY = defaultFlipY;
			}
		}

		public override void OnGraphStop(Playable playable)
		{
			m_FirstFrameHappened = false;
			if (!(playableHandle == null))
			{
				Skeleton skeleton = playableHandle.Skeleton;
				skeleton.flipX = defaultFlipX;
				skeleton.flipY = defaultFlipY;
			}
		}
	}
}
