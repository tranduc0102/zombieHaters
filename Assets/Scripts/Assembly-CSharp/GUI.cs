using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IAP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
	[SerializeField]
	private Text moneyText;

	[SerializeField]
	private Text gemsText;

	[SerializeField]
	private Text textPoints;

	[SerializeField]
	private Text textBestScore;

	[SerializeField]
	private Text textNewSurvivorsLeft;

	public GameObject noInternetPanel;

	[SerializeField]
	private Animator mainMenuAnim;

	[SerializeField]
	private Image nextLocationImage;

	[SerializeField]
	private OnLevelUpFx onLevelUpFx;

	[Header("Video Reward Buster")]
	public Animator videoAnim;

	public Animator multiplierAnim;

	public Image multiplierPanelImage;

	public VideoMultiplier videoMultiplier;

	[Header("Pop-ups")]
	public PopUpsPanel popUpsPanel;

	[Header("Level")]
	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Image levelProgress;

	[Header("Offline")]
	[SerializeField]
	private GameObject offlinePanel;

	[SerializeField]
	private Text offlineRewardText;

	public Button offlineX2;

	public RectTransform objButtonOfflineOk;

	[Header("UIPanels")]
	[SerializeField]
	private GameObject PlayUI;

	[SerializeField]
	private GameObject PauseUI;

	[SerializeField]
	private GameObject GameOverUI;

	[SerializeField]
	private GameObject JoystickUI;

	[SerializeField]
	private GameObject MainMenuUI;

	[SerializeField]
	private GameObject TutorialUI;

	[SerializeField]
	private GameObject AbilityTutorialUI;

	[SerializeField]
	private GameObject UpgradeTutorialUI;

	[SerializeField]
	private GameObject NextWorldReadyUI;

	[Header("Sound")]
	[SerializeField]
	private GameObject musicON;

	[SerializeField]
	private GameObject musicOFF;

	[SerializeField]
	private GameObject soundON;

	[SerializeField]
	private GameObject soundOFF;

	[Header("MoneyBox")]
	[SerializeField]
	private Image secretHatImage;

	public Animator secretAnimator;

	public GameObject newSecret;

	[Header("BossUI")]
	[SerializeField]
	private Image fillProgressToBoss;

	[SerializeField]
	private Image fillBossHealth;

	[SerializeField]
	private Text bossNameText;

	[SerializeField]
	private Animator topPanelAnimator;

	[SerializeField]
	private ParticleSystem progressToBossFx;

	[SerializeField]
	private Text textButtonNewLocation;

	[SerializeField]
	private Text textCount1;

	[SerializeField]
	private Text textCount2;

	private float maxWidthProgressToBoss;

	private float maxWidthBossHealth;

	[Header("CarCaravanEventUI")]
	[SerializeField]
	private Image fillFuelTank;

	[SerializeField]
	private Image imageCanister;

	[SerializeField]
	private Image imageWeight1;

	[SerializeField]
	private Image imageWeight2;

	[SerializeField]
	public CarPointer carPointer;

	[SerializeField]
	private FuelPointer fuelPointer;

	[SerializeField]
	private Animator animatorApocalypseUI;

	private float maxWidthFuelTank;

	private List<Image> imageCanisters;

	[Header("WorldsUI")]
	[SerializeField]
	private GameObject buttonNextWorld;

	[SerializeField]
	private GameObject buttonPrevWorld;

	[SerializeField]
	private Text textWorldName;

	[Header("LoadingUI")]
	[SerializeField]
	private Loading loadingScreen;

	[NonSerialized]
	public bool isnewHeroOpened;

	[SerializeField]
	private Animation psSurv;

	[SerializeField]
	private Animation psKillAll;

	[SerializeField]
	private Text delayResumeText;

	[HideInInspector]
	public bool pauseReady;

	private bool MoneyFxCoroutineInProgress;

	[Space]
	public RawImage rawImage;

	public GameObject devSettings;

	private int cachedPlayerLevel;

	[HideInInspector]
	public bool isNewWorldOpened;

	private Coroutine showWorldName;

	[SerializeField]
	private Animator nextWorldReadyHand;

	[SerializeField]
	private RectTransform killAllRect;

	[SerializeField]
	private RectTransform newSurvivorRect;

	[SerializeField]
	private RectTransform incSpeedRect;

	public GameOverManager gameOverManager;

	[Header("PvpUI")]
	public PvpPointer pvpPointer;

	public Text lbl_pvpInGameTimer;

	public MovedFillBar pvpLevelFillBar;

	public Text lbl_pvpPlayerLevel;

	public UIPvpScoreBoard pvpScoreBoard;

	[HideInInspector]
	public string currentAnimationTrigger;

	private HashSet<Survivors> heroesInQueue = new HashSet<Survivors>();

	private Coroutine showLastHeroCor;

	private Coroutine resume;

	private Vector3 defaultResumeScale;

	private void Start()
	{
		DataLoader.SetGui(this);
		if (GameManager.instance.isTutorialNow)
		{
			PlayUI.SetActive(true);
			JoystickUI.SetActive(true);
			StartTutorial();
		}
		else if (!PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted))
		{
			UpgradeTutorialUI.SetActive(true);
			UIController.instance.scrollControllers.survivorsController.scrollRect.enabled = false;
		}
		CheckForNewWorld();
		GetComponent<Canvas>().enabled = true;
		SoundManager.Instance.GetSavedInfo();
		DataLoader.dataUpdateManager.StartUpdate();
		DataLoader.Instance.CheckClosedWalls();
		if (secretAnimator.gameObject.activeInHierarchy)
		{
			secretAnimator.SetBool("IsOpened", true);
		}
		GameManager.instance.RefreshCurrentWorldNumber();
		RefreshWorldsButtons();
		maxWidthProgressToBoss = fillProgressToBoss.rectTransform.sizeDelta.x;
		maxWidthBossHealth = fillBossHealth.rectTransform.sizeDelta.x;
		maxWidthFuelTank = fillFuelTank.rectTransform.sizeDelta.x;
		videoMultiplier.UpdatePercentage();
		UpdateMenuContent();
		CheckOpenedHeroes();
		defaultResumeScale = delayResumeText.transform.localScale;
		popUpsPanel.subscription.UpdateRedCircle();
	}

	public void StartTutorialAfterReset()
	{
		PlayUI.SetActive(true);
		JoystickUI.SetActive(true);
		StartTutorial();
	}

	public void ChangeAnimationState(string trigger)
	{
		if (trigger != "GameOver" && trigger != "MainMenu" && trigger != "Present")
		{
			mainMenuAnim.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		}
		else
		{
			mainMenuAnim.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		}
		mainMenuAnim.SetTrigger(trigger);
		currentAnimationTrigger = trigger;
		AnalyticsManager.instance.LogEvent("Changed_MainMenu_Trigger", new Dictionary<string, string> { { "Trigger", trigger } });
	}

	public void ResetTrigger(string trigger)
	{
		if (trigger != "GameOver" && trigger != "MainMenu" && trigger != "Present")
		{
			mainMenuAnim.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		}
		else
		{
			mainMenuAnim.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		}
		mainMenuAnim.ResetTrigger(trigger);
	}

	public void OnAnimationCompleted(string triggerName, string clipName, UnityAction call)
	{
		StartCoroutine(WaitForAnimationCompleted(triggerName, clipName, call));
	}

	private IEnumerator WaitForAnimationCompleted(string triggerName, string clipName, UnityAction call)
	{
		while (currentAnimationTrigger != triggerName)
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(DataLoader.gui.GetAnimationClipTime(clipName));
		call();
	}

	public float GetAnimationClipTime(string clipName)
	{
		RuntimeAnimatorController runtimeAnimatorController = mainMenuAnim.runtimeAnimatorController;
		for (int i = 0; i < runtimeAnimatorController.animationClips.Length; i++)
		{
			if (runtimeAnimatorController.animationClips[i].name == clipName)
			{
				return runtimeAnimatorController.animationClips[i].length;
			}
		}
		return 0f;
	}

	private IEnumerator StateTime(string trigger)
	{
		Time.timeScale = 1f;
		yield return new WaitForSeconds(1f);
		mainMenuAnim.SetTrigger(trigger);
	}

	public void SetMusic(bool state)
	{
		musicON.SetActive(!state);
		musicOFF.SetActive(state);
	}

	public void SetSound(bool state)
	{
		soundON.SetActive(!state);
		soundOFF.SetActive(state);
	}

	public void UpdateMenuContent()
	{
		UIController.instance.scrollControllers.survivorsController.UpdateAllContent();
		UIController.instance.scrollControllers.achievementsController.UpdateAllContent();
		UpdateMoney();
		UpdateGems();
		UpdateExperience();
	}

	public void UpdateMoney()
	{
		moneyText.text = AbbreviationUtility.AbbreviateNumber(Math.Floor(DataLoader.playerData.money));
		if (!MoneyFxCoroutineInProgress && GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay)
		{
			StartCoroutine(UpdateMoneyScaleFx());
		}
		UIController.instance.scrollControllers.survivorsController.UpdateInactiveButton();
	}

	public void UpdateGems()
	{
		gemsText.text = DataLoader.playerData.gems.ToString();
	}

	public void UpdateMoney(double money)
	{
		moneyText.text = AbbreviationUtility.AbbreviateNumber(money);
	}

	private IEnumerator UpdateMoneyScaleFx()
	{
		float speed = 8f;
		MoneyFxCoroutineInProgress = true;
		Vector3 defaultScale = moneyText.transform.localScale;
		for (float j = 0f; j < 1f; j += Time.deltaTime * speed)
		{
			moneyText.transform.localScale = Vector3.Lerp(moneyText.transform.localScale, moneyText.transform.localScale + new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * speed);
			yield return null;
		}
		for (float i = 0f; i < 1f; i += Time.deltaTime)
		{
			moneyText.transform.localScale = Vector3.Lerp(moneyText.transform.localScale, defaultScale, Time.deltaTime * speed);
			yield return null;
		}
		moneyText.transform.localScale = defaultScale;
		MoneyFxCoroutineInProgress = false;
	}

	public void UpdateExperience()
	{
		if (cachedPlayerLevel == 0)
		{
			cachedPlayerLevel = DataLoader.Instance.GetCurrentPlayerLevel();
		}
		int currentPlayerLevel = DataLoader.Instance.GetCurrentPlayerLevel();
		if (currentPlayerLevel > cachedPlayerLevel)
		{
			onLevelUpFx.Play();
			cachedPlayerLevel = currentPlayerLevel;
		}
		levelText.text = currentPlayerLevel.ToString();
		if (currentPlayerLevel >= 3 && !PlayerPrefs.HasKey(StaticConstants.LeagueTutorialCompleted))
		{
			StartCoroutine(DelayedLeagueTutorial());
		}
		if (currentPlayerLevel == DataLoader.Instance.levelExperience.Length)
		{
			levelText.text = DataLoader.Instance.levelExperience.Length.ToString();
			levelText.resizeTextForBestFit = true;
			levelProgress.fillAmount = 1f;
		}
		else
		{
			levelProgress.fillAmount = (float)((DataLoader.playerData.experience - DataLoader.Instance.levelExperience[currentPlayerLevel - 1]) / (DataLoader.Instance.levelExperience[currentPlayerLevel] - DataLoader.Instance.levelExperience[currentPlayerLevel - 1]));
		}
	}

	public IEnumerator DelayedLeagueTutorial()
	{
		while (GameManager.instance.currentGameMode != 0)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		while (popUpsPanel.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		popUpsPanel.ratingGames.StartTutorial();
	}

	public void NoInternetPanel(bool isConnected)
	{
		popUpsPanel.gameObject.SetActive(!isConnected);
		noInternetPanel.SetActive(!isConnected);
		if (isConnected)
		{
			UnityEngine.Object.FindObjectOfType<TimeManager>().UpdateTime();
			DataLoader.dataUpdateManager.UpdateAfterConnect();
			Time.timeScale = 1f;
			return;
		}
		for (int i = 0; i < popUpsPanel.transform.childCount; i++)
		{
			if (popUpsPanel.transform.GetChild(i) != noInternetPanel.transform)
			{
				popUpsPanel.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		Time.timeScale = 0f;
	}

	public void PopUpsClosed()
	{
		if (!PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted))
		{
			MoneyBoxManager.instance.TrySpawnBox();
			UpgradeTutorialUI.SetActive(true);
			UIController.instance.scrollControllers.survivorsController.scrollRect.enabled = false;
		}
	}

	public void TryToShowInterstirial()
	{
		if (DataLoader.playerData.gamesPlayed > 3 && heroesInQueue.Count == 0 && !isNewWorldOpened)
		{
			isNewWorldOpened = false;
			/*AdsManager.instance.interstitialAdsCounter++;
			if (AdsManager.instance.interstitialAdsCounter >= 1)
			{
				AdsManager.instance.ShowInterstitial();
				AdsManager.instance.interstitialAdsCounter = 0;
			}*/
		}
	}

	public void CheckOpenedHeroes()
	{
		List<SaveData.HeroData> newOpenedHeroes = DataLoader.Instance.GetNewOpenedHeroes();
		if (newOpenedHeroes.Count > 0)
		{
			for (int i = 0; i < DataLoader.Instance.survivors.Count; i++)
			{
				for (int j = 0; j < newOpenedHeroes.Count; j++)
				{
					if (DataLoader.Instance.survivors[i].heroType == newOpenedHeroes[j].heroType && heroesInQueue.Add(DataLoader.Instance.survivors[i]))
					{
						AnalyticsManager.instance.LogEvent("Unlock_" + DataLoader.Instance.survivors[i].heroType, new Dictionary<string, string>());
					}
				}
			}
			if (showLastHeroCor != null)
			{
				StopCoroutine(showLastHeroCor);
			}
			showLastHeroCor = StartCoroutine(DelayedShowOpenedHero());
			UIController.instance.scrollControllers.survivorsController.scrollRect.content.anchoredPosition = new Vector2(0f, UIController.instance.scrollControllers.survivorsController.scrollRect.content.anchoredPosition.y);
			DataLoader.Instance.SavePlayerData();
		}
		UpdateMenuContent();
	}

	private IEnumerator DelayedShowOpenedHero()
	{
		while (GameManager.instance.currentGameMode != 0)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.6f);
		while (popUpsPanel.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		for (int i = 0; i < heroesInQueue.Count; i++)
		{
			do
			{
				yield return null;
			}
			while (popUpsPanel.gameObject.activeInHierarchy);
			popUpsPanel.gameObject.SetActive(true);
			popUpsPanel.openHeroPanel.SetActive(true);
			UIController.instance.scrollControllers.survivorsController.SetOpenedheroIcon(heroesInQueue.ElementAt(i).heroType);
			popUpsPanel.heroName.text = LanguageManager.instance.GetLocalizedText(heroesInQueue.ElementAt(i).name);
			do
			{
				yield return null;
			}
			while (!UIController.instance.scrollControllers.survivorsController.animationCompleted);
		}
		heroesInQueue.Clear();
	}

	public void OnApplicationPause(bool pause)
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP && pause && pauseReady && !GameManager.instance.isTutorialNow && PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted))
		{
			JoystickUI.SetActive(false);
			PauseUI.SetActive(true);
			Time.timeScale = 0f;
			if (resume != null)
			{
				StopCoroutine(resume);
			}
			delayResumeText.transform.parent.gameObject.SetActive(false);
		}
	}

	private void StartTutorial()
	{
		TutorialUI.SetActive(true);
		ChangeAnimationState("Tutorial");
		/*Go();*/
	}

	public void Resume()
	{
		PauseUI.SetActive(false);
		if (resume != null)
		{
			StopCoroutine(resume);
		}
		resume = StartCoroutine(resumeDelay());
	}

	private IEnumerator resumeDelay()
	{
		delayResumeText.transform.parent.gameObject.SetActive(true);
		delayResumeText.transform.localScale = defaultResumeScale;
		Vector3 defaultTextScale = delayResumeText.transform.localScale;
		for (int delayseconds = 3; delayseconds > 0; delayseconds--)
		{
			delayResumeText.text = delayseconds.ToString();
			delayResumeText.transform.localScale = defaultTextScale;
			for (float i = delayseconds; i > (float)(delayseconds - 1); i -= 0.08f)
			{
				delayResumeText.transform.localScale = Vector3.Lerp(delayResumeText.transform.localScale, Vector3.zero, 0.05f);
				yield return new WaitForSecondsRealtime(0.02f);
			}
		}
		JoystickUI.SetActive(true);
		Time.timeScale = 1f;
		delayResumeText.transform.parent.gameObject.SetActive(false);
		delayResumeText.transform.localScale = defaultTextScale;
	}

	public void Go()
	{
		PlayUI.SetActive(true);
		JoystickUI.SetActive(true);
		UIController.instance.scrollControllers.wantedListController.PrepareForGameplay();
		GameManager.instance.Go();
		UIController.instance.scrollControllers.survivorsController.ActivateHeroCams(false);
		if (!GameManager.instance.isTutorialNow && !PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted))
		{
			psSurv.gameObject.SetActive(true);
			psKillAll.gameObject.SetActive(true);
			AbilityTutorialUI.SetActive(true);
		}
		pauseReady = true;
		textBestScore.text = DataLoader.playerData.bestScore.ToString();
		RefreshBossUI();
		PrepareBoosters();
	}

	public void GoPVP()
	{
		PlayUI.SetActive(true);
		JoystickUI.SetActive(true);
		GameManager.instance.LoadPVPArena();
		UIController.instance.scrollControllers.survivorsController.ActivateHeroCams(false);
		UIController.instance.scrollControllers.wantedListController.PrepareForGameplay();
		popUpsPanel.gameObject.SetActive(false);
		PrepareBoosters();
	}

	public void GoArena()
	{
		PlayUI.SetActive(true);
		JoystickUI.SetActive(true);
		animatorApocalypseUI.SetBool("Apocalypse", false);
		GameManager.instance.GoArena();
		UIController.instance.scrollControllers.survivorsController.ActivateHeroCams(false);
		UIController.instance.scrollControllers.wantedListController.PrepareForGameplay();
		popUpsPanel.gameObject.SetActive(false);
		AnalyticsManager.instance.LogEvent("CarFuelingStart", new Dictionary<string, string>());
		PrepareBoosters();
	}

	private void PrepareBoosters()
	{
		Vector2 vector = new Vector2(25f, 25f);
		if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
		{
			incSpeedRect.gameObject.SetActive(true);
			killAllRect.gameObject.SetActive(false);
			newSurvivorRect.gameObject.SetActive(false);
			incSpeedRect.anchorMin = new Vector2(0f, 0f);
			incSpeedRect.anchorMax = new Vector2(0f, 0f);
			incSpeedRect.pivot = new Vector2(0f, 0f);
			incSpeedRect.anchoredPosition = vector;
		}
		else if (GameManager.instance.currentGameMode == GameManager.GameModes.Arena)
		{
			incSpeedRect.gameObject.SetActive(true);
			killAllRect.gameObject.SetActive(false);
			newSurvivorRect.gameObject.SetActive(true);
			incSpeedRect.anchorMin = new Vector2(0f, 0f);
			incSpeedRect.anchorMax = new Vector2(0f, 0f);
			incSpeedRect.pivot = new Vector2(0f, 0f);
			incSpeedRect.anchoredPosition = vector;
			newSurvivorRect.anchorMin = new Vector2(1f, 0f);
			newSurvivorRect.anchorMax = new Vector2(1f, 0f);
			newSurvivorRect.pivot = new Vector2(1f, 0f);
			newSurvivorRect.anchoredPosition = vector * new Vector2(-1f, 1f);
		}
		else
		{
			newSurvivorRect.gameObject.SetActive(true);
			killAllRect.gameObject.SetActive(GameManager.instance.currentGameMode != GameManager.GameModes.Arena);
			incSpeedRect.gameObject.SetActive(GameManager.instance.currentGameMode == GameManager.GameModes.Arena);
			incSpeedRect.anchorMin = new Vector2(1f, 0f);
			incSpeedRect.anchorMax = new Vector2(1f, 0f);
			incSpeedRect.pivot = new Vector2(1f, 0f);
			newSurvivorRect.anchorMin = new Vector2(1f, 0f);
			newSurvivorRect.anchorMax = new Vector2(1f, 0f);
			newSurvivorRect.pivot = new Vector2(1f, 0f);
			killAllRect.anchorMin = new Vector2(0f, 0f);
			killAllRect.anchorMax = new Vector2(0f, 0f);
			killAllRect.pivot = new Vector2(0f, 0f);
			killAllRect.anchoredPosition = vector;
			newSurvivorRect.anchoredPosition = vector * new Vector2(-1f, 1f);
			incSpeedRect.anchoredPosition = vector;
		}
		BasePointer.bottomOffset = SetCanvasBounds.Instance.offset;
		BasePointer.topOffset = SetCanvasBounds.Instance.offset;
		killAllRect.GetComponent<AbilityButton>().Reset();
		newSurvivorRect.GetComponent<AbilityButton>().Reset();
		incSpeedRect.GetComponent<AbilityButton>().Reset();
	}

	public void StartArena()
	{
		pauseReady = true;
		OnApplicationPause(true);
		Resume();
		carPointer.StartPointer();
		fuelPointer.StartPointer();
	}

	public void StartPVP()
	{
		pauseReady = true;
		OnApplicationPause(true);
		Resume();
		pvpPointer.StartPointer();
	}

	public void AbilityTutorialComplete()
	{
		AbilityTutorialUI.SetActive(false);
		PlayerPrefs.SetInt(StaticConstants.AbilityTutorialCompleted, 1);
		PlayerPrefs.Save();
		WavesManager.instance.StartGame();
		psSurv.gameObject.SetActive(false);
		psKillAll.gameObject.SetActive(false);
	}

	public void UpgradeTutorialComplete()
	{
		if (!PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted))
		{
			UIController.instance.scrollControllers.survivorsController.scrollRect.enabled = true;
			UpgradeTutorialUI.SetActive(false);
			UIController.instance.scrollControllers.survivorsController.UpdateAllContent();
			PlayerPrefs.SetInt(StaticConstants.UpgradeTutorialCompleted, 1);
			PlayerPrefs.Save();
		}
	}

	public void WinArena()
	{
		GameOver();
	}

	public void GameOver()
	{
		pauseReady = false;
		SoundManager.Instance.PlayStepsSound(false);
		JoystickUI.SetActive(false);
		UpdateMoney();
		UIController.instance.scrollControllers.survivorsController.ActivateHeroCams(true);
		if (GameManager.instance.isTutorialNow)
		{
			gameOverManager.SetGameOverMenu(200.0, 1, 5, 1L, GameManager.instance.inGameTime);
			ResetTrigger("Tutorial");
			ChangeAnimationState("TutorialCompleted");
		}
		else
		{
			gameOverManager.presentController.TryToShowPresent((int)DataLoader.Instance.inGameMoneyCounter);
		}
	}

	private void CheckForNewWorld()
	{
		if (GameManager.instance.IsWorldOpen(GameManager.instance.currentWorldNumber + 1) && (!PlayerPrefs.HasKey(StaticConstants.GoToNextWorldPopUpShowed) || PlayerPrefs.GetInt(StaticConstants.GoToNextWorldPopUpShowed) <= GameManager.instance.currentWorldNumber))
		{
			NextWorldReadyUI.SetActive(true);
			nextWorldReadyHand.SetTrigger("Click");
			AnalyticsManager.instance.LogEvent("NewWorldOpened", new Dictionary<string, string>());
			isNewWorldOpened = true;
		}
		else
		{
			isNewWorldOpened = false;
		}
		RefreshWorldsButtons();
	}

	public void MainMenu()
	{
		GameManager.instance.Reset();
		CheckOpenedHeroes();
		PauseUI.SetActive(false);
		DataLoader.dataUpdateManager.UpdateAfterConnect();
		DataLoader.gui.gameOverManager.gameOverOpened = false;
		pauseReady = false;
		SoundManager.Instance.soundVolume = 1f;
		CheckForNewWorld();
		UpdateMoney();
		UIController.instance.scrollControllers.wantedListController.UpdateAll();
	}

	private void Update()
	{
		textPoints.text = Mathf.RoundToInt(GameManager.instance.Points).ToString();
		textNewSurvivorsLeft.text = GameManager.instance.newSurvivorsLeft.ToString();
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		if (GameManager.instance.currentGameMode == GameManager.GameModes.GamePlay || GameManager.instance.currentGameMode == GameManager.GameModes.Arena)
		{
			if (PauseUI.activeInHierarchy)
			{
				Resume();
			}
			else
			{
				OnApplicationPause(true);
			}
		}
		else if (!popUpsPanel.gameObject.activeInHierarchy && !gameOverManager.gameOverOpened)
		{
			popUpsPanel.gameObject.SetActive(true);
			popUpsPanel.exitPanel.SetActive(true);
		}
	}

	public void GetOfflineMoney()
	{
		double num = DataLoader.dataUpdateManager.LoadOfflineTime();
		float num2 = 0f;
		popUpsPanel.starterPack.starterMenu.SetActive(GameManager.instance.IsTutorialCompleted());
		num2 = 0f;
		for (int i = 0; i < DataLoader.playerData.heroData.Count; i++)
		{
			num2 += DataLoader.Instance.GetHeroPower(DataLoader.playerData.heroData[i].heroType);
		}
		if (num > 30.0)
		{
			Debug.Log("Offline gold: " + (float)(num * (double)num2 * (double)StaticConstants.OfflineGoldConst) + "\nSeconds: " + num);
		}
		if (GameManager.instance.currentGameMode != 0)
		{
			return;
		}
		if (PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted) && num / 60.0 > (double)StaticConstants.MinOfflineMinutes && heroesInQueue.Count == 0)
		{
			if (num / 60.0 > (double)StaticConstants.MaxOfflineMinutes)
			{
				Debug.Log("Minutes > " + StaticConstants.MaxOfflineMinutes);
				num = StaticConstants.MaxOfflineMinutes * 60f;
			}
			popUpsPanel.DisablePopupsWithoutBg();
			objButtonOfflineOk.anchoredPosition = new Vector2(-200f, objButtonOfflineOk.anchoredPosition.y);
			offlineX2.image.rectTransform.anchoredPosition = new Vector2(200f, offlineX2.image.rectTransform.anchoredPosition.y);
			popUpsPanel.gameObject.SetActive(true);
			offlineX2.gameObject.SetActive(true);
			offlinePanel.SetActive(true);
			double f = Math.Floor((float)(num * (double)num2 * (double)StaticConstants.OfflineGoldConst * (double)(1f + PassiveAbilitiesManager.bonusHelper.GoldBonus)) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus) * (1f + PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus) * (1f + PassiveAbilitiesManager.bonusHelper.GetCriticalHitChance()));
			if (InAppManager.Instance.IsSubscribed())
			{
				f *= 1.5;
			}
			offlineRewardText.text = AbbreviationUtility.AbbreviateNumber(f);
			DataLoader.Instance.RefreshMoney(f, true);
			offlineX2.interactable = true;
			offlineX2.onClick.RemoveAllListeners();
			offlineX2.onClick.AddListener(delegate
			{
                StartCoroutine(DelayedX3(f));
              /*  AdsManager.instance.ShowRewarded(delegate
				{
				
				}, AdsManager.AdName.RewardX3Offline);*/
			});
			DataLoader.dataUpdateManager.SaveOfflineTime();
		}
		else if (num > 900.0)
		{
			Debug.Log("Sorry, offline time < " + StaticConstants.MinOfflineMinutes + " min, money won't be scored. You can change minimal time in StaticConstants.cs -> MinOfflineMinutes");
			if (!popUpsPanel.starterPack.autoShowCompleted)
			{
				popUpsPanel.starterPack.TryToShowPack();
			}
		}
	}

	private IEnumerator DelayedX3(double money)
	{
		DataLoader.Instance.RefreshMoney(money * 2.0, true);
		yield return new WaitForSeconds(0.5f);
		objButtonOfflineOk.anchoredPosition = new Vector2(0f, objButtonOfflineOk.anchoredPosition.y);
		offlineX2.gameObject.SetActive(false);
		StartCoroutine(OfflineCounter(money, money * 2.0));
		AnalyticsManager.instance.LogEvent("OfflileX3", new Dictionary<string, string> { 
		{
			"Money",
			money.ToString()
		} });
	}

	private IEnumerator OfflineCounter(double currentMoney, double targetMoney)
	{
		offlineRewardText.text = currentMoney.ToString();
		float speed = 12f;
		for (int i = 1; (float)i <= speed; i++)
		{
			yield return new WaitForSeconds(0.05f);
			offlineRewardText.text = (currentMoney + Math.Ceiling(targetMoney / (double)speed * (double)i)).ToString();
		}
	}

	public void RefreshBossUI()
	{
		fillProgressToBoss.transform.parent.gameObject.SetActive(true);
		fillBossHealth.transform.parent.gameObject.SetActive(false);
		topPanelAnimator.SetBool("HideAll", false);
		RefreshProgressToBoss();
	}

	public void SetTopPanelAnimationState(bool bossInDaHause)
	{
		if (bossInDaHause)
		{
			progressToBossFx.Play();
			fillBossHealth.transform.parent.gameObject.SetActive(true);
		}
		topPanelAnimator.SetBool("BossInDaHause", bossInDaHause);
	}

	public void RefreshFuelTankFill(int capacity, int value)
	{
	}

	public void RefreshArenaTimeRemaining(int seconds, bool setFull = false)
	{
		if (setFull)
		{
			fillFuelTank.rectTransform.sizeDelta = new Vector2(maxWidthFuelTank, fillFuelTank.rectTransform.sizeDelta.y);
			animatorApocalypseUI.SetBool("Apocalypse", true);
			return;
		}
		animatorApocalypseUI.SetBool("Apocalypse", false);
		if (seconds == ArenaWavesManager.instance.timeBeforeStart)
		{
			fillFuelTank.rectTransform.sizeDelta = new Vector2(5f, fillFuelTank.rectTransform.sizeDelta.y);
			return;
		}
		if (!fillFuelTank.transform.parent.gameObject.activeSelf)
		{
			fillFuelTank.transform.parent.gameObject.SetActive(true);
		}
		fillFuelTank.rectTransform.sizeDelta = new Vector2((1f - (float)seconds / (float)ArenaWavesManager.instance.timeBeforeStart) * maxWidthFuelTank, fillFuelTank.rectTransform.sizeDelta.y);
	}

	public void InitCanistersUI(int countCanisters)
	{
		if (imageCanisters == null)
		{
			imageCanisters = new List<Image>();
		}
		while (imageCanisters.Count > countCanisters)
		{
			UnityEngine.Object.Destroy(imageCanisters[imageCanisters.Count - 1].gameObject);
			imageCanisters.RemoveAt(imageCanisters.Count - 1);
		}
		for (int i = imageCanisters.Count; i < countCanisters; i++)
		{
			imageCanisters.Add(UnityEngine.Object.Instantiate(imageCanister, imageCanister.transform.parent));
		}
		for (int j = 0; j < countCanisters; j++)
		{
			imageCanisters[j].gameObject.SetActive(true);
			imageCanisters[j].color = new Color(1f, 1f, 1f, 0.3f);
			imageCanisters[j].transform.localPosition = new Vector3(80f * ((float)j - (float)countCanisters / 2f) + 40f, 0f, 0f);
		}
		imageCanister.gameObject.SetActive(false);
		imageWeight1.enabled = false;
		imageWeight2.enabled = false;
	}

	public void RefreshCarCaravanProgress()
	{
		if (CarControll.countGasInHands <= 0)
		{
			StartCoroutine(RefreshCanistersUI());
		}
		else
		{
			imageCanisters[CarControll.countGasInHands - 1].GetComponentInChildren<ParticleSystem>().Play();
			imageCanisters[CarControll.countGasInHands - 1].color = new Color(1f, 1f, 1f, 1f);
		}
		float num = CarControll.countGasInHands - 1;
		if (num < 0f)
		{
			num = 0f;
		}
		num *= 0.12f;
		SurvivorHuman.moveForceArenaMultiplier = 1f - num;
		imageWeight1.enabled = CarControll.countGasInHands == 2;
		imageWeight2.enabled = CarControll.countGasInHands == 3;
	}

	private IEnumerator RefreshCanistersUI()
	{
		if (imageCanisters == null)
		{
			yield break;
		}
		int countActiveCanisters = 0;
		foreach (Image imgcan in imageCanisters)
		{
			if (!imgcan.gameObject.activeSelf)
			{
				continue;
			}
			if (imgcan.color == new Color(1f, 1f, 1f, 1f))
			{
				while (imgcan.transform.localScale != Vector3.zero)
				{
					imgcan.transform.localScale = Vector3.MoveTowards(imgcan.transform.localScale, Vector3.zero, Time.deltaTime * 8f);
					yield return null;
				}
				UnityEngine.Object.Destroy(imgcan.gameObject);
			}
			else
			{
				countActiveCanisters++;
			}
		}
		imageCanisters.RemoveRange(0, imageCanisters.Count - countActiveCanisters);
		for (int i = 0; i < imageCanisters.Count; i++)
		{
			Vector3 newPos = new Vector3(80f * ((float)i - (float)imageCanisters.Count / 2f) + 40f, 0f, 0f);
			while (imageCanisters[i].transform.localPosition != newPos)
			{
				imageCanisters[i].transform.localPosition = Vector3.MoveTowards(imageCanisters[i].transform.localPosition, newPos, Time.deltaTime * 100f);
				yield return null;
			}
		}
	}

	public void RefreshProgressToBoss()
	{
		if (!WavesManager.instance.bossInDaHause)
		{
			fillProgressToBoss.rectTransform.sizeDelta = new Vector2((GameManager.instance.Points - (float)WavesManager.instance.GetBossDeadAtPoints()) / (float)WavesManager.instance.GetNextBossTargetPoints() * maxWidthProgressToBoss, fillProgressToBoss.rectTransform.sizeDelta.y);
		}
		else
		{
			fillProgressToBoss.rectTransform.sizeDelta = new Vector2(maxWidthProgressToBoss, fillProgressToBoss.rectTransform.sizeDelta.y);
		}
	}

	public void RefreshBossHealth(float maxCountHealth, float countHealth, string name = "")
	{
		if (countHealth <= 0f)
		{
			if (WavesManager.instance.GetNextBossTargetPoints() > 0)
			{
				fillProgressToBoss.rectTransform.sizeDelta = new Vector2(0f, fillProgressToBoss.rectTransform.sizeDelta.y);
			}
			else
			{
				topPanelAnimator.SetBool("HideAll", true);
			}
		}
		if (WavesManager.instance.bossInDaHause)
		{
			fillBossHealth.rectTransform.sizeDelta = new Vector2(countHealth / maxCountHealth * maxWidthBossHealth, fillBossHealth.rectTransform.sizeDelta.y);
			bossNameText.text = LanguageManager.instance.GetLocalizedText(name);
			bossNameText.font = LanguageManager.instance.currentLanguage.font;
		}
		else
		{
			fillBossHealth.rectTransform.sizeDelta = new Vector2(0f, fillBossHealth.rectTransform.sizeDelta.y);
		}
	}

	public void RefreshWorldsButtons()
	{
		if (GameManager.instance.currentWorldNumber > 1)
		{
			buttonPrevWorld.SetActive(true);
		}
		else
		{
			buttonPrevWorld.SetActive(false);
		}
		if (GameManager.instance.currentWorldNumber < GameManager.instance.GetWorldsCount())
		{
			buttonNextWorld.SetActive(true);
		}
		else
		{
			buttonNextWorld.SetActive(false);
		}
		if (GameManager.instance.bossKillsForOpenWorld[1] - DataLoader.playerData.GetZombieByType(SaveData.ZombieData.ZombieType.BOSS).totalTimesKilled > 0)
		{
			textButtonNewLocation.text = GameManager.instance.bossKillsForOpenWorld[1] - DataLoader.playerData.GetZombieByType(SaveData.ZombieData.ZombieType.BOSS).totalTimesKilled + string.Empty;
			textCount1.text = textButtonNewLocation.text;
			textCount2.text = textButtonNewLocation.text;
			textButtonNewLocation.gameObject.SetActive(true);
			nextLocationImage.sprite = UIController.instance.multiplyImages.nextLocationButton.inactive;
		}
		else
		{
			textButtonNewLocation.gameObject.SetActive(false);
			nextLocationImage.sprite = UIController.instance.multiplyImages.nextLocationButton.active;
		}
		if (showWorldName != null)
		{
			StopCoroutine(showWorldName);
		}
		showWorldName = StartCoroutine(ShowWorldName());
	}

	private IEnumerator ShowWorldName()
	{
		textWorldName.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		textWorldName.gameObject.SetActive(true);
		textWorldName.text = LanguageManager.instance.GetLocalizedText(GameManager.instance.worldNames[GameManager.instance.currentWorldNumber - 1]);
		textWorldName.font = LanguageManager.instance.currentLanguage.font;
		textWorldName.color -= new Color(0f, 0f, 0f, textWorldName.color.a);
		while (textWorldName.color.a < 1f)
		{
			textWorldName.color += new Color(0f, 0f, 0f, Time.deltaTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		while (textWorldName.color.a > 0f)
		{
			textWorldName.color -= new Color(0f, 0f, 0f, Time.deltaTime);
			yield return null;
		}
		textWorldName.gameObject.SetActive(false);
		showWorldName = null;
	}

	public void GoToNextWorld()
	{
		int num = GameManager.instance.ChangeWorld(1);
		if (num > 0)
		{
			popUpsPanel.gameObject.SetActive(true);
			popUpsPanel.closedWorldPanel.SetActive(true);
			StartCoroutine(UIController.instance.Scale(popUpsPanel.closedWorldPanel.transform));
			Text[] bossKillsRemainingTexts = popUpsPanel.bossKillsRemainingTexts;
			foreach (Text text in bossKillsRemainingTexts)
			{
				text.text = num.ToString();
			}
		}
		else
		{
			RefreshWorldsButtons();
			if (NextWorldReadyUI.activeSelf)
			{
				NextWorldReadyUI.SetActive(false);
				PlayerPrefs.SetInt(StaticConstants.GoToNextWorldPopUpShowed, GameManager.instance.currentWorldNumber);
				PlayerPrefs.Save();
			}
		}
	}

	public void GoToPrevWorld()
	{
		GameManager.instance.ChangeWorld(-1);
		RefreshWorldsButtons();
	}

	public void GoToDailyBoss()
	{
		PlayUI.SetActive(true);
		JoystickUI.SetActive(true);
		ChangeAnimationState("Game");
		GameManager.instance.GoDailyBoss();
		pauseReady = true;
		textBestScore.text = DataLoader.playerData.bestScore.ToString();
		RefreshBossUI();
	}

	public void DailyBossComplete()
	{
		SoundManager.Instance.PlayStepsSound(false);
		JoystickUI.SetActive(false);
		PlayUI.SetActive(false);
		ChangeAnimationState("GameOver");
		pauseReady = false;
	}

	public void Loading(bool state)
	{
		if (state)
		{
			loadingScreen.StartLoading();
		}
		else
		{
			loadingScreen.EndLoading();
		}
	}

	public void LogClickEvent(string eventName)
	{
		AnalyticsManager.instance.LogEvent(eventName, new Dictionary<string, string>());
	}
}
