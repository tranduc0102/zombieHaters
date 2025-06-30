using System;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineboyFootplanter : MonoBehaviour
	{
		[Serializable]
		public class FootMovement
		{
			public AnimationCurve xMoveCurve;

			public AnimationCurve raiseCurve;

			public float maxRaise;

			public float minDistanceCompensate;

			public float maxDistanceCompensate;
		}

		[Serializable]
		public class Foot
		{
			public Vector2 worldPos;

			public float displacementFromCenter;

			public float distanceFromCenter;

			[Space]
			public float lerp;

			public Vector2 worldPosPrev;

			public Vector2 worldPosNext;

			public bool IsStepInProgress
			{
				get
				{
					return lerp < 1f;
				}
			}

			public bool IsPrettyMuchDoneStepping
			{
				get
				{
					return lerp > 0.7f;
				}
			}

			public void UpdateDistance(float centerOfGravityX)
			{
				displacementFromCenter = worldPos.x - centerOfGravityX;
				distanceFromCenter = Mathf.Abs(displacementFromCenter);
			}

			public void StartNewStep(float newDistance, float centerOfGravityX, float tentativeY, float footRayRaise, RaycastHit2D[] hits, Vector2 footSize)
			{
				lerp = 0f;
				worldPosPrev = worldPos;
				float x = centerOfGravityX - newDistance;
				Vector2 origin = new Vector2(x, tentativeY + footRayRaise);
				int num = Physics2D.BoxCast(origin, footSize, 0f, Vector2.down, new ContactFilter2D
				{
					useTriggers = false
				}, hits);
				worldPosNext = ((num <= 0) ? new Vector2(x, tentativeY) : hits[0].point);
			}

			public void UpdateStepProgress(float deltaTime, float stepSpeed, float shuffleDistance, FootMovement forwardMovement, FootMovement backwardMovement)
			{
				if (IsStepInProgress)
				{
					lerp += deltaTime * stepSpeed;
					float f = worldPosNext.x - worldPosPrev.x;
					float num = Mathf.Sign(f);
					float num2 = Mathf.Abs(f);
					FootMovement footMovement = ((!(num > 0f)) ? backwardMovement : forwardMovement);
					worldPos.x = Mathf.Lerp(worldPosPrev.x, worldPosNext.x, footMovement.xMoveCurve.Evaluate(lerp));
					float num3 = Mathf.Lerp(worldPosPrev.y, worldPosNext.y, lerp);
					if (num2 > shuffleDistance)
					{
						float num4 = Mathf.Clamp(num2 * 0.5f, 1f, 2f);
						worldPos.y = num3 + footMovement.raiseCurve.Evaluate(lerp) * footMovement.maxRaise * num4;
					}
					else
					{
						lerp += Time.deltaTime;
						worldPos.y = num3;
					}
					if (lerp > 1f)
					{
						lerp = 1f;
					}
				}
			}

			public static float GetNewDisplacement(float otherLegDisplacementFromCenter, float comfyDistance, float minimumFootDistanceX, float maxNewStepDisplacement, FootMovement forwardMovement, FootMovement backwardMovement)
			{
				FootMovement footMovement = ((!(Mathf.Sign(otherLegDisplacementFromCenter) < 0f)) ? backwardMovement : forwardMovement);
				float num = UnityEngine.Random.Range(footMovement.minDistanceCompensate, footMovement.maxDistanceCompensate);
				float num2 = otherLegDisplacementFromCenter * num;
				if (Mathf.Abs(num2) > maxNewStepDisplacement || Mathf.Abs(otherLegDisplacementFromCenter) < minimumFootDistanceX)
				{
					num2 = comfyDistance * Mathf.Sign(num2) * num;
				}
				return num2;
			}
		}

		public float timeScale = 0.5f;

		[SpineBone("", "", true, false)]
		public string nearBoneName;

		[SpineBone("", "", true, false)]
		public string farBoneName;

		[Header("Settings")]
		public Vector2 footSize;

		public float footRayRaise = 2f;

		public float comfyDistance = 1f;

		public float centerOfGravityXOffset = -0.25f;

		public float feetTooFarApartThreshold = 3f;

		public float offBalanceThreshold = 1.4f;

		public float minimumSpaceBetweenFeet = 0.5f;

		public float maxNewStepDisplacement = 2f;

		public float shuffleDistance = 1f;

		public float baseLerpSpeed = 3.5f;

		public FootMovement forward;

		public FootMovement backward;

		[Header("Debug")]
		[SerializeField]
		private float balance;

		[SerializeField]
		private float distanceBetweenFeet;

		[SerializeField]
		private Foot nearFoot;

		[SerializeField]
		private Foot farFoot;

		private Skeleton skeleton;

		private Bone nearFootBone;

		private Bone farFootBone;

		private RaycastHit2D[] hits = new RaycastHit2D[1];

		public float Balance
		{
			get
			{
				return balance;
			}
		}

		private void Start()
		{
			Time.timeScale = timeScale;
			Vector3 position = base.transform.position;
			nearFoot.worldPos = position;
			nearFoot.worldPos.x -= comfyDistance;
			nearFoot.worldPosPrev = (nearFoot.worldPosNext = nearFoot.worldPos);
			farFoot.worldPos = position;
			farFoot.worldPos.x += comfyDistance;
			farFoot.worldPosPrev = (farFoot.worldPosNext = farFoot.worldPos);
			SkeletonAnimation component = GetComponent<SkeletonAnimation>();
			skeleton = component.Skeleton;
			component.UpdateLocal += UpdateLocal;
			nearFootBone = skeleton.FindBone(nearBoneName);
			farFootBone = skeleton.FindBone(farBoneName);
			nearFoot.lerp = 1f;
			farFoot.lerp = 1f;
		}

		private void UpdateLocal(ISkeletonAnimation animated)
		{
			Transform transform = base.transform;
			Vector2 vector = transform.position;
			float centerOfGravityX = vector.x + centerOfGravityXOffset;
			nearFoot.UpdateDistance(centerOfGravityX);
			farFoot.UpdateDistance(centerOfGravityX);
			balance = nearFoot.displacementFromCenter + farFoot.displacementFromCenter;
			distanceBetweenFeet = Mathf.Abs(nearFoot.worldPos.x - farFoot.worldPos.x);
			bool flag = Mathf.Abs(balance) > offBalanceThreshold;
			if (distanceBetweenFeet > feetTooFarApartThreshold || flag)
			{
				Foot foot;
				Foot foot2;
				if (nearFoot.distanceFromCenter > farFoot.distanceFromCenter)
				{
					foot = nearFoot;
					foot2 = farFoot;
				}
				else
				{
					foot = farFoot;
					foot2 = nearFoot;
				}
				if (!foot.IsStepInProgress && foot2.IsPrettyMuchDoneStepping)
				{
					float newDisplacement = Foot.GetNewDisplacement(foot2.displacementFromCenter, comfyDistance, minimumSpaceBetweenFeet, maxNewStepDisplacement, forward, backward);
					foot.StartNewStep(newDisplacement, centerOfGravityX, vector.y, footRayRaise, hits, footSize);
				}
			}
			float deltaTime = Time.deltaTime;
			float num = baseLerpSpeed;
			num += (Mathf.Abs(balance) - 0.6f) * 2.5f;
			nearFoot.UpdateStepProgress(deltaTime, num, shuffleDistance, forward, backward);
			farFoot.UpdateStepProgress(deltaTime, num, shuffleDistance, forward, backward);
			nearFootBone.SetPosition(transform.InverseTransformPoint(nearFoot.worldPos));
			farFootBone.SetPosition(transform.InverseTransformPoint(farFoot.worldPos));
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(nearFoot.worldPos, 0.15f);
				Gizmos.DrawWireSphere(nearFoot.worldPosNext, 0.15f);
				Gizmos.color = Color.magenta;
				Gizmos.DrawSphere(farFoot.worldPos, 0.15f);
				Gizmos.DrawWireSphere(farFoot.worldPosNext, 0.15f);
			}
		}
	}
}
