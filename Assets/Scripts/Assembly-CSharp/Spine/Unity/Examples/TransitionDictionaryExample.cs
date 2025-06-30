using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public sealed class TransitionDictionaryExample : MonoBehaviour
	{
		[Serializable]
		public struct SerializedEntry
		{
			public AnimationReferenceAsset from;

			public AnimationReferenceAsset to;

			public AnimationReferenceAsset transition;
		}

		[SerializeField]
		private List<SerializedEntry> transitions = new List<SerializedEntry>();

		private readonly Dictionary<AnimationStateData.AnimationPair, Animation> dictionary = new Dictionary<AnimationStateData.AnimationPair, Animation>();

		private void Start()
		{
			dictionary.Clear();
			foreach (SerializedEntry transition in transitions)
			{
				dictionary.Add(new AnimationStateData.AnimationPair(transition.from.Animation, transition.to.Animation), transition.transition.Animation);
			}
		}

		public Animation GetTransition(Animation from, Animation to)
		{
			Animation value;
			dictionary.TryGetValue(new AnimationStateData.AnimationPair(from, to), out value);
			return value;
		}
	}
}
