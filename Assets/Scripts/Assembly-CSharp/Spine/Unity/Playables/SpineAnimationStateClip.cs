using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Spine.Unity.Playables
{
	[Serializable]
	public class SpineAnimationStateClip : PlayableAsset, ITimelineClipAsset
	{
		public SpineAnimationStateBehaviour template = new SpineAnimationStateBehaviour();

		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.None;
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<SpineAnimationStateBehaviour> scriptPlayable = ScriptPlayable<SpineAnimationStateBehaviour>.Create(graph, template);
			scriptPlayable.GetBehaviour();
			return scriptPlayable;
		}
	}
}
