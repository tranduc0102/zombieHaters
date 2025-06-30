using System.Collections;
using UnityEngine;

public class UIRatingPanel : UIBaseScrollPanel<UIRatingCell>
{
	[SerializeField]
	private float scrollSpeed = 1500f;

	private int currentCell;

	private bool changingCell;

	[HideInInspector]
	public int lastRating;

	public override int GetCellCount()
	{
		return ArenaManager.instance.GetmaxArenaIndex() + 1;
	}

	public override void CreateCells()
	{
		base.CreateCells();
		float size = 0f - scrollRect.content.anchoredPosition.x + cellPrefab.sizeDelta.x + distanceBetweenCells;
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].SetCellSize(size, startBorder);
		}
		lastRating = DataLoader.playerData.arenaRating;
		scrollRect.enabled = false;
	}

	public override void UpdateAllContent()
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].UpdateAppearance(currentCell);
		}
	}

	private void OnEnable()
	{
		if (ArenaManager.instance.arenaSwitched != 0)
		{
			StartCoroutine(DelayedScroll());
		}
		else
		{
			currentCell = ArenaManager.instance.GetArenaIndexByRating(DataLoader.playerData.arenaRating);
			ArenaManager.instance.UpdateAll();
			UpdateAllContent();
			scrollRect.content.anchoredPosition = new Vector2(0f - dataArray[currentCell].GetCellCenterX(), scrollRect.content.anchoredPosition.y);
			UpdateCellsSize();
		}
		lastRating = DataLoader.playerData.arenaRating;
	}

	private IEnumerator DelayedScroll()
	{
		while (DataLoader.gui.popUpsPanel.loading.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.25f);
		StartCoroutine(ChangeCell(ArenaManager.instance.arenaSwitched == 1));
		ArenaManager.instance.UpdateAll();
		ArenaManager.instance.arenaSwitched = 0;
	}

	public void PrevCell()
	{
		if (!changingCell)
		{
			StartCoroutine(ChangeCell(false));
		}
	}

	public void NextCell()
	{
		if (!changingCell)
		{
			StartCoroutine(ChangeCell(true));
		}
	}

	private IEnumerator ChangeCell(bool next)
	{
		currentCell += (next ? 1 : (-1));
		if (currentCell >= dataArray.Length)
		{
			currentCell = dataArray.Length - 1;
			yield break;
		}
		if (currentCell < 0)
		{
			currentCell = 0;
			yield break;
		}
		Vector2 target = new Vector2(0f - dataArray[currentCell].GetCellCenterX(), scrollRect.content.anchoredPosition.y);
		changingCell = true;
		if (!next)
		{
			dataArray[currentCell + 1].psPrev.Play();
			UpdateAllContent();
		}
		while (scrollRect.content.anchoredPosition != target)
		{
			yield return null;
			scrollRect.content.anchoredPosition = Vector2.MoveTowards(scrollRect.content.anchoredPosition, target, scrollSpeed * Time.deltaTime);
			UpdateCellsSize();
		}
		changingCell = false;
		if (next)
		{
			dataArray[currentCell].psNext.Play();
			UpdateAllContent();
		}
	}

	private void UpdateCellsSize()
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].UpdateSize(0f - scrollRect.content.anchoredPosition.x);
		}
	}
}
