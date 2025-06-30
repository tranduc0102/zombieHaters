using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Test : MonoBehaviour
{
	[SerializeField]
	private Transform cameraTarget;

	public Vector3 joystickDirection = Vector3.zero;

	private void Update()
	{
		RefreshJoystickDirection();
		base.transform.position = cameraTarget.position + joystickDirection * 10f;
	}

	public void RefreshJoystickDirection()
	{
        joystickDirection = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0f, CrossPlatformInputManager.GetAxis("Vertical"));
       /* if (Application.isEditor)
		{
			joystickDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		}
		else
		{
		
		}*/
	}
}
