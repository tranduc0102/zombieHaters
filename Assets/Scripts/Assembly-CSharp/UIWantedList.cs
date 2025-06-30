using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIWantedList : MonoBehaviour
{
	public ScrollRect scrollRect;

	public RectTransform killAllBossesPrefab;

	public RectTransform bossCellPrefab;

	public Text newCountText;

	public GameObject countObj;

	public RenderTexture prefabRenderTexture;

	[SerializeField]
	private ParticleSystem newBossKilledFx;

	private float fxdelay = 0.8f;

	private int count;

	private float spaceBetweenCells = 15f;

	private float worldContentHeight;

	public Camera bossCameraPrefab;

	private List<UIBossCell> bossCells = new List<UIBossCell>();

	private List<UICellKillAllBosses> killAllCells = new List<UICellKillAllBosses>();

	private void OnEnable()
	{
		UpdateAll();
		StartCoroutine(UIController.instance.Scale(base.transform));
		AnalyticsManager.instance.LogEvent("WantedListOpened", new Dictionary<string, string>());
	}

	public void UpdateNewBossFx(bool open)
	{
		if (open)
		{
			newBossKilledFx.Stop();
		}
		else if (count > 0)
		{
			newBossKilledFx.Play();
		}
	}

	public void UpdateAll()
	{
		UpdateBossCells();
		UpdateKillAllCells();
		UpdateCountText();
	}

	public void UpdateBossCells()
	{
		count = 0;
		for (int i = 0; i < bossCells.Count; i++)
		{
			bossCells[i].UpdateCell(this);
		}
	}

	public void PrepareForGameplay()
	{
		newBossKilledFx.Stop();
	}

	public void UpdateKillAllCells()
	{
		Debug.Log("UpdateKillAllCells");
		count = 0;
		for (int i = 0; i < killAllCells.Count; i++)
		{
			killAllCells[i].UpdateContent();
		}
	}

	public void UpdateCountText()
	{
		count = 0;
		for (int i = 0; i < killAllCells.Count; i++)
		{
			if (killAllCells[i].IsReady())
			{
				count++;
			}
		}
		for (int j = 0; j < bossCells.Count; j++)
		{
			if (bossCells[j].IsReady())
			{
				count++;
			}
		}
		countObj.SetActive(count > 0);
		if (countObj.activeInHierarchy)
		{
			newBossKilledFx.Play();
		}
		else
		{
			newBossKilledFx.Stop();
		}
		newCountText.text = count.ToString();
	}

	public void CreateCells()
	{
		WavesManager.Bosses[] bossesByWorld = WavesManager.instance.bossesByWorld;
		float num = 0f;
		float num2 = killAllBossesPrefab.rect.y - spaceBetweenCells;
		for (int i = 0; i < bossesByWorld.Length; i++)
		{
			RectTransform rectTransform = Object.Instantiate(killAllBossesPrefab, scrollRect.content);
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, num2);
			num2 -= rectTransform.rect.height * rectTransform.localScale.y + spaceBetweenCells;
			killAllCells.Add(rectTransform.GetComponent<UICellKillAllBosses>());
			killAllCells.Last().SetContent(bossesByWorld[i], i);
			for (int j = 0; j < bossesByWorld[i].bosses.Length; j++)
			{
				RectTransform rectTransform2 = Object.Instantiate(bossCellPrefab, scrollRect.content);
				rectTransform2.anchoredPosition = new Vector2(rectTransform2.anchoredPosition.x, num2);
				num2 -= rectTransform2.rect.height * rectTransform2.localScale.y + spaceBetweenCells;
				bossCells.Add(rectTransform2.GetComponent<UIBossCell>());
				bossCells.Last().SetContent(bossesByWorld[i].bosses[j], j + i * bossesByWorld[i].bosses.Length);
			}
			num += killAllBossesPrefab.sizeDelta.y + spaceBetweenCells + (bossCellPrefab.sizeDelta.y + spaceBetweenCells) * (float)bossesByWorld[i].bosses.Length;
		}
		killAllBossesPrefab.gameObject.SetActive(false);
		bossCellPrefab.gameObject.SetActive(false);
		scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, num + spaceBetweenCells);
		SetWorldHeight(bossesByWorld[0].bosses.Length);
		Invoke("UpdateCountText", 0.2f);
	}

	public void OpenPopup()
	{
		DataLoader.gui.popUpsPanel.OpenPopup();
		base.gameObject.SetActive(true);
		scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, worldContentHeight * (float)(GameManager.instance.currentWorldNumber - 1));
	}

	public void SetWorldHeight(int bossesInWorld)
	{
		worldContentHeight = killAllBossesPrefab.sizeDelta.y + spaceBetweenCells + (float)bossesInWorld * (bossCellPrefab.sizeDelta.y + spaceBetweenCells);
	}
}
