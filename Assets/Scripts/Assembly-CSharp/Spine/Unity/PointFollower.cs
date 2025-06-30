using System;
using UnityEngine;

namespace Spine.Unity
{
	[ExecuteInEditMode]
	[AddComponentMenu("Spine/Point Follower")]
	public class PointFollower : MonoBehaviour, IHasSkeletonRenderer, IHasSkeletonComponent
	{
		[SerializeField]
		public SkeletonRenderer skeletonRenderer;

		[SpineSlot("", "skeletonRenderer", false, true, false)]
		public string slotName;

		[SpineAttachment(true, false, false, "slotName", "skeletonRenderer", "", true, true)]
		public string pointAttachmentName;

		public bool followRotation = true;

		public bool followSkeletonFlip = true;

		public bool followSkeletonZPosition;

		private Transform skeletonTransform;

		private bool skeletonTransformIsParent;

		private PointAttachment point;

		private Bone bone;

		private bool valid;

		public SkeletonRenderer SkeletonRenderer
		{
			get
			{
				return skeletonRenderer;
			}
		}

		public ISkeletonComponent SkeletonComponent
		{
			get
			{
				return skeletonRenderer;
			}
		}

		public bool IsValid
		{
			get
			{
				return valid;
			}
		}

		public void Initialize()
		{
			valid = skeletonRenderer != null && skeletonRenderer.valid;
			if (valid)
			{
				UpdateReferences();
			}
		}

		private void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer)
		{
			Initialize();
		}

		private void UpdateReferences()
		{
			skeletonTransform = skeletonRenderer.transform;
			skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
			skeletonRenderer.OnRebuild += HandleRebuildRenderer;
			skeletonTransformIsParent = object.ReferenceEquals(skeletonTransform, base.transform.parent);
			bone = null;
			point = null;
			if (!string.IsNullOrEmpty(pointAttachmentName))
			{
				Skeleton skeleton = skeletonRenderer.skeleton;
				int num = skeleton.FindSlotIndex(slotName);
				if (num >= 0)
				{
					Slot slot = skeleton.slots.Items[num];
					bone = slot.bone;
					point = skeleton.GetAttachment(num, pointAttachmentName) as PointAttachment;
				}
			}
		}

		public void LateUpdate()
		{
			if (point == null)
			{
				if (string.IsNullOrEmpty(pointAttachmentName))
				{
					return;
				}
				UpdateReferences();
				if (point == null)
				{
					return;
				}
			}
			Vector2 vector = default(Vector2);
			point.ComputeWorldPosition(bone, out vector.x, out vector.y);
			float num = point.ComputeWorldRotation(bone);
			Transform transform = base.transform;
			if (skeletonTransformIsParent)
			{
				transform.localPosition = new Vector3(vector.x, vector.y, (!followSkeletonZPosition) ? transform.localPosition.z : 0f);
				if (followRotation)
				{
					float f = num * 0.5f * ((float)Math.PI / 180f);
					Quaternion localRotation = default(Quaternion);
					localRotation.z = Mathf.Sin(f);
					localRotation.w = Mathf.Cos(f);
					transform.localRotation = localRotation;
				}
			}
			else
			{
				Vector3 position = skeletonTransform.TransformPoint(new Vector3(vector.x, vector.y, 0f));
				if (!followSkeletonZPosition)
				{
					position.z = transform.position.z;
				}
				Transform parent = transform.parent;
				if (parent != null)
				{
					Matrix4x4 localToWorldMatrix = parent.localToWorldMatrix;
					if (localToWorldMatrix.m00 * localToWorldMatrix.m11 - localToWorldMatrix.m01 * localToWorldMatrix.m10 < 0f)
					{
						num = 0f - num;
					}
				}
				if (followRotation)
				{
					Vector3 eulerAngles = skeletonTransform.rotation.eulerAngles;
					transform.SetPositionAndRotation(position, Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + num));
				}
				else
				{
					transform.position = position;
				}
			}
			if (followSkeletonFlip)
			{
				Vector3 localScale = transform.localScale;
				localScale.y = Mathf.Abs(localScale.y) * ((!(bone.skeleton.flipX ^ bone.skeleton.flipY)) ? 1f : (-1f));
				transform.localScale = localScale;
			}
		}
	}
}
