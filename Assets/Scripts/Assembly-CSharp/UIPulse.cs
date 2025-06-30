using System.Collections;
using UnityEngine;

public class UIPulse : MonoBehaviour
{
	[SerializeField]
	private float loopDelay;

	[SerializeField]
	private int pulseCount;

	[SerializeField]
	private float maxScale;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float timeBetweenPulse;

	private void OnEnable()
	{
		StartCoroutine(Pulse());
	}

	private IEnumerator Pulse()
	{
		while (true)
		{
			yield return new WaitForSeconds(loopDelay);
			for (int i = 0; i < pulseCount; i++)
			{
				while (base.transform.localScale != Vector3.one * maxScale)
				{
					base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.one * maxScale, Time.deltaTime * speed);
					yield return null;
				}
				while (base.transform.localScale != Vector3.one)
				{
					base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, Vector3.one, Time.deltaTime * speed);
					yield return null;
				}
				yield return new WaitForSeconds(timeBetweenPulse);
			}
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
