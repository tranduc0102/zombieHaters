using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class JoystickF : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
{
	[Tooltip("When checked, this joystick will stay in a fixed position.")]
	public bool joystickStaysInFixedPosition;

	[Tooltip("Sets the maximum distance the handle (knob) stays away from the center of this joystick. If the joystick handle doesn't look or feel right you can change this value. Must be a whole number. Default value is 4.")]
	public int joystickHandleDistance = 4;

	public Image bgImage;

	public Image joystickKnobImage;

	private Vector3 inputVector;

	private Vector3 unNormalizedInput;

	private Vector3[] fourCornersArray = new Vector3[4];

	private Vector2 bgImageStartPosition;

	private int touchCounter;

	private RectTransform rect;

	private void Start()
	{
		CrossPlatformInputManager.SetAxisZero("Horizontal");
		CrossPlatformInputManager.SetAxisZero("Vertical");
		rect = base.gameObject.GetComponent<RectTransform>();
		if (GetComponent<Image>() == null)
		{
			Debug.LogError("There is no joystick image attached to this script.");
		}
		if (base.transform.GetChild(0).GetComponent<Image>() == null)
		{
			Debug.LogError("There is no joystick handle image attached to this script.");
		}
		if (GetComponent<Image>() != null && base.transform.GetChild(0).GetComponent<Image>() != null)
		{
			bgImage.rectTransform.SetAsLastSibling();
		}
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		Vector2 localPoint = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, ped.position, ped.pressEventCamera, out localPoint))
		{
			localPoint.x /= bgImage.rectTransform.sizeDelta.x;
			localPoint.y /= bgImage.rectTransform.sizeDelta.y;
			inputVector = new Vector3(localPoint.x, localPoint.y, 0f);
			unNormalizedInput = inputVector;
			inputVector = ((!(inputVector.magnitude > 1f)) ? inputVector : inputVector.normalized);
			joystickKnobImage.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImage.rectTransform.sizeDelta.x / (float)joystickHandleDistance), inputVector.y * (bgImage.rectTransform.sizeDelta.y / (float)joystickHandleDistance));
			if (!joystickStaysInFixedPosition && unNormalizedInput.magnitude > inputVector.magnitude)
			{
				Vector3 position = bgImage.rectTransform.position;
				position.x += ped.delta.x;
				position.y += ped.delta.y;
				position.x = Mathf.Clamp(position.x, bgImage.rectTransform.sizeDelta.x, Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, (float)Screen.height - bgImage.rectTransform.sizeDelta.y);
				bgImage.rectTransform.position = position;
			}
			CrossPlatformInputManager.SetAxis("Horizontal", inputVector.x);
			CrossPlatformInputManager.SetAxis("Vertical", inputVector.y);
		}
	}

	public virtual void OnPointerDown(PointerEventData ped)
	{
		touchCounter++;
		if (touchCounter == 1)
		{
			bgImage.gameObject.SetActive(true);
			joystickKnobImage.rectTransform.anchoredPosition = Vector2.zero;
			bgImage.rectTransform.position = ped.position;
			Vector2 localPoint;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, ped.position, ped.pressEventCamera, out localPoint))
			{
				bgImage.rectTransform.anchoredPosition3D = new Vector3(localPoint.x, localPoint.y, 0f);
			}
			OnDrag(ped);
		}
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		touchCounter--;
		if (touchCounter == 0)
		{
			bgImage.gameObject.SetActive(false);
			inputVector = Vector2.zero;
			joystickKnobImage.rectTransform.anchoredPosition = Vector2.zero;
			CrossPlatformInputManager.SetAxisZero("Horizontal");
			CrossPlatformInputManager.SetAxisZero("Vertical");
		}
	}

	public Vector3 GetInputDirection()
	{
		return new Vector3(inputVector.x, inputVector.y, 0f);
	}

	private void OnDisable()
	{
		bgImage.gameObject.SetActive(false);
		inputVector = Vector2.zero;
		joystickKnobImage.rectTransform.anchoredPosition = Vector2.zero;
		CrossPlatformInputManager.SetAxisZero("Horizontal");
		CrossPlatformInputManager.SetAxisZero("Vertical");
		OnPointerUp(null);
	}

	private void OnEnable()
	{
		bgImage.gameObject.SetActive(false);
		inputVector = Vector2.zero;
		joystickKnobImage.rectTransform.anchoredPosition = Vector2.zero;
		CrossPlatformInputManager.SetAxisZero("Horizontal");
		CrossPlatformInputManager.SetAxisZero("Vertical");
		touchCounter = 0;
	}
}
