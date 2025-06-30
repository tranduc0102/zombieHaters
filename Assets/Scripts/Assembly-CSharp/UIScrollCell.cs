using UnityEngine;

public class UIScrollCell : MonoBehaviour
{
	public RectTransform rectTransform;

	public int cellIndex { get; private set; }

	public virtual void SetContent(int index)
	{
		cellIndex = index;
	}

	protected void SetPivot(Vector2 pivot)
	{
		if (!(rectTransform == null))
		{
			Vector2 size = rectTransform.rect.size;
			Vector2 vector = rectTransform.pivot - pivot;
			Vector3 vector2 = new Vector3(vector.x * size.x, vector.y * size.y);
			rectTransform.pivot = pivot;
			rectTransform.localPosition -= vector2;
		}
	}
}
