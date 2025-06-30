using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Spine.Unity.Modules
{
	public class SpineEventUnityHandler : MonoBehaviour
	{
		[Serializable]
		public class EventPair
		{
			[SpineEvent("", "", true, false)]
			public string spineEvent;

			public UnityEvent unityHandler;

			public AnimationState.TrackEntryEventDelegate eventDelegate;
		}

		public List<EventPair> events = new List<EventPair>();

		private ISkeletonComponent skeletonComponent;

		private IAnimationStateComponent animationStateComponent;

		private void Start()
		{
			skeletonComponent = skeletonComponent ?? GetComponent<ISkeletonComponent>();
			if (skeletonComponent == null)
			{
				return;
			}
			animationStateComponent = animationStateComponent ?? (skeletonComponent as IAnimationStateComponent);
			if (animationStateComponent == null)
			{
				return;
			}
			Skeleton skeleton = skeletonComponent.Skeleton;
			if (skeleton == null)
			{
				return;
			}
			SkeletonData data = skeleton.Data;
			AnimationState animationState = animationStateComponent.AnimationState;
			foreach (EventPair ep in events)
			{
				EventData eventData = data.FindEvent(ep.spineEvent);
				ep.eventDelegate = ep.eventDelegate ?? ((AnimationState.TrackEntryEventDelegate)delegate(TrackEntry trackEntry, Event e)
				{
					if (e.Data == eventData)
					{
						ep.unityHandler.Invoke();
					}
				});
				animationState.Event += ep.eventDelegate;
			}
		}

		private void OnDestroy()
		{
			animationStateComponent = animationStateComponent ?? GetComponent<IAnimationStateComponent>();
			if (animationStateComponent == null)
			{
				return;
			}
			AnimationState animationState = animationStateComponent.AnimationState;
			foreach (EventPair @event in events)
			{
				if (@event.eventDelegate != null)
				{
					animationState.Event -= @event.eventDelegate;
				}
				@event.eventDelegate = null;
			}
		}
	}
}
