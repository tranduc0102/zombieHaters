using UnityEngine;

namespace Spine.Unity.Examples
{
	public class TwoByTwoTransformEffectExample : MonoBehaviour
	{
		public Vector2 xAxis = new Vector2(1f, 0f);

		public Vector2 yAxis = new Vector2(0f, 1f);

		private SkeletonRenderer skeletonRenderer;

		private void OnEnable()
		{
			skeletonRenderer = GetComponent<SkeletonRenderer>();
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
				skeletonRenderer.OnPostProcessVertices += ProcessVertices;
				Debug.Log("2x2 Transform Effect Enabled.");
			}
		}

		private void ProcessVertices(MeshGeneratorBuffers buffers)
		{
			if (base.enabled)
			{
				int vertexCount = buffers.vertexCount;
				Vector3[] vertexBuffer = buffers.vertexBuffer;
				Vector3 vector = default(Vector3);
				for (int i = 0; i < vertexCount; i++)
				{
					Vector3 vector2 = vertexBuffer[i];
					vector.x = xAxis.x * vector2.x + yAxis.x * vector2.y;
					vector.y = xAxis.y * vector2.x + yAxis.y * vector2.y;
					vertexBuffer[i] = vector;
				}
			}
		}

		private void OnDisable()
		{
			if (!(skeletonRenderer == null))
			{
				skeletonRenderer.OnPostProcessVertices -= ProcessVertices;
				Debug.Log("2x2 Transform Effect Disabled.");
			}
		}
	}
}
