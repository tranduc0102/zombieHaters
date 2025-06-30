using UnityEngine;

namespace Spine.Unity.Examples
{
	public class DraggableTransform : MonoBehaviour
	{
		private Vector2 mousePreviousWorld;

		private Vector2 mouseDeltaWorld;

		private Camera mainCamera;

		private void Start()
		{
			mainCamera = Camera.main;
		}

		private void Update()
		{
			Vector2 vector = Input.mousePosition;
			Vector2 vector2 = mainCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, 0f - mainCamera.transform.position.z));
			mouseDeltaWorld = vector2 - mousePreviousWorld;
			mousePreviousWorld = vector2;
		}

		private void OnMouseDrag()
		{
			base.transform.Translate(mouseDeltaWorld);
		}
	}
}
