using System;
using System.Collections.Generic;

namespace Spine.Unity.Modules.AnimationTools
{
	public static class AnimationTools
	{
		public static void MatchAnimationTimelines(IEnumerable<Animation> animations, SkeletonData skeletonData)
		{
			if (animations == null)
			{
				return;
			}
			if (skeletonData == null)
			{
				throw new ArgumentNullException("skeletonData", "Timelines can't be matched without a SkeletonData source.");
			}
			Dictionary<int, Timeline> dictionary = new Dictionary<int, Timeline>();
			foreach (Animation animation in animations)
			{
				foreach (Timeline timeline in animation.timelines)
				{
					if (!(timeline is EventTimeline))
					{
						int propertyId = timeline.PropertyId;
						if (!dictionary.ContainsKey(propertyId))
						{
							dictionary.Add(propertyId, GetFillerTimeline(timeline, skeletonData));
						}
					}
				}
			}
			List<int> list = new List<int>(dictionary.Keys);
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Animation animation2 in animations)
			{
				hashSet.Clear();
				foreach (Timeline timeline2 in animation2.timelines)
				{
					if (!(timeline2 is EventTimeline))
					{
						hashSet.Add(timeline2.PropertyId);
					}
				}
				ExposedList<Timeline> timelines = animation2.timelines;
				foreach (int item in list)
				{
					if (!hashSet.Contains(item))
					{
						timelines.Add(dictionary[item]);
					}
				}
			}
			dictionary.Clear();
			dictionary = null;
			list.Clear();
			list = null;
			hashSet.Clear();
			hashSet = null;
		}

		private static Timeline GetFillerTimeline(Timeline timeline, SkeletonData skeletonData)
		{
			int propertyId = timeline.PropertyId;
			switch ((TimelineType)(propertyId >> 24))
			{
			case TimelineType.Rotate:
				return GetFillerTimeline((RotateTimeline)timeline, skeletonData);
			case TimelineType.Translate:
				return GetFillerTimeline((TranslateTimeline)timeline, skeletonData);
			case TimelineType.Scale:
				return GetFillerTimeline((ScaleTimeline)timeline, skeletonData);
			case TimelineType.Shear:
				return GetFillerTimeline((ShearTimeline)timeline, skeletonData);
			case TimelineType.Attachment:
				return GetFillerTimeline((AttachmentTimeline)timeline, skeletonData);
			case TimelineType.Color:
				return GetFillerTimeline((ColorTimeline)timeline, skeletonData);
			case TimelineType.TwoColor:
				return GetFillerTimeline((TwoColorTimeline)timeline, skeletonData);
			case TimelineType.Deform:
				return GetFillerTimeline((DeformTimeline)timeline, skeletonData);
			case TimelineType.DrawOrder:
				return GetFillerTimeline((DrawOrderTimeline)timeline, skeletonData);
			case TimelineType.IkConstraint:
				return GetFillerTimeline((IkConstraintTimeline)timeline, skeletonData);
			case TimelineType.TransformConstraint:
				return GetFillerTimeline((TransformConstraintTimeline)timeline, skeletonData);
			case TimelineType.PathConstraintPosition:
				return GetFillerTimeline((PathConstraintPositionTimeline)timeline, skeletonData);
			case TimelineType.PathConstraintSpacing:
				return GetFillerTimeline((PathConstraintSpacingTimeline)timeline, skeletonData);
			case TimelineType.PathConstraintMix:
				return GetFillerTimeline((PathConstraintMixTimeline)timeline, skeletonData);
			default:
				return null;
			}
		}

		private static RotateTimeline GetFillerTimeline(RotateTimeline timeline, SkeletonData skeletonData)
		{
			RotateTimeline rotateTimeline = new RotateTimeline(1);
			rotateTimeline.boneIndex = timeline.boneIndex;
			rotateTimeline.SetFrame(0, 0f, 0f);
			return rotateTimeline;
		}

		private static TranslateTimeline GetFillerTimeline(TranslateTimeline timeline, SkeletonData skeletonData)
		{
			TranslateTimeline translateTimeline = new TranslateTimeline(1);
			translateTimeline.boneIndex = timeline.boneIndex;
			translateTimeline.SetFrame(0, 0f, 0f, 0f);
			return translateTimeline;
		}

		private static ScaleTimeline GetFillerTimeline(ScaleTimeline timeline, SkeletonData skeletonData)
		{
			ScaleTimeline scaleTimeline = new ScaleTimeline(1);
			scaleTimeline.boneIndex = timeline.boneIndex;
			scaleTimeline.SetFrame(0, 0f, 0f, 0f);
			return scaleTimeline;
		}

		private static ShearTimeline GetFillerTimeline(ShearTimeline timeline, SkeletonData skeletonData)
		{
			ShearTimeline shearTimeline = new ShearTimeline(1);
			shearTimeline.boneIndex = timeline.boneIndex;
			shearTimeline.SetFrame(0, 0f, 0f, 0f);
			return shearTimeline;
		}

		private static AttachmentTimeline GetFillerTimeline(AttachmentTimeline timeline, SkeletonData skeletonData)
		{
			AttachmentTimeline attachmentTimeline = new AttachmentTimeline(1);
			attachmentTimeline.slotIndex = timeline.slotIndex;
			SlotData slotData = skeletonData.slots.Items[attachmentTimeline.slotIndex];
			attachmentTimeline.SetFrame(0, 0f, slotData.attachmentName);
			return attachmentTimeline;
		}

		private static ColorTimeline GetFillerTimeline(ColorTimeline timeline, SkeletonData skeletonData)
		{
			ColorTimeline colorTimeline = new ColorTimeline(1);
			colorTimeline.slotIndex = timeline.slotIndex;
			SlotData slotData = skeletonData.slots.Items[colorTimeline.slotIndex];
			colorTimeline.SetFrame(0, 0f, slotData.r, slotData.g, slotData.b, slotData.a);
			return colorTimeline;
		}

		private static TwoColorTimeline GetFillerTimeline(TwoColorTimeline timeline, SkeletonData skeletonData)
		{
			TwoColorTimeline twoColorTimeline = new TwoColorTimeline(1);
			twoColorTimeline.slotIndex = timeline.slotIndex;
			SlotData slotData = skeletonData.slots.Items[twoColorTimeline.slotIndex];
			twoColorTimeline.SetFrame(0, 0f, slotData.r, slotData.g, slotData.b, slotData.a, slotData.r2, slotData.g2, slotData.b2);
			return twoColorTimeline;
		}

		private static DeformTimeline GetFillerTimeline(DeformTimeline timeline, SkeletonData skeletonData)
		{
			DeformTimeline deformTimeline = new DeformTimeline(1);
			deformTimeline.slotIndex = timeline.slotIndex;
			deformTimeline.attachment = timeline.attachment;
			SlotData slotData = skeletonData.slots.Items[deformTimeline.slotIndex];
			if (deformTimeline.attachment.IsWeighted())
			{
				deformTimeline.SetFrame(0, 0f, new float[deformTimeline.attachment.vertices.Length]);
			}
			else
			{
				deformTimeline.SetFrame(0, 0f, deformTimeline.attachment.vertices.Clone() as float[]);
			}
			return deformTimeline;
		}

		private static DrawOrderTimeline GetFillerTimeline(DrawOrderTimeline timeline, SkeletonData skeletonData)
		{
			DrawOrderTimeline drawOrderTimeline = new DrawOrderTimeline(1);
			drawOrderTimeline.SetFrame(0, 0f, null);
			return drawOrderTimeline;
		}

		private static IkConstraintTimeline GetFillerTimeline(IkConstraintTimeline timeline, SkeletonData skeletonData)
		{
			IkConstraintTimeline ikConstraintTimeline = new IkConstraintTimeline(1);
			IkConstraintData ikConstraintData = skeletonData.ikConstraints.Items[timeline.ikConstraintIndex];
			ikConstraintTimeline.SetFrame(0, 0f, ikConstraintData.mix, ikConstraintData.bendDirection);
			return ikConstraintTimeline;
		}

		private static TransformConstraintTimeline GetFillerTimeline(TransformConstraintTimeline timeline, SkeletonData skeletonData)
		{
			TransformConstraintTimeline transformConstraintTimeline = new TransformConstraintTimeline(1);
			TransformConstraintData transformConstraintData = skeletonData.transformConstraints.Items[timeline.transformConstraintIndex];
			transformConstraintTimeline.SetFrame(0, 0f, transformConstraintData.rotateMix, transformConstraintData.translateMix, transformConstraintData.scaleMix, transformConstraintData.shearMix);
			return transformConstraintTimeline;
		}

		private static PathConstraintPositionTimeline GetFillerTimeline(PathConstraintPositionTimeline timeline, SkeletonData skeletonData)
		{
			PathConstraintPositionTimeline pathConstraintPositionTimeline = new PathConstraintPositionTimeline(1);
			PathConstraintData pathConstraintData = skeletonData.pathConstraints.Items[timeline.pathConstraintIndex];
			pathConstraintPositionTimeline.SetFrame(0, 0f, pathConstraintData.position);
			return pathConstraintPositionTimeline;
		}

		private static PathConstraintSpacingTimeline GetFillerTimeline(PathConstraintSpacingTimeline timeline, SkeletonData skeletonData)
		{
			PathConstraintSpacingTimeline pathConstraintSpacingTimeline = new PathConstraintSpacingTimeline(1);
			PathConstraintData pathConstraintData = skeletonData.pathConstraints.Items[timeline.pathConstraintIndex];
			pathConstraintSpacingTimeline.SetFrame(0, 0f, pathConstraintData.spacing);
			return pathConstraintSpacingTimeline;
		}

		private static PathConstraintMixTimeline GetFillerTimeline(PathConstraintMixTimeline timeline, SkeletonData skeletonData)
		{
			PathConstraintMixTimeline pathConstraintMixTimeline = new PathConstraintMixTimeline(1);
			PathConstraintData pathConstraintData = skeletonData.pathConstraints.Items[timeline.pathConstraintIndex];
			pathConstraintMixTimeline.SetFrame(0, 0f, pathConstraintData.rotateMix, pathConstraintData.translateMix);
			return pathConstraintMixTimeline;
		}
	}
}
