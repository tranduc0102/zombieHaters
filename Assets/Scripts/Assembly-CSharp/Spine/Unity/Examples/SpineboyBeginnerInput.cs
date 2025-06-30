using UnityEngine;

namespace Spine.Unity.Examples
{
	public class SpineboyBeginnerInput : MonoBehaviour
	{
		public string horizontalAxis = "Horizontal";

		public string attackButton = "Fire1";

		public string jumpButton = "Jump";

		public SpineboyBeginnerModel model;

		private void OnValidate()
		{
			if (model == null)
			{
				model = GetComponent<SpineboyBeginnerModel>();
			}
		}

		private void Update()
		{
			if (!(model == null))
			{
				float axisRaw = Input.GetAxisRaw(horizontalAxis);
				model.TryMove(axisRaw);
				if (Input.GetButton(attackButton))
				{
					model.TryShoot();
				}
				if (Input.GetButtonDown(jumpButton))
				{
					model.TryJump();
				}
			}
		}
	}
}
