using UnityEngine;

namespace Spine.Unity.Examples
{
	public class ConstrainedCamera : MonoBehaviour
	{
		public Transform target;

		public Vector3 offset;

		public Vector3 min;

		public Vector3 max;

		public float smoothing = 5f;

		private void LateUpdate()
		{
			Vector3 b = target.position + offset;
			b.x = Mathf.Clamp(b.x, min.x, max.x);
			b.y = Mathf.Clamp(b.y, min.y, max.y);
			b.z = Mathf.Clamp(b.z, min.z, max.z);
			base.transform.position = Vector3.Lerp(base.transform.position, b, smoothing * Time.deltaTime);
		}
	}
}
