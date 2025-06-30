using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIColorAlphaChanger : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private Color targetColor;

	[SerializeField]
	private float loopDelay;

	[SerializeField]
	private float speed;

	[SerializeField]
	private float timeBetweenColorChange;

	[SerializeField]
	private int changesCount;

	private Color currentColor;

	private Color startColor;

	private void Start()
	{
		startColor = image.color;
		StartCoroutine(ChangeColor());
	}

	private IEnumerator ChangeColor()
	{
		currentColor = startColor;
		while (true)
		{
			yield return new WaitForSeconds(loopDelay);
			for (int i = 0; i < changesCount; i++)
			{
				while (currentColor.a < 1f)
				{
					currentColor.a += Time.deltaTime * speed;
					image.color = currentColor;
					yield return null;
				}
				while (currentColor.a > 0f)
				{
					currentColor.a -= Time.deltaTime * speed;
					image.color = currentColor;
					yield return null;
				}
				yield return new WaitForSeconds(timeBetweenColorChange);
			}
		}
	}
}
