using System.Collections;
using UnityEngine;

public class UIObjectRotator : MonoBehaviour
{
	[SerializeField]
	private float speed;

	[SerializeField]
	private Vector3 direction;

	private IEnumerator Spin()
	{
		while (true)
		{
			base.transform.Rotate(direction, speed * Time.deltaTime);
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(Spin());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
