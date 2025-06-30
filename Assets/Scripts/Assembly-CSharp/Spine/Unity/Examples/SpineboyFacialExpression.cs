using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineboyFacialExpression : MonoBehaviour
	{
		public SpineboyFootplanter footPlanter;

		[SpineSlot("", "", false, true, false)]
		public string eyeSlotName;

		[SpineSlot("", "", false, true, false)]
		public string mouthSlotName;

		[SpineAttachment(true, false, false, "eyeSlotName", "", "", true, false)]
		public string shockEyeName;

		[SpineAttachment(true, false, false, "eyeSlotName", "", "", true, false)]
		public string normalEyeName;

		[SpineAttachment(true, false, false, "mouthSlotName", "", "", true, false)]
		public string shockMouthName;

		[SpineAttachment(true, false, false, "mouthSlotName", "", "", true, false)]
		public string normalMouthName;

		public Slot eyeSlot;

		public Slot mouthSlot;

		public Attachment shockEye;

		public Attachment normalEye;

		public Attachment shockMouth;

		public Attachment normalMouth;

		public float balanceThreshold = 2.5f;

		public float shockDuration = 1f;

		[Header("Debug")]
		public float shockTimer;

		private void Start()
		{
			SkeletonAnimation component = GetComponent<SkeletonAnimation>();
			Skeleton skeleton = component.Skeleton;
			eyeSlot = skeleton.FindSlot(eyeSlotName);
			mouthSlot = skeleton.FindSlot(mouthSlotName);
			int slotIndex = skeleton.FindSlotIndex(eyeSlotName);
			shockEye = skeleton.GetAttachment(slotIndex, shockEyeName);
			normalEye = skeleton.GetAttachment(slotIndex, normalEyeName);
			int slotIndex2 = skeleton.FindSlotIndex(mouthSlotName);
			shockMouth = skeleton.GetAttachment(slotIndex2, shockMouthName);
			normalMouth = skeleton.GetAttachment(slotIndex2, normalMouthName);
		}

		private void Update()
		{
			if (Mathf.Abs(footPlanter.Balance) > balanceThreshold)
			{
				shockTimer = shockDuration;
			}
			if (shockTimer > 0f)
			{
				shockTimer -= Time.deltaTime;
			}
			if (shockTimer > 0f)
			{
				eyeSlot.Attachment = shockEye;
				mouthSlot.Attachment = shockMouth;
			}
			else
			{
				eyeSlot.Attachment = normalEye;
				mouthSlot.Attachment = normalMouth;
			}
		}
	}
}
