using UnityEngine;

public class MovedFillBar : MonoBehaviour
{
	[SerializeField]
	private RectTransform bar;

	private float startPosition;

	private float endPosition;

	private void Start()
	{
		startPosition = 0f;
		bar.anchoredPosition = new Vector2(startPosition, bar.anchoredPosition.y);
		endPosition = startPosition + bar.sizeDelta.x;
	}

	public void FillBar(float percentage)
	{
		bar.anchoredPosition = new Vector2(startPosition + (endPosition - startPosition) * percentage, bar.anchoredPosition.y);
	}
}
