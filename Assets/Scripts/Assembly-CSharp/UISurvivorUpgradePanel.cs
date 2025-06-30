using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISurvivorUpgradePanel : UIBaseScrollPanel<SurviviorContent>
{
	[HideInInspector]
	public SaveData.HeroData.HeroType lastOpenedHeroType;

	[HideInInspector]
	public bool animationCompleted = true;

	public RenderTexture renderTextureSurvivorPrefab;

	public Camera heroCamPrefab;

	public override void CreateCells()
	{
		base.CreateCells();
		UpdateAllContent();
		UpdateInactiveButton();
	}

	public override void UpdateAllContent()
	{
		if (dataArray != null && dataArray.Length > 0)
		{
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].UpdateContent();
			}
		}
	}

	public void SetRandomVideo()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].SetVideoButton(false);
			list.Add(i);
		}
		if (IsVideoUpgradeAvailable() || /*!AdsManager.instance.IsRewardedVideoAvailable(AdsManager.AdName.RewardUpgradeHero) ||*/ !GameManager.instance.IsTutorialCompleted() || DataLoader.Instance.GetCurrentPlayerLevel() <= 2)
		{
			return;
		}
		do
		{
			int index = Random.Range(0, list.Count);
			if (dataArray[list[index]].IsVideoAvailable() || dataArray[list[index]].SetVideoButton(true))
			{
				StartCoroutine(ScrollToHero(list[index]));
				AnalyticsManager.instance.LogEvent("AddedVideoButton", new Dictionary<string, string> { 
				{
					"Hero",
					dataArray[list[index]].heroData.heroType.ToString()
				} });
				break;
			}
			list.Remove(list[index]);
		}
		while (list.Count > 0);
	}

	private IEnumerator ScrollToHero(int heroIndex)
	{
		heroIndex++;
		scrollRect.content.anchoredPosition = new Vector2(0f, scrollRect.content.anchoredPosition.y);
		yield return null;
		RectTransform rect = DataLoader.gui.GetComponent<RectTransform>();
		float visibleHeroes = rect.sizeDelta.x / (cellPrefab.sizeDelta.x + distanceBetweenCells);
		Vector2 pos = new Vector2((0f - ((float)heroIndex - visibleHeroes)) * (cellPrefab.sizeDelta.x + distanceBetweenCells), scrollRect.content.anchoredPosition.y);
		while (scrollRect.content.anchoredPosition.x > pos.x + distanceBetweenCells * 2f)
		{
			scrollRect.content.anchoredPosition = Vector2.MoveTowards(scrollRect.content.anchoredPosition, pos, Time.deltaTime * 2500f);
			yield return new WaitForSeconds(Time.deltaTime / 2f);
		}
	}

	public void UpdateInactiveButton()
	{
		if (dataArray != null)
		{
			for (int i = 0; i < dataArray.Length; i++)
			{
				dataArray[i].UpdateInactiveButton();
			}
		}
	}

	public void AnimateLastheroOpened()
	{
		StartCoroutine(DelayedAnimation());
	}

	public IEnumerator DelayedAnimation()
	{
		int index = GetHeroIndex(lastOpenedHeroType);
		GameManager.instance.Reset();
		yield return StartCoroutine(ScrollToHero(index));
		dataArray[index].unlockFX.Play();
		SoundManager.Instance.PlaySound(SoundManager.Instance.newHeroOpened);
		yield return new WaitForSeconds(0.1f);
		DataLoader.gui.UpdateMenuContent();
		animationCompleted = true;
		if (lastOpenedHeroType == SaveData.HeroData.HeroType.SNIPER)
		{
			UIRateUs.instance.Show();
		}
	}

	public int GetHeroIndex(SaveData.HeroData.HeroType _type)
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (dataArray[i].heroData.heroType == _type)
			{
				return i;
			}
		}
		return 0;
	}

	public bool IsVideoUpgradeAvailable()
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (dataArray[i].IsVideoAvailable())
			{
				StartCoroutine(ScrollToHero(i));
				AnalyticsManager.instance.LogEvent("AddedVideoButton", new Dictionary<string, string> { 
				{
					"Hero",
					dataArray[i].heroData.heroType.ToString()
				} });
				return true;
			}
		}
		return false;
	}

	public void SetOpenedheroIcon(SaveData.HeroData.HeroType _type)
	{
		lastOpenedHeroType = _type;
		animationCompleted = false;
		DataLoader.gui.popUpsPanel.heroIcon.texture = dataArray[GetHeroIndex(_type)].rawImage.texture;
	}

	public void ActivateHeroCams(bool active)
	{
		for (int i = 0; i < dataArray.Length; i++)
		{
			dataArray[i].ActivateCamera(active);
		}
	}

	public override int GetCellCount()
	{
		return DataLoader.Instance.survivors.Count;
	}
}
