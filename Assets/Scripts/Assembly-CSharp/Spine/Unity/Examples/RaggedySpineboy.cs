using System.Collections;
using Spine.Unity.Modules;
using UnityEngine;

namespace Spine.Unity.Examples
{
	public class RaggedySpineboy : MonoBehaviour
	{
		public LayerMask groundMask;

		public float restoreDuration = 0.5f;

		public Vector2 launchVelocity = new Vector2(50f, 100f);

		private SkeletonRagdoll2D ragdoll;

		private Collider2D naturalCollider;

		private void Start()
		{
			ragdoll = GetComponent<SkeletonRagdoll2D>();
			naturalCollider = GetComponent<Collider2D>();
		}

		private void AddRigidbody()
		{
			Rigidbody2D rigidbody2D = base.gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.freezeRotation = true;
			naturalCollider.enabled = true;
		}

		private void RemoveRigidbody()
		{
			Object.Destroy(GetComponent<Rigidbody2D>());
			naturalCollider.enabled = false;
		}

		private void OnMouseUp()
		{
			if (naturalCollider.enabled)
			{
				Launch();
			}
		}

		private void Launch()
		{
			RemoveRigidbody();
			ragdoll.Apply();
			ragdoll.RootRigidbody.velocity = new Vector2(Random.Range(0f - launchVelocity.x, launchVelocity.x), launchVelocity.y);
			StartCoroutine(WaitUntilStopped());
		}

		private IEnumerator Restore()
		{
			Vector3 estimatedPos = ragdoll.EstimatedSkeletonPosition;
			Vector3 rbPosition = ragdoll.RootRigidbody.position;
			Vector3 skeletonPoint = estimatedPos;
			RaycastHit2D hit = Physics2D.Raycast(rbPosition, estimatedPos - rbPosition, Vector3.Distance(estimatedPos, rbPosition), groundMask);
			if (hit.collider != null)
			{
				skeletonPoint = hit.point;
			}
			ragdoll.RootRigidbody.isKinematic = true;
			ragdoll.SetSkeletonPosition(skeletonPoint);
			yield return ragdoll.SmoothMix(0f, restoreDuration);
			ragdoll.Remove();
			AddRigidbody();
		}

		private IEnumerator WaitUntilStopped()
		{
			yield return new WaitForSeconds(0.5f);
			float t = 0f;
			while (t < 0.5f)
			{
				t = ((!(ragdoll.RootRigidbody.velocity.magnitude > 0.09f)) ? (t + Time.deltaTime) : 0f);
				yield return null;
			}
			StartCoroutine(Restore());
		}
	}
}
