using UnityEngine;

namespace Spine.Unity.Examples
{
	public class Rotator : MonoBehaviour
	{
		public Vector3 direction = new Vector3(0f, 0f, 1f);

		public float speed = 1f;

		private void Update()
		{
			base.transform.Rotate(direction * (speed * Time.deltaTime * 100f));
		}
	}
}
