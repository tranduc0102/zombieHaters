using UnityEngine;
using UnityEngine.Playables;

namespace Spine.Unity.Playables
{
	public class SpineAnimationStateMixerBehaviour : PlayableBehaviour
	{
		private float[] lastInputWeights;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			SkeletonAnimation skeletonAnimation = playerData as SkeletonAnimation;
			if (skeletonAnimation == null)
			{
				return;
			}
			Skeleton skeleton = skeletonAnimation.Skeleton;
			AnimationState animationState = skeletonAnimation.AnimationState;
			if (!Application.isPlaying)
			{
				PreviewEditModePose(playable, skeletonAnimation);
				return;
			}
			int inputCount = playable.GetInputCount();
			if (lastInputWeights == null || lastInputWeights.Length < inputCount)
			{
				lastInputWeights = new float[inputCount];
				for (int i = 0; i < inputCount; i++)
				{
					lastInputWeights[i] = 0f;
				}
			}
			float[] array = lastInputWeights;
			for (int j = 0; j < inputCount; j++)
			{
				float num = array[j];
				float inputWeight = playable.GetInputWeight(j);
				bool flag = inputWeight > num;
				array[j] = inputWeight;
				if (!flag)
				{
					continue;
				}
				SpineAnimationStateBehaviour behaviour = ((ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(j)).GetBehaviour();
				if (behaviour.animationReference == null)
				{
					float mixDuration = ((!behaviour.customDuration) ? animationState.Data.DefaultMix : behaviour.mixDuration);
					animationState.SetEmptyAnimation(0, mixDuration);
				}
				else if (behaviour.animationReference.Animation != null)
				{
					TrackEntry trackEntry = animationState.SetAnimation(0, behaviour.animationReference.Animation, behaviour.loop);
					trackEntry.EventThreshold = behaviour.eventThreshold;
					trackEntry.DrawOrderThreshold = behaviour.drawOrderThreshold;
					trackEntry.AttachmentThreshold = behaviour.attachmentThreshold;
					if (behaviour.customDuration)
					{
						trackEntry.MixDuration = behaviour.mixDuration;
					}
				}
				skeletonAnimation.Update(0f);
				skeletonAnimation.LateUpdate();
			}
		}

		public void PreviewEditModePose(Playable playable, SkeletonAnimation spineComponent)
		{
			if (Application.isPlaying || spineComponent == null)
			{
				return;
			}
			int inputCount = playable.GetInputCount();
			int num = -1;
			for (int i = 0; i < inputCount; i++)
			{
				float inputWeight = playable.GetInputWeight(i);
				if (inputWeight >= 1f)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				ScriptPlayable<SpineAnimationStateBehaviour> playable2 = (ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(num);
				SpineAnimationStateBehaviour behaviour = playable2.GetBehaviour();
				Skeleton skeleton = spineComponent.Skeleton;
				if (behaviour.animationReference != null && spineComponent.SkeletonDataAsset.GetSkeletonData(true) != behaviour.animationReference.SkeletonDataAsset.GetSkeletonData(true))
				{
					Debug.LogWarningFormat("SpineAnimationStateMixerBehaviour tried to apply an animation for the wrong skeleton. Expected {0}. Was {1}", spineComponent.SkeletonDataAsset, behaviour.animationReference.SkeletonDataAsset);
				}
				Animation animation = null;
				float time = 0f;
				bool loop = false;
				if (num != 0 && inputCount > 1)
				{
					ScriptPlayable<SpineAnimationStateBehaviour> playable3 = (ScriptPlayable<SpineAnimationStateBehaviour>)playable.GetInput(num - 1);
					SpineAnimationStateBehaviour behaviour2 = playable3.GetBehaviour();
					animation = behaviour2.animationReference.Animation;
					time = (float)playable3.GetTime();
					loop = behaviour2.loop;
				}
				Animation animation2 = behaviour.animationReference.Animation;
				float num2 = (float)playable2.GetTime();
				float num3 = behaviour.mixDuration;
				if (!behaviour.customDuration && animation != null)
				{
					num3 = spineComponent.AnimationState.Data.GetMix(animation, animation2);
				}
				if (animation != null && num3 > 0f && num2 < num3)
				{
					skeleton.SetToSetupPose();
					float num4 = 1f - num2 / num3;
					num4 = ((!(num4 > 0.5f)) ? (num4 * 2f) : 1f);
					animation.Apply(skeleton, 0f, time, loop, null, num4, MixPose.Setup, MixDirection.Out);
					animation2.Apply(skeleton, 0f, num2, behaviour.loop, null, num2 / num3, MixPose.Current, MixDirection.In);
				}
				else
				{
					skeleton.SetToSetupPose();
					animation2.PoseSkeleton(skeleton, num2, behaviour.loop);
				}
			}
		}
	}
}
