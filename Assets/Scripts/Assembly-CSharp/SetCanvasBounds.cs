using UnityEngine;

public class SetCanvasBounds : MonoBehaviour
{
	[HideInInspector]
	public bool isBoundsChanged;

	[HideInInspector]
	public float offset;

	public RectTransform canvas;

	public RectTransform[] panels;

	private Rect lastSafeArea = new Rect(0f, 0f, Screen.width, Screen.height);

	public static SetCanvasBounds Instance { get; private set; }

	private Rect GetSafeArea()
	{
		float x = 0f;
		float y = 0f;
		float width = Screen.width;
		float height = Screen.height;
		return new Rect(x, y, width, height);
	}

	private void Awake()
	{
		Instance = this;
		Rect safeArea = GetSafeArea();
		if (lastSafeArea != safeArea)
		{
			ApplySafeArea(safeArea);
			StatusBarManager.BarStyle(1);
			StatusBarManager.BarAnim(2);
			isBoundsChanged = true;
		}
	}

	private void ApplySafeArea(Rect area)
	{
		Vector2 position = area.position;
		Vector2 anchorMax = area.position + area.size;
		position.x /= Screen.width;
		position.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		offset = area.yMax - area.yMax * anchorMax.y;
		for (int i = 0; i < panels.Length; i++)
		{
			panels[i].anchorMin = position;
			panels[i].anchorMax = anchorMax;
		}
		lastSafeArea = area;
	}
}
