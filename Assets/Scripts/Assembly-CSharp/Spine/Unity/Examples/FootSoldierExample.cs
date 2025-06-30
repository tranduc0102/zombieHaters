using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class FootSoldierExample : MonoBehaviour
	{
		[SpineAnimation("Idle", "", true, false)]
		public string idleAnimation;

		[SpineAnimation("", "", true, false)]
		public string attackAnimation;

		[SpineAnimation("", "", true, false)]
		public string moveAnimation;

		[SpineSlot("", "", false, true, false)]
		public string eyesSlot;

		[SpineAttachment(true, false, false, "eyesSlot", "", "", true, false)]
		public string eyesOpenAttachment;

		[SpineAttachment(true, false, false, "eyesSlot", "", "", true, false)]
		public string blinkAttachment;

		[Range(0f, 0.2f)]
		public float blinkDuration = 0.05f;

		public KeyCode attackKey = KeyCode.Mouse0;

		public KeyCode rightKey = KeyCode.D;

		public KeyCode leftKey = KeyCode.A;

		public float moveSpeed = 3f;

		private SkeletonAnimation skeletonAnimation;

		private void Awake()
		{
			skeletonAnimation = GetComponent<SkeletonAnimation>();
			skeletonAnimation.OnRebuild += Apply;
		}

		private void Apply(SkeletonRenderer skeletonRenderer)
		{
			StartCoroutine("Blink");
		}

		private void Update()
		{
			if (Input.GetKey(attackKey))
			{
				skeletonAnimation.AnimationName = attackAnimation;
			}
			else if (Input.GetKey(rightKey))
			{
				skeletonAnimation.AnimationName = moveAnimation;
				skeletonAnimation.Skeleton.FlipX = false;
				base.transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);
			}
			else if (Input.GetKey(leftKey))
			{
				skeletonAnimation.AnimationName = moveAnimation;
				skeletonAnimation.Skeleton.FlipX = true;
				base.transform.Translate((0f - moveSpeed) * Time.deltaTime, 0f, 0f);
			}
			else
			{
				skeletonAnimation.AnimationName = idleAnimation;
			}
		}

		private IEnumerator Blink()
		{
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(0.25f, 3f));
				skeletonAnimation.Skeleton.SetAttachment(eyesSlot, blinkAttachment);
				yield return new WaitForSeconds(blinkDuration);
				skeletonAnimation.Skeleton.SetAttachment(eyesSlot, eyesOpenAttachment);
			}
		}
	}
}
