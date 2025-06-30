using UnityEngine;
using UnityEngine.UI;

public class UIBottomPanelButtons : MonoBehaviour
{
	[SerializeField]
	private UIBottomScrollPanel bottomScrollPanel;

	[SerializeField]
	private Button[] buttons;

	private int currentSelectedIndex = -1;

	public void OpenBoostersContent()
	{
		if (SelectButton(0))
		{
			bottomScrollPanel.SetNewContentType<UIBoostersScrollCellContent>();
		}
	}

	public void OpenPassiveAbilitiesContent()
	{
		if (SelectButton(1))
		{
			bottomScrollPanel.SetNewContentType<UIPassiveAbilitiesScrollCellContent>();
		}
	}

	public void OpenHeroContent()
	{
		if (SelectButton(2))
		{
			bottomScrollPanel.SetNewContentType<UIHeroesScrollCellContent>();
		}
	}

	public void OpenBossesContent()
	{
		if (SelectButton(3))
		{
			bottomScrollPanel.SetNewContentType<UIBossesScrollCellContent>();
		}
	}

	public void OpenWorldsContent()
	{
		if (SelectButton(4))
		{
			bottomScrollPanel.SetNewContentType<UIWorldsScrollCellContent>();
		}
	}

	public bool SelectButton(int index)
	{
		if (currentSelectedIndex == index)
		{
			bottomScrollPanel.ScrollToTop();
			return false;
		}
		currentSelectedIndex = index;
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].image.color = ((i != currentSelectedIndex) ? Color.white : new Color(0.20784314f, 0.35686275f, 0.81960785f, 1f));
		}
		return true;
	}
}
