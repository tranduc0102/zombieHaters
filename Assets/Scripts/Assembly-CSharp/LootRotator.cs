using System.Collections;
using UnityEngine;

public class LootRotator : MonoBehaviour
{
	[SerializeField]
	private float rotationSpeed;

	[SerializeField]
	private float upDownSpeed;

	[SerializeField]
	private Vector3 upDownOffset;

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = base.transform.localPosition;
		StartCoroutine(Rotation());
		StartCoroutine(UpDown());
	}

	private IEnumerator UpDown()
	{
		while (true)
		{
			if (base.transform.localPosition != startPosition + upDownOffset)
			{
				base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, startPosition + upDownOffset, Time.deltaTime * upDownSpeed);
				yield return null;
				continue;
			}
			yield return null;
			while (base.transform.localPosition != startPosition - upDownOffset)
			{
				base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, startPosition - upDownOffset, Time.deltaTime * upDownSpeed);
				yield return null;
			}
			yield return null;
		}
	}

	private IEnumerator Rotation()
	{
		while (true)
		{
			base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y + rotationSpeed * Time.deltaTime, base.transform.eulerAngles.z);
			yield return null;
		}
	}
}
