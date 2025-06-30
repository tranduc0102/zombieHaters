using UnityEngine;

namespace Spine.Unity.Examples
{
	public class BoneLocalOverride : MonoBehaviour
	{
		[SpineBone("", "", true, false)]
		public string boneName;

		[Space]
		[Range(0f, 1f)]
		public float alpha = 1f;

		[Space]
		public bool overridePosition = true;

		public Vector2 localPosition;

		[Space]
		public bool overrideRotation = true;

		[Range(0f, 360f)]
		public float rotation;

		private ISkeletonAnimation spineComponent;

		private Bone bone;

		private void Awake()
		{
			spineComponent = GetComponent<ISkeletonAnimation>();
			if (spineComponent == null)
			{
				base.enabled = false;
				return;
			}
			spineComponent.UpdateLocal += OverrideLocal;
			if (bone == null)
			{
				base.enabled = false;
			}
		}

		private void OverrideLocal(ISkeletonAnimation animated)
		{
			if (bone == null || bone.Data.Name != boneName)
			{
				if (string.IsNullOrEmpty(boneName))
				{
					return;
				}
				bone = spineComponent.Skeleton.FindBone(boneName);
				if (bone == null)
				{
					Debug.LogFormat("Cannot find bone: '{0}'", boneName);
					return;
				}
			}
			if (overridePosition)
			{
				bone.X = Mathf.Lerp(bone.X, localPosition.x, alpha);
				bone.Y = Mathf.Lerp(bone.Y, localPosition.y, alpha);
			}
			if (overrideRotation)
			{
				bone.Rotation = Mathf.Lerp(bone.Rotation, rotation, alpha);
			}
		}
	}
}
