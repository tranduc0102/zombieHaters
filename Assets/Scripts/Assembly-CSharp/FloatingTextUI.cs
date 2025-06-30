using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextUI : MonoBehaviour
{
	public Text text;

	[SerializeField]
	private Image coinImage;

	private Color color;

	private void Start()
	{
		StartCoroutine(Up());
	}

	private IEnumerator Up()
	{
		float speed = 160f;
		while (text.color.a > 0f)
		{
			yield return null;
			text.rectTransform.anchoredPosition = new Vector2(text.rectTransform.anchoredPosition.x, text.rectTransform.anchoredPosition.y + speed * Time.deltaTime);
			color = text.color;
			color.a -= Time.deltaTime;
			text.color = color;
			color = coinImage.color;
			color.a -= Time.deltaTime;
			coinImage.color = color;
		}
		Object.Destroy(base.gameObject);
	}
}
