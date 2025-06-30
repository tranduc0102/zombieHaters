using UnityEngine;
using UnityEngine.UI;

public abstract class UIBaseScrollPanel<T> : MonoBehaviour where T : UIScrollCell
{
	public ScrollRect scrollRect;

	[SerializeField]
	protected RectTransform cellPrefab;

	[SerializeField]
	protected float distanceBetweenCells = 10f;

	[SerializeField]
	protected float startBorder;

	[SerializeField]
	protected bool horizontal;

	protected T[] dataArray;

	[HideInInspector]
	public bool cellsCreated;

	public virtual void CreateCells()
	{
		dataArray = UIController.instance.CreareScrollContent<T>(cellPrefab, scrollRect, GetCellCount(), startBorder, distanceBetweenCells, horizontal);
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].SetContent(i);
		}
		cellPrefab.gameObject.SetActive(false);
		cellsCreated = true;
	}

	public abstract void UpdateAllContent();

	public abstract int GetCellCount();
}
