using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopUpsPanel : MonoBehaviour
{
	public Image background;

	[Header("OpenHero")]
	public GameObject openHeroPanel;

	public Text heroName;

	public RawImage heroIcon;

	[Header("Daily")]
	public GameObject dailyPanel;

	public Text dailyHeaderText;

	[Header("Secret")]
	public GameObject secretPanel;

	[Header("ClosedWorld")]
	public GameObject closedWorldPanel;

	public Text[] bossKillsRemainingTexts;

	[Space]
	public UIPopupShop shop;

	public UIStarterPack starterPack;

	public UIRatingGames ratingGames;

	public GameObject connectBattlePanel;

	public GameObject exitPanel;

	public Loading loading;

	public UIIngameShop ingameShop;

	public UISubscription subscription;

	public UINotEnoughMoney notEnoughMoney;

	public UITimeWarp timeWarp;

	private void OnEnable()
	{
		StartCoroutine(SmoothBackGround());
		if ((bool)UIController.instance)
		{
			UIController.instance.scrollControllers.wantedListController.UpdateNewBossFx(true);
		}
		else
		{
			Debug.LogWarning("UIController instance wasn't created yet");
		}
	}

	public void OnDisable()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
		base.gameObject.SetActive(false);
		UIController.instance.scrollControllers.wantedListController.UpdateNewBossFx(false);
	}

	public void DisablePopups()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.gameObject != background.gameObject)
			{
				base.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		if (!starterPack.autoShowCompleted && GameManager.instance.IsTutorialCompleted())
		{
			starterPack.TryToShowPack();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void DisablePopupsWithoutBg()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.gameObject != background.gameObject)
			{
				base.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	public void OpenDaily()
	{
		if (TimeManager.gotDateTime)
		{
			OpenPopup();
			dailyPanel.SetActive(true);
		}
		else
		{
			UIController.instance.scrollControllers.dailyController.dailyAnim.SetBool("IsOpened", false);
		}
	}

	public void OpenSecret()
	{
		if (TimeManager.gotDateTime)
		{
			OpenPopup();
			secretPanel.SetActive(true);
		}
		else
		{
			DataLoader.gui.secretAnimator.SetBool("IsOpened", false);
		}
	}

	public void OpenPopup()
	{
		base.gameObject.SetActive(true);
	}

	public IEnumerator SmoothBackGround()
	{
		background.gameObject.SetActive(true);
		Color color = new Color(0f, 0f, 0f, 0f);
		background.color = color;
		while (background.color.a < 43f / 51f)
		{
			if (color.a + Time.unscaledDeltaTime * 3f > 43f / 51f)
			{
				background.color = new Color(0f, 0f, 0f, 43f / 51f);
				break;
			}
			color.a += Time.unscaledDeltaTime * 3f;
			background.color = color;
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime / 2f);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && base.gameObject.activeInHierarchy && !CheckException())
		{
			base.gameObject.SetActive(false);
		}
	}

	private bool CheckException()
	{
		if (openHeroPanel.activeInHierarchy)
		{
			UIController.instance.scrollControllers.survivorsController.AnimateLastheroOpened();
			base.gameObject.SetActive(false);
			return true;
		}
		if (DataLoader.gui.noInternetPanel.activeInHierarchy)
		{
			return true;
		}
		if (ratingGames.IsTutorial())
		{
			return true;
		}
		if (connectBattlePanel.activeInHierarchy)
		{
			return true;
		}
		return false;
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
