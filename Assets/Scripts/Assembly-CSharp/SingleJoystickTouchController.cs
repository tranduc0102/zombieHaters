using UnityEngine;
using UnityEngine.UI;

public class SingleJoystickTouchController : MonoBehaviour
{
	public Image singleJoystickBackgroundImage;

	public bool singleJoyStickAlwaysVisible;

	private Image singleJoystickHandleImage;

	private SingleJoystick singleJoystick;

	private int singleSideFingerID = -1;

	private void Start()
	{
		if (singleJoystickBackgroundImage.GetComponent<SingleJoystick>() == null)
		{
			Debug.LogError("There is no joystick attached to this script.");
		}
		else
		{
			singleJoystick = singleJoystickBackgroundImage.GetComponent<SingleJoystick>();
			singleJoystickBackgroundImage.enabled = singleJoyStickAlwaysVisible;
		}
		if (singleJoystick.transform.GetChild(0).GetComponent<Image>() == null)
		{
			Debug.LogError("There is no joystick handle (knob) attached to this script.");
			return;
		}
		singleJoystickHandleImage = singleJoystick.transform.GetChild(0).GetComponent<Image>();
		singleJoystickHandleImage.enabled = singleJoyStickAlwaysVisible;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (Input.touchCount <= 0)
		{
			return;
		}
		Touch[] touches = Input.touches;
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (touches[i].phase == TouchPhase.Began && singleSideFingerID == -1)
			{
				singleSideFingerID = touches[i].fingerId;
				Vector3 position = singleJoystickBackgroundImage.rectTransform.position;
				position.x = touches[i].position.x + singleJoystickBackgroundImage.rectTransform.sizeDelta.x / 2f;
				position.y = touches[i].position.y - singleJoystickBackgroundImage.rectTransform.sizeDelta.y / 2f;
				position.x = Mathf.Clamp(position.x, singleJoystickBackgroundImage.rectTransform.sizeDelta.x, Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, (float)Screen.height - singleJoystickBackgroundImage.rectTransform.sizeDelta.y);
				singleJoystickBackgroundImage.rectTransform.position = position;
				singleJoystickBackgroundImage.enabled = true;
				singleJoystickBackgroundImage.rectTransform.GetChild(0).GetComponent<Image>().enabled = true;
			}
			if (touches[i].phase == TouchPhase.Ended && touches[i].fingerId == singleSideFingerID)
			{
				singleJoystickBackgroundImage.enabled = singleJoyStickAlwaysVisible;
				singleJoystickHandleImage.enabled = singleJoyStickAlwaysVisible;
				singleSideFingerID = -1;
			}
		}
	}
}
