using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasePointer : MonoBehaviour
{
	[SerializeField]
	protected Transform arrow;

	[SerializeField]
	protected RectTransform CanvasRect;

	[SerializeField]
	protected Text distanceText;

	[Space]
	[SerializeField]
	public float showDistance = 20f;

	[HideInInspector]
	public static float bottomOffset = 200f;

	[HideInInspector]
	public static float topOffset;

	protected Animation showPointerAnim;

	protected RectTransform pointerImage;

	protected Transform cameraTarget;

	protected float borderSpace;

	protected List<Vector3> targets = new List<Vector3>();

	protected bool showArrow = true;

	protected void Start()
	{
		cameraTarget = Object.FindObjectOfType<CameraTarget>().transform;
		pointerImage = arrow.parent.GetComponent<RectTransform>();
		borderSpace = Mathf.Max(pointerImage.sizeDelta.x, pointerImage.sizeDelta.y) / 2f;
		showPointerAnim = pointerImage.GetComponent<Animation>();
	}

	protected void ShowDistance()
	{
		distanceText.enabled = true;
	}

	protected void FixedUpdate()
	{
		if (targets.Count <= 0)
		{
			pointerImage.gameObject.SetActive(false);
			distanceText.enabled = false;
			return;
		}
		SortTargets();
		if (Vector3.Distance(cameraTarget.position, targets[0]) <= showDistance)
		{
			if (!pointerImage.gameObject.activeSelf && (PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted) || GameManager.instance.isTutorialNow))
			{
				pointerImage.gameObject.SetActive(true);
				showPointerAnim.Play();
				Invoke("ShowDistance", 0.5f);
			}
			Vector2 vector = Camera.main.WorldToViewportPoint(targets[0]);
			vector.x -= 0.5f;
			vector.y -= 0.5f;
			Vector2 vector2 = new Vector2(vector.x * CanvasRect.sizeDelta.x, vector.y * CanvasRect.sizeDelta.y);
			if (targets[0].z - cameraTarget.position.z < 0f && vector2.y > 0f)
			{
				vector2 *= -1f;
			}
			if (Mathf.Abs(vector2.x) > CanvasRect.sizeDelta.x / 2f - borderSpace || Mathf.Abs(vector2.y) > CanvasRect.sizeDelta.y / 2f - borderSpace - ((!(vector2.y < 0f)) ? topOffset : bottomOffset))
			{
				if (Mathf.Abs(vector2.x / (CanvasRect.sizeDelta.x / 2f - borderSpace)) > Mathf.Abs(vector2.y / (CanvasRect.sizeDelta.y / 2f - borderSpace - ((!(vector2.y < 0f)) ? topOffset : bottomOffset))))
				{
					float x = vector2.x;
					vector2.x = Mathf.Sign(vector2.x) * (CanvasRect.sizeDelta.x / 2f - borderSpace);
					vector2.y *= Mathf.Abs(vector2.x / x);
				}
				else
				{
					float y = vector2.y;
					vector2.y = Mathf.Sign(vector2.y) * (CanvasRect.sizeDelta.y / 2f - borderSpace - ((!(vector2.y < 0f)) ? topOffset : bottomOffset));
					vector2.x *= Mathf.Abs(vector2.y / y);
				}
				if (showArrow)
				{
					arrow.gameObject.SetActive(true);
					float num = Vector2.Angle(vector2, new Vector2(1f, 0f));
					if (Vector2.Angle(vector2, new Vector2(0f, 1f)) > 90f)
					{
						num *= -1f;
					}
					arrow.localEulerAngles = new Vector3(0f, 0f, num);
					SetDistanceText();
				}
				else
				{
					arrow.gameObject.SetActive(false);
				}
			}
			else
			{
				arrow.gameObject.SetActive(false);
			}
			pointerImage.anchoredPosition = vector2;
		}
		else
		{
			pointerImage.gameObject.SetActive(false);
		}
	}

	protected virtual void SortTargets()
	{
		for (int i = 0; i < targets.Count; i++)
		{
			for (int j = i; j < targets.Count; j++)
			{
				if (Vector3.Distance(cameraTarget.position, targets[j]) < Vector3.Distance(cameraTarget.position, targets[i]))
				{
					Vector3 value = targets[j];
					targets[j] = targets[i];
					targets[i] = value;
				}
			}
		}
	}

	protected virtual void SetDistanceText()
	{
		distanceText.text = (int)Vector3.Distance(cameraTarget.position, targets[0]) + "m";
	}
}
