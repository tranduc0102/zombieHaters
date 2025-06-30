using System;
using System.Collections;
using UnityEngine;

namespace Spine.Unity.Examples
{
	[SelectionBase]
	public class SpineboyBeginnerModel : MonoBehaviour
	{
		[Header("Current State")]
		public SpineBeginnerBodyState state;

		public bool facingLeft;

		[Range(-1f, 1f)]
		public float currentSpeed;

		[Header("Balance")]
		public float shootInterval = 0.12f;

		private float lastShootTime;

		public event Action ShootEvent;

		public void TryJump()
		{
			StartCoroutine(JumpRoutine());
		}

		public void TryShoot()
		{
			float time = Time.time;
			if (time - lastShootTime > shootInterval)
			{
				lastShootTime = time;
				if (this.ShootEvent != null)
				{
					this.ShootEvent();
				}
			}
		}

		public void TryMove(float speed)
		{
			currentSpeed = speed;
			if (speed != 0f)
			{
				bool flag = speed < 0f;
				facingLeft = flag;
			}
			if (state != SpineBeginnerBodyState.Jumping)
			{
				state = ((speed != 0f) ? SpineBeginnerBodyState.Running : SpineBeginnerBodyState.Idle);
			}
		}

		private IEnumerator JumpRoutine()
		{
			if (state != SpineBeginnerBodyState.Jumping)
			{
				state = SpineBeginnerBodyState.Jumping;
				Vector3 pos = base.transform.localPosition;
				for (float t2 = 0f; t2 < 0.6f; t2 += Time.deltaTime)
				{
					float d = 20f * (0.6f - t2);
					base.transform.Translate(d * Time.deltaTime * Vector3.up);
					yield return null;
				}
				for (float t = 0f; t < 0.6f; t += Time.deltaTime)
				{
					float d2 = 20f * t;
					base.transform.Translate(d2 * Time.deltaTime * Vector3.down);
					yield return null;
				}
				base.transform.localPosition = pos;
				state = SpineBeginnerBodyState.Idle;
			}
		}
	}
}
