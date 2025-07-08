using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ACEPlay.Bridge;
using DG.Tweening;
using GuiInGame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SurviviorContent : UIScrollCell
{
    public HeroInfo heroInfo;

    [SerializeField]
    private Image heroIcon;

    [SerializeField]
    private Text heroNameText;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Text levelText;

    [SerializeField]
    private Button upgradeButton;

    public RawImage rawImage;

    [SerializeField]
    private ParticleSystem upgradeFX;

    [SerializeField]
    private Animator upgradeButtonAnimator;

    [SerializeField]
    private RectTransform imageLevel;

    [SerializeField]
    private Image imagePerkFillBar;

    [SerializeField]
    private Image imagePerk;

    [SerializeField]
    private Image imageUpgradeButton;

    [SerializeField]
    private Text newLevelPowerText;

    [SerializeField]
    private GameObject newLevelPower;

    public GameObject fakeVideoUpgrade;

    [SerializeField]
    private Image imageInfoShining;

    [SerializeField]
    private RectTransform powerPlashKa;

    [SerializeField]
    private RectTransform buttonInfoRect;

    [Header("Locked")]
    [SerializeField]
    private Text levelRequiredText;

    [SerializeField]
    private GameObject levelRequired;

    [SerializeField]
    private GameObject lockObj;

    [SerializeField]
    private GameObject lockedName;

    [SerializeField]
    private Text lockedNameText;

    [SerializeField]
    private GameObject currentlevel;

    [SerializeField]
    private GameObject infoButton;

    [SerializeField]
    private GameObject objPurchaseHero;

    private List<Survivors.SurvivorLevels> levelsInfo;

    [NonSerialized]
    public SaveData.HeroData heroData;

    private SaveData.HeroData.HeroType type;

    [Space]
    private Vector2 startSize;

    public float speed = 200f;

    public float scale = 1.2f;

    public ParticleSystem unlockFX;

    private Camera heroCamera;

    private Animator anim;

    [HideInInspector]
    public bool canMakeVideoUpgrade;

    private Coroutine dayHeroCor;

    [Header("Selected")]
    [SerializeField]
    private GameObject selecteObj;
    [SerializeField]
    private GameObject selectedText;
    [SerializeField]
    private Text txtNotif;
    private void Start()
    {
        SetUpSelected(DataLoader.Instance.survivorHumansSelected?.ToList());
    }

    public override void SetContent(int index)
    {
        base.SetContent(index);
        heroIcon.sprite = DataLoader.Instance.survivors[index].icon;
        levelsInfo = DataLoader.Instance.survivors[index].levels;
        type = DataLoader.Instance.survivors[index].heroType;
        RenderTexture renderTexture = new RenderTexture(UIController.instance.scrollControllers.survivorsController.renderTextureSurvivorPrefab);
        rawImage.texture = renderTexture;
        float num = 2f * UIController.instance.scrollControllers.survivorsController.heroCamPrefab.orthographicSize * UIController.instance.scrollControllers.survivorsController.heroCamPrefab.aspect + 1f;
        heroCamera = UnityEngine.Object.Instantiate(UIController.instance.scrollControllers.survivorsController.heroCamPrefab, new Vector3(10000f + (float)index * num, 0f, 0f), Quaternion.identity, TransformParentManager.Instance.upgradePanelCams);
        SurvivorHuman survivorHuman = UnityEngine.Object.Instantiate(DataLoader.Instance.survivors[index].survivorPrefab, heroCamera.transform.GetChild(0));
        SkinnedMeshRenderer componentInChildren = survivorHuman.GetComponentInChildren<SkinnedMeshRenderer>();
        Renderer[] componentsInChildren = survivorHuman.GetComponentsInChildren<MeshRenderer>();
        componentInChildren.materials[0] = new Material(componentInChildren.materials[0]);
        componentInChildren.materials[0].SetFloat("_Outline", 0.04f);
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            componentsInChildren[i].materials[0] = new Material(componentsInChildren[i].materials[0]);
            componentsInChildren[i].materials[0].SetFloat("_Outline", 0.04f);
        }
        Component[] components = survivorHuman.gameObject.GetComponents<Component>();
        foreach (Component component in components)
        {
            if (!(component is Transform))
            {
                UnityEngine.Object.Destroy(component);
            }
        }
        heroData = DataLoader.playerData.heroData.FirstOrDefault((SaveData.HeroData hd) => hd.heroType == DataLoader.Instance.survivors[base.cellIndex].heroType);
        survivorHuman.transform.Rotate(new Vector3(0f, 180f, 0f));
        anim = survivorHuman.GetComponentInChildren<Animator>();
        anim.SetBool("Rest", false);
        anim.SetBool("Run", false);
        if (heroData.heroType == SaveData.HeroData.HeroType.SMG)
        {
            anim.SetBool("Shoot", true);
        }
        heroCamera.targetTexture = renderTexture;
        UpdateContent();
        startSize = imageLevel.sizeDelta;
        SetVideoButton(false);
        StartCoroutine(WaitForSurvivorStendUp());
    }

    private IEnumerator WaitForSurvivorStendUp()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Rest"))
        {
            yield return null;
        }
        yield return null;
        heroCamera.Render();
        ActivateCamera(false);
    }

    public void SetLocalizedText()
    {
        heroNameText.text = LanguageManager.instance.GetLocalizedText(DataLoader.Instance.survivors[base.cellIndex].name);
        lockedNameText.text = heroNameText.text;
        if (DataLoader.Instance.survivors[base.cellIndex].heroOpenType == HeroOpenType.Level)
        {
            levelRequiredText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Level) + " " + DataLoader.Instance.survivors[base.cellIndex].requiredLevelToOpen;
        }
        levelRequiredText.font = LanguageManager.instance.currentLanguage.font;
    }

    private IEnumerator DayHeroTimer(TimeSpan timeSpan)
    {
        do
        {
            yield return null;
        }
        while (!TimeManager.gotDateTime);
        while (timeSpan.TotalSeconds > 0.0)
        {
            if (timeSpan.Days > 0)
            {
                levelRequiredText.text = string.Format("{0:D1} {1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                levelRequiredText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1.0));
            yield return new WaitForSecondsRealtime(1f);
        }
        DataLoader.gui.CheckOpenedHeroes();
    }

    public void Upgrade(bool rewarded)
    {
        if (!heroData.isOpened && heroData.currentLevel < DataLoader.playerData.survivorMaxLevel)
        {
            return;
        }
        if (rewarded)
        {
            UnityEvent onDone = new UnityEvent();
            onDone.AddListener(delegate
            {
                StartCoroutine(DelayedVideoUpgrade());
                AnalyticsManager.instance.LogEvent("RewardedHeroUpgrade", new Dictionary<string, string>
                {
                   {
                       "HeroType",
                       DataLoader.playerData.heroData[base.cellIndex].heroType.ToString()
                   },
                   {
                       "Level",
                       DataLoader.playerData.heroData[base.cellIndex].currentLevel.ToString()
                   }
                });
            });
            BridgeController.instance.ShowRewarded($"Show rewarded:{AdName.RewardUpgradeHero}", onDone);
            /*AdsManager.instance.ShowRewarded(delegate
			{
				
			}, AdsManager.AdName.RewardUpgradeHero);*/
        }
        else if (DataLoader.Instance.RefreshMoney(-levelsInfo[heroData.currentLevel].cost))
        {
            IncreaseLevel(false);
            AnalyticsManager.instance.LogEvent("HeroUpgrade", new Dictionary<string, string>
            {
                {
                    "HeroType",
                    DataLoader.playerData.heroData[base.cellIndex].heroType.ToString()
                },
                {
                    "Level",
                    DataLoader.playerData.heroData[base.cellIndex].currentLevel.ToString()
                }
            });
        }
        else
        {
            DataLoader.gui.popUpsPanel.notEnoughMoney.Open(this, (double)levelsInfo[heroData.currentLevel].cost - DataLoader.playerData.money);
        }
    }

    private IEnumerator DelayedVideoUpgrade()
    {
        IncreaseLevel(true);
        yield return new WaitForSeconds(0.5f);
        SetVideoButton(false);
        if (heroInfo.gameObject.activeInHierarchy)
        {
            heroInfo.UpdateInfo();
        }
    }

    public void ActivateCamera(bool active)
    {
        active = false;
        heroCamera.enabled = active;
    }

    public void IncreaseLevel(bool rewarded)
    {
        if (!heroInfo.gameObject.activeInHierarchy)
        {
            StartCoroutine(LevelScaler());
        }
        SoundManager.Instance.PlaySound(SoundManager.Instance.upgradeSound);
        heroData.currentLevel++;
        for (int i = 0; i < DataLoader.playerData.heroData.Count; i++)
        {
            if (DataLoader.playerData.heroData[i].heroType == DataLoader.Instance.survivors[base.cellIndex].heroType)
            {
                DataLoader.playerData.heroData[i] = heroData;
                break;
            }
        }
        UpdateContent();
        DataLoader.Instance.UpdateIdleHero(heroData.heroType);
        DataLoader.gui.UpdateMenuContent();
        DataLoader.Instance.SavePlayerData();
        DataLoader.gui.UpgradeTutorialComplete();
        if (heroInfo.surviviorContent == this && heroInfo.gameObject.activeInHierarchy)
        {
            heroInfo.PlayFx(rewarded);
            heroInfo.UpdateInfo();
        }
    }

    public void OpenHeroInfo(bool isLocked)
    {
        if (PlayerPrefs.HasKey(StaticConstants.UpgradeTutorialCompleted))
        {
            heroInfo.SetContent(base.cellIndex, rawImage.texture, isLocked, this);
            DataLoader.gui.popUpsPanel.gameObject.SetActive(true);
            heroInfo.gameObject.SetActive(true);
            UpdateInactiveButton();
        }
    }

    private IEnumerator LevelScaler()
    {
        while (imageLevel.sizeDelta.x < startSize.x * scale)
        {
            imageLevel.sizeDelta += new Vector2(Time.deltaTime * speed, Time.deltaTime * speed);
            yield return null;
        }
        upgradeFX.Play();
        while (imageLevel.sizeDelta.x > startSize.x)
        {
            imageLevel.sizeDelta -= new Vector2(Time.deltaTime * speed, Time.deltaTime * speed);
            yield return null;
        }
        imageLevel.sizeDelta = startSize;
    }

    public void UpdateContent()
    {
        heroData = DataLoader.playerData.heroData.FirstOrDefault((SaveData.HeroData hd) => hd.heroType == DataLoader.Instance.survivors[base.cellIndex].heroType);
        SetLocalizedText();
        DisableCharacter(heroData.isOpened);
        if (heroData.currentLevel >= DataLoader.playerData.survivorMaxLevel)
        {
            SetVideoButton(false);
        }
        if (upgradeButtonAnimator.gameObject.activeInHierarchy)
        {
            upgradeButtonAnimator.SetBool("Active", false);
        }
        if (heroData.currentLevel == DataLoader.playerData.survivorMaxLevel)
        {
            costText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Max_Maximum);
            costText.font = LanguageManager.instance.currentLanguage.font;
        }
        else
        {
            UpdateInactiveButton();
            costText.text = AbbreviationUtility.AbbreviateNumber(levelsInfo[heroData.currentLevel].cost);
        }
        int currentLevel = heroData.currentLevel;
        SetPerk(currentLevel);
        float num = (1f + PassiveAbilitiesManager.bonusHelper.GetCriticalHitChance()) * (1f + PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus);
        if (currentLevel < DataLoader.playerData.survivorMaxLevel && heroData.isOpened)
        {
            newLevelPowerText.text = "<color=#ffffff>" + AbbreviationUtility.AbbreviateNumber(GetLevelPower(heroData.currentLevel) * num, 2) + "</color>+" + AbbreviationUtility.AbbreviateNumber((GetLevelPower(heroData.currentLevel + 1) - GetLevelPower(heroData.currentLevel)) * num, 2);
        }
        else
        {
            newLevelPowerText.text = "<color=#ffffff>" + AbbreviationUtility.AbbreviateNumber(GetLevelPower(heroData.currentLevel) * num, 2) + "</color>";
        }
        levelText.text = currentLevel.ToString();
        if (DataLoader.Instance.survivors[base.cellIndex].heroOpenType == HeroOpenType.Day && !heroData.isOpened)
        {
            TimeSpan timeSpan = DataLoader.playerData.firstEnterDate.Add(TimeSpan.FromDays(DataLoader.Instance.survivors[base.cellIndex].daysToOpen)).Subtract(TimeManager.CurrentDateTime);
            if (dayHeroCor != null)
            {
                StopCoroutine(dayHeroCor);
            }
            dayHeroCor = StartCoroutine(DayHeroTimer(timeSpan));
        }
    }

    public void DisableCharacter(bool isOpened)
    {
        lockObj.SetActive(!isOpened);
        selecteObj.SetActive(isOpened);
        upgradeButton.gameObject.SetActive(isOpened);
        currentlevel.SetActive(isOpened);
        levelRequired.SetActive(!isOpened);
        lockedName.SetActive(!isOpened);
        objPurchaseHero.SetActive(!isOpened);
        newLevelPower.gameObject.SetActive(isOpened);
    }

    public void SetPerk(int currentLevel)
    {
        imagePerk.gameObject.SetActive(currentLevel > 24);
        imagePerkFillBar.fillAmount = (float)(currentLevel % 25) / 25f;
        for (int i = 25; i <= 150; i += 25)
        {
            if (currentLevel >= i)
            {
                imagePerk.sprite = UIController.instance.multiplyImages.perks[(i - 1) / 25].active;
            }
        }
        imagePerk.SetNativeSize();
    }

    public void UpdateInactiveButton()
    {
        if (heroData.currentLevel < DataLoader.playerData.survivorMaxLevel && IsEnoughMoney() && heroData.isOpened)
        {
            costText.color = Color.white;
            if (upgradeButtonAnimator.gameObject.activeInHierarchy)
            {
                upgradeButtonAnimator.SetBool("Active", true);
            }
            imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[0].active;
            upgradeButton.interactable = true;
            if (heroInfo.surviviorContent == this)
            {
                heroInfo.imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[3].active;
                heroInfo.SetUpgradeButtonInteractable(true);
            }
            return;
        }
        if (heroData.currentLevel >= DataLoader.playerData.survivorMaxLevel)
        {
            upgradeButton.interactable = false;
            imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[0].inactive;
            if (heroInfo.surviviorContent == this)
            {
                heroInfo.imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[3].inactive;
                heroInfo.SetUpgradeButtonInteractable(false);
            }
        }
        else
        {
            imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[0].active;
            if (heroInfo.surviviorContent == this)
            {
                heroInfo.imageUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[3].active;
                heroInfo.SetUpgradeButtonInteractable(false);
            }
        }
        if (upgradeButtonAnimator.gameObject.activeInHierarchy)
        {
            upgradeButtonAnimator.SetBool("Active", false);
        }
    }

    public float GetLevelPower(int level)
    {
        if (level < 1)
        {
            level = 1;
        }
        return DataLoader.Instance.survivors[base.cellIndex].levels[level - 1].power * 10f;
    }

    public bool SetVideoButton(bool active)
    {
        if (heroData.isOpened && heroData.currentLevel < DataLoader.playerData.survivorMaxLevel && active)
        {
            fakeVideoUpgrade.SetActive(true);
            if (!canMakeVideoUpgrade)
            {
                StartCoroutine(SetPowerPlashkaSize(true));
                canMakeVideoUpgrade = true;
            }
            return true;
        }
        fakeVideoUpgrade.SetActive(false);
        canMakeVideoUpgrade = false;
        StartCoroutine(SetPowerPlashkaSize(false));
        return false;
    }

    public IEnumerator SetPowerPlashkaSize(bool open)
    {
        float powerplashkaspeed = 550f;
        if (open)
        {
            powerPlashKa.sizeDelta = new Vector2(powerPlashKa.sizeDelta.x, 250f);
            buttonInfoRect.sizeDelta = new Vector2(buttonInfoRect.sizeDelta.x, 267f);
        }
        else
        {
            while (powerPlashKa.sizeDelta.y > 165f)
            {
                powerPlashKa.sizeDelta = Vector2.MoveTowards(powerPlashKa.sizeDelta, new Vector2(powerPlashKa.sizeDelta.x, 165f), powerplashkaspeed * Time.deltaTime);
                yield return null;
            }
            powerPlashKa.sizeDelta = new Vector2(powerPlashKa.sizeDelta.x, 165f);
            buttonInfoRect.sizeDelta = new Vector2(buttonInfoRect.sizeDelta.x, 338f);
        }
        yield return null;
    }

    public IEnumerator InfoShining()
    {
        Color color = imageInfoShining.color;
        float shiningspeed = 2f;
        while (fakeVideoUpgrade.activeInHierarchy)
        {
            while (color.a > 0f)
            {
                color.a -= Time.deltaTime * shiningspeed;
                imageInfoShining.color = color;
                yield return null;
            }
            while (color.a < 1f)
            {
                color.a += Time.deltaTime * shiningspeed;
                imageInfoShining.color = color;
                yield return null;
            }
        }
        color.a = 0f;
        imageInfoShining.color = color;
    }

    public bool IsVideoAvailable()
    {
        return canMakeVideoUpgrade;
    }

    public bool IsEnoughMoney()
    {
        return (double)levelsInfo[heroData.currentLevel].cost < DataLoader.playerData.money;
    }
    public void OnSelectHero()
    {
        int heroIndex = (int)type;
        var selectedSur = GameManager.instance.survivors;
        bool check = false;
        foreach (var item in selectedSur)
        {
            if(item.heroType == type)
            {
                check = true; break;
            }
        }
        if (check)
        {
            if (GameManager.instance.survivors.Count < 3)
            {
                txtNotif.text = "Min size selected : 2 survivor";
                txtNotif.DOFade(1f, 1f).SetLoops(2, LoopType.Yoyo);
                return;
            }
            if (GameManager.instance != null)
            {
                var spawned = GameManager.instance.survivors.FirstOrDefault(s => s.heroType == type);
                if (spawned != null)
                {
                    GameManager.instance.survivors.Remove(spawned);
                    UnityEngine.Object.Destroy(spawned.gameObject);
                }
            }
        }
        else
        {
            if (GameManager.instance.survivors.Count < 4)
            {
                if (GameManager.instance != null)
                {
                    bool alreadySpawned = GameManager.instance.survivors.Any(s => (int)s.heroType == heroIndex);
                    if (!alreadySpawned)
                    {
                        Vector3 spawnPos = GameManager.instance.Position;

                        float num = UnityEngine.Random.Range(-0.5f, 0.5f);
                        float num2 = UnityEngine.Random.Range(-0.5f, 0.5f);

                        var prefab = DataLoader.Instance.GetSurvivorPrefab((SaveData.HeroData.HeroType)heroIndex);
                        if (prefab != null)
                        {
                            var survivor = UnityEngine.Object.Instantiate(
                                prefab,
                                new Vector3(spawnPos.x + num, spawnPos.y, spawnPos.z + num2),
                                Quaternion.identity,
                                TransformParentManager.Instance.heroes
                            );
                        }
                    }
                }
            }
            else
            {
                txtNotif.text = "Max size selected : 4 survivor";
                txtNotif.DOFade(1f, 1f).SetLoops(2, LoopType.Yoyo);
            }
        }
        DOVirtual.DelayedCall(0.1f, delegate
        {
            List<int> selected = new List<int>();
            foreach (var item in GameManager.instance.survivors)
            {
                selected.Add((int)item.heroType);
            }
            if (selected.Count < 4)
            {
                selected.Add(-1);
            }
            SaveData.HeroData.HeroType[] heroTypes = selected
                .Select(i => (SaveData.HeroData.HeroType)i)
                .ToArray();
            DataLoader.Instance.SaveSelectedSurvivor(heroTypes);
            selectedText.SetActive(selected.Contains((int)type));
        });
    }
    public void SetUpSelected(List<int> data)
    {
        selectedText.SetActive(data.Contains((int)type));
    }
}
