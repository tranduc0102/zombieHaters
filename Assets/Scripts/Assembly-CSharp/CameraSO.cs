using System.Collections;
using UnityEngine;

public class CameraSO : MonoBehaviour
{
	private float defaultSize;

	private DeviceOrientation prevOrientation;

	private void Start()
	{
		defaultSize = Camera.main.fieldOfView;
		prevOrientation = Input.deviceOrientation;
		StartCoroutine(Smoothing());
	}

	private void Update()
	{
		if (prevOrientation != Input.deviceOrientation)
		{
			StopAllCoroutines();
			prevOrientation = Input.deviceOrientation;
			StartCoroutine(Smoothing());
		}
	}

	private IEnumerator Smoothing()
	{
		float targetField;
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
		{
			targetField = defaultSize * ((float)Screen.width / (float)Screen.height);
		}
		else
		{
			if (Input.deviceOrientation != DeviceOrientation.Portrait && Input.deviceOrientation != DeviceOrientation.PortraitUpsideDown)
			{
				yield break;
			}
			targetField = defaultSize;
		}
		while ((int)Camera.main.fieldOfView != (int)targetField)
		{
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetField, 0.2f);
			yield return null;
		}
	}
}
