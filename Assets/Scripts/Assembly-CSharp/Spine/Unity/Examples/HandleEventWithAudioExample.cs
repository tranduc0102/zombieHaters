using UnityEngine;

namespace Spine.Unity.Examples
{
	public class HandleEventWithAudioExample : MonoBehaviour
	{
		public SkeletonAnimation skeletonAnimation;

		[SpineEvent("", "skeletonAnimation", true, true)]
		public string eventName;

		[Space]
		public AudioSource audioSource;

		public AudioClip audioClip;

		public float basePitch = 1f;

		public float randomPitchOffset = 0.1f;

		[Space]
		public bool logDebugMessage;

		private EventData eventData;

		private void OnValidate()
		{
			if (skeletonAnimation == null)
			{
				GetComponent<SkeletonAnimation>();
			}
			if (audioSource == null)
			{
				GetComponent<AudioSource>();
			}
		}

		private void Start()
		{
			if (!(audioSource == null) && !(skeletonAnimation == null))
			{
				skeletonAnimation.Initialize(false);
				if (skeletonAnimation.valid)
				{
					eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
					skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
				}
			}
		}

		private void HandleAnimationStateEvent(TrackEntry trackEntry, Event e)
		{
			if (logDebugMessage)
			{
				Debug.Log("Event fired! " + e.Data.Name);
			}
			if (eventData == e.Data)
			{
				Play();
			}
		}

		public void Play()
		{
			audioSource.pitch = basePitch + Random.Range(0f - randomPitchOffset, randomPitchOffset);
			audioSource.clip = audioClip;
			audioSource.Play();
		}
	}
}
