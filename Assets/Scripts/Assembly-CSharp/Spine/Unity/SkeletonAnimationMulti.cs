using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
	public class SkeletonAnimationMulti : MonoBehaviour
	{
		private const int MainTrackIndex = 0;

		public bool initialFlipX;

		public bool initialFlipY;

		public string initialAnimation;

		public bool initialLoop;

		[Space]
		public List<SkeletonDataAsset> skeletonDataAssets = new List<SkeletonDataAsset>();

		[Header("Settings")]
		public MeshGenerator.Settings meshGeneratorSettings = MeshGenerator.Settings.Default;

		private readonly List<SkeletonAnimation> skeletonAnimations = new List<SkeletonAnimation>();

		private readonly Dictionary<string, Animation> animationNameTable = new Dictionary<string, Animation>();

		private readonly Dictionary<Animation, SkeletonAnimation> animationSkeletonTable = new Dictionary<Animation, SkeletonAnimation>();

		private SkeletonAnimation currentSkeletonAnimation;

		public Dictionary<Animation, SkeletonAnimation> AnimationSkeletonTable
		{
			get
			{
				return animationSkeletonTable;
			}
		}

		public Dictionary<string, Animation> AnimationNameTable
		{
			get
			{
				return animationNameTable;
			}
		}

		public SkeletonAnimation CurrentSkeletonAnimation
		{
			get
			{
				return currentSkeletonAnimation;
			}
		}

		private void Clear()
		{
			foreach (SkeletonAnimation skeletonAnimation in skeletonAnimations)
			{
				Object.Destroy(skeletonAnimation.gameObject);
			}
			skeletonAnimations.Clear();
			animationNameTable.Clear();
			animationSkeletonTable.Clear();
		}

		private void SetActiveSkeleton(SkeletonAnimation skeletonAnimation)
		{
			foreach (SkeletonAnimation skeletonAnimation2 in skeletonAnimations)
			{
				skeletonAnimation2.gameObject.SetActive(skeletonAnimation2 == skeletonAnimation);
			}
			currentSkeletonAnimation = skeletonAnimation;
		}

		private void Awake()
		{
			Initialize(false);
		}

		public void Initialize(bool overwrite)
		{
			if (skeletonAnimations.Count != 0 && !overwrite)
			{
				return;
			}
			Clear();
			MeshGenerator.Settings meshSettings = meshGeneratorSettings;
			Transform parent = base.transform;
			foreach (SkeletonDataAsset skeletonDataAsset in skeletonDataAssets)
			{
				SkeletonAnimation skeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
				skeletonAnimation.transform.SetParent(parent, false);
				skeletonAnimation.SetMeshSettings(meshSettings);
				skeletonAnimation.initialFlipX = initialFlipX;
				skeletonAnimation.initialFlipY = initialFlipY;
				Skeleton skeleton = skeletonAnimation.skeleton;
				skeleton.FlipX = initialFlipX;
				skeleton.FlipY = initialFlipY;
				skeletonAnimation.Initialize(false);
				skeletonAnimations.Add(skeletonAnimation);
			}
			Dictionary<string, Animation> dictionary = animationNameTable;
			Dictionary<Animation, SkeletonAnimation> dictionary2 = animationSkeletonTable;
			foreach (SkeletonAnimation skeletonAnimation2 in skeletonAnimations)
			{
				foreach (Animation animation in skeletonAnimation2.Skeleton.Data.Animations)
				{
					dictionary[animation.Name] = animation;
					dictionary2[animation] = skeletonAnimation2;
				}
			}
			SetActiveSkeleton(skeletonAnimations[0]);
			SetAnimation(initialAnimation, initialLoop);
		}

		public Animation FindAnimation(string animationName)
		{
			Animation value;
			animationNameTable.TryGetValue(animationName, out value);
			return value;
		}

		public TrackEntry SetAnimation(string animationName, bool loop)
		{
			return SetAnimation(FindAnimation(animationName), loop);
		}

		public TrackEntry SetAnimation(Animation animation, bool loop)
		{
			if (animation == null)
			{
				return null;
			}
			SkeletonAnimation value;
			animationSkeletonTable.TryGetValue(animation, out value);
			if (value != null)
			{
				SetActiveSkeleton(value);
				value.skeleton.SetToSetupPose();
				return value.state.SetAnimation(0, animation, loop);
			}
			return null;
		}

		public void SetEmptyAnimation(float mixDuration)
		{
			currentSkeletonAnimation.state.SetEmptyAnimation(0, mixDuration);
		}

		public void ClearAnimation()
		{
			currentSkeletonAnimation.state.ClearTrack(0);
		}

		public TrackEntry GetCurrent()
		{
			return currentSkeletonAnimation.state.GetCurrent(0);
		}
	}
}
