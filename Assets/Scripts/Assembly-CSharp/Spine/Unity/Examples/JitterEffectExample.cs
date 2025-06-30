using UnityEngine;

namespace Spine.Unity.Examples
{
	public class JitterEffectExample : MonoBehaviour
	{
		[Range(0f, 0.8f)]
		public float jitterMagnitude = 0.2f;

		private SkeletonRenderer skeletonRenderer;

		private void OnEnable()
		{
			skeletonRenderer = GetComponent<SkeletonRenderer>();
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
				skeletonRenderer.OnPostProcessVertices += ProcessVertices;
				Debug.Log("Jitter Effect Enabled.");
			}
		}

		private void ProcessVertices(MeshGeneratorBuffers buffers)
		{
			if (base.enabled)
			{
				int vertexCount = buffers.vertexCount;
				Vector3[] vertexBuffer = buffers.vertexBuffer;
				for (int i = 0; i < vertexCount; i++)
				{
					vertexBuffer[i] += (Vector3)(Random.insideUnitCircle * jitterMagnitude);
				}
			}
		}

		private void OnDisable()
		{
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
				Debug.Log("Jitter Effect Disabled.");
			}
		}
	}
}
