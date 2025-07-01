using System;
using System.Collections;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfo : MonoBehaviour
{
	[SerializeField]
	private Text DescriptionText;

	[SerializeField]
	private Text shortDescriptionText;

	[SerializeField]
	private RawImage rawImage;

	[SerializeField]
	private Text currentPowerText;

	[SerializeField]
	private Text currentPowerTextInactive;

	[SerializeField]
	private GameObject activePowerPlashka;

	[SerializeField]
	private GameObject inactivePowerPlashka;

	public Text costText;

	public Text cost2Text;

	[SerializeField]
	private Text newPower;

	[SerializeField]
	private GameObject lockedStripe;

	[SerializeField]
	private Button buttonUpgrade;

	[SerializeField]
	private Button buttonUpgrade2;

	[SerializeField]
	private Button buttonVideoUpgrade;

	[SerializeField]
	private Button buttonBuy;

	[SerializeField]
	private Text levelText;

	[SerializeField]
	private Text textPriceReal;

	[SerializeField]
	private Text heroname;

	[SerializeField]
	private Text lockedStripeText;

	[SerializeField]
	private Image popupImage;

	[SerializeField]
	private Image imageVideoUpgradeButton;

	[SerializeField]
	private Image imageVideo;

	[SerializeField]
	private ParticleSystem upgradeButtonFx;

	[SerializeField]
	private ParticleSystem videoUpgradeButtonFx;

	[SerializeField]
	private RectTransform descriptionRect;

	public Image imageUpgradeButton;

	[SerializeField]
	private ParticleSystem upgradeFx;

	[SerializeField]
	private GameObject priceLoader;

	private Survivors survivor;

	[NonSerialized]
	public SurviviorContent surviviorContent;

	private int productIndex;

	private string opensInTranslate;

	private int survivorIndex;

	private Coroutine getPriceCor;

    public void SetContent(int survivorIndex, Texture texture, bool isLocked, SurviviorContent content)
	{
		rawImage.texture = texture;
		surviviorContent = content;
		survivor = DataLoader.Instance.survivors[survivorIndex];
		DescriptionText.text = LanguageManager.instance.GetLocalizedText(survivor.heroStory);
		DescriptionText.font = LanguageManager.instance.currentLanguage.font;
		shortDescriptionText.text = LanguageManager.instance.GetLocalizedText(survivor.shortDescriptionKey);
		shortDescriptionText.font = LanguageManager.instance.currentLanguage.font;
		opensInTranslate = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Opens_in);
		heroname.text = LanguageManager.instance.GetLocalizedText(survivor.fullname);
		SetIsLocked(isLocked);
		this.survivorIndex = survivorIndex;
	}

    public void SetIsLocked(bool isLocked)
	{
		if (isLocked)
		{
			foreach (HeroesPurchaseInfo heroesPurchase in InAppManager.Instance.heroesPurchases)
			{
				if (heroesPurchase.heroType == surviviorContent.heroData.heroType)
				{
					productIndex = heroesPurchase.index;
					UIController.instance.StartGetPrice(textPriceReal, priceLoader, heroesPurchase, ref getPriceCor);
				}
			}
			lockedStripeText.font = LanguageManager.instance.currentLanguage.font;
		}
		descriptionRect.sizeDelta = ((!isLocked) ? new Vector2(descriptionRect.sizeDelta.x, 340f) : new Vector2(descriptionRect.sizeDelta.x, 262f));
		DescriptionText.color = ((!isLocked) ? Color.white : new Color(0f, 0.32156864f, 0.4745098f, 1f));
		buttonBuy.gameObject.SetActive(isLocked);
		buttonUpgrade2.gameObject.SetActive(!isLocked);
		buttonVideoUpgrade.gameObject.SetActive(!isLocked);
		buttonUpgrade.interactable = !isLocked;
		lockedStripe.SetActive(isLocked);
		activePowerPlashka.SetActive(!isLocked);
		inactivePowerPlashka.SetActive(isLocked);
		popupImage.sprite = ((!isLocked) ? UIController.instance.multiplyImages.popup.active : UIController.instance.multiplyImages.popup.inactive);
	}

	private IEnumerator DayHeroTimer(TimeSpan timeSpan)
	{
		while (timeSpan.TotalSeconds > 0.0)
		{
			if (timeSpan.Days > 0)
			{
				lockedStripeText.text = string.Format(opensInTranslate + "\n{0:D1} {1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else
			{
				lockedStripeText.text = string.Format(opensInTranslate + "\n{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1.0));
			yield return new WaitForSeconds(1f);
		}
	}

    public void Upgrade(bool rewarded)
	{
		string text = levelText.text;
		surviviorContent.Upgrade(rewarded);
	}

    public void PlayFx(bool rewarded)
	{
		upgradeFx.Play();
		if (rewarded)
		{
			videoUpgradeButtonFx.Play();
		}
		else
		{
			upgradeButtonFx.Play();
		}
		SetVideoButton(!rewarded);
		UpdateInfo();
	}

    public void Buy()
	{
		InAppManager.Instance.BuyProductID(productIndex);
	}

    public void SetVideoButton(bool active)
	{
		buttonVideoUpgrade.interactable = active;
		if (active)
		{
			imageVideo.sprite = UIController.instance.multiplyImages.upgrageButtons[2].active;
			imageVideoUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[1].active;
		}
		else
		{
			imageVideo.sprite = UIController.instance.multiplyImages.upgrageButtons[2].inactive;
			imageVideoUpgradeButton.sprite = UIController.instance.multiplyImages.upgrageButtons[1].inactive;
		}
	}

    public void OnEnable()
	{
		UpdateInfo();
		if (surviviorContent.fakeVideoUpgrade.activeInHierarchy)
		{
			buttonUpgrade2.image.rectTransform.anchoredPosition = new Vector2(-200f, 0f);
			buttonVideoUpgrade.image.rectTransform.anchoredPosition = new Vector2(200f, 0f);
		}
		else
		{
			buttonUpgrade2.image.rectTransform.anchoredPosition = Vector2.zero;
			buttonVideoUpgrade.image.rectTransform.anchoredPosition = Vector2.zero;
		}
		if (DataLoader.Instance.survivors[survivorIndex].heroOpenType == HeroOpenType.Level)
		{
			lockedStripeText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Reach_x_Level_to_Unlock).Replace("x", DataLoader.Instance.survivors[survivorIndex].requiredLevelToOpen.ToString());
		}
		else if (DataLoader.Instance.survivors[survivorIndex].heroOpenType == HeroOpenType.Day)
		{
			TimeSpan timeSpan = DataLoader.playerData.firstEnterDate.Add(TimeSpan.FromDays(DataLoader.Instance.survivors[survivorIndex].daysToOpen)).Subtract(TimeManager.CurrentDateTime);
			StartCoroutine(DayHeroTimer(timeSpan));
		}
		StartCoroutine(UIController.instance.Scale(base.transform));
	}

    public void UpdateInfo()
	{
		int currentHeroLevel = GetCurrentHeroLevel();
		float num = (1f + PassiveAbilitiesManager.bonusHelper.GetCriticalHitChance()) * (1f + PassiveAbilitiesManager.bonusHelper.AttackSpeedBonus) * (1f + PassiveAbilitiesManager.bonusHelper.DamageBonus);
		currentPowerText.text = AbbreviationUtility.AbbreviateNumber(surviviorContent.GetLevelPower(currentHeroLevel) * num, 2);
		currentPowerTextInactive.text = currentPowerText.text;
		if (currentHeroLevel < DataLoader.playerData.survivorMaxLevel)
		{
			newPower.text = "+" + AbbreviationUtility.AbbreviateNumber((surviviorContent.GetLevelPower(currentHeroLevel + 1) - surviviorContent.GetLevelPower(currentHeroLevel)) * num, 2);
			AbbreviationUtility.AbbreviateNumber(survivor.levels[currentHeroLevel].cost);
			costText.text = AbbreviationUtility.AbbreviateNumber(survivor.levels[currentHeroLevel].cost);
			cost2Text.text = costText.text;
			buttonUpgrade.interactable = true;
			buttonUpgrade2.interactable = true;
			costText.transform.parent.gameObject.SetActive(true);
			cost2Text.gameObject.SetActive(true);
		}
		else
		{
			newPower.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Max_Maximum);
			newPower.font = LanguageManager.instance.currentLanguage.font;
			buttonUpgrade.interactable = false;
			buttonUpgrade2.interactable = false;
			costText.transform.parent.gameObject.SetActive(false);
			cost2Text.gameObject.SetActive(false);
		}
		SetVideoButton(currentHeroLevel < DataLoader.playerData.survivorMaxLevel && surviviorContent.IsVideoAvailable());
		levelText.text = currentHeroLevel.ToString();
		if (surviviorContent.heroData.isOpened)
		{
			buttonUpgrade2.gameObject.SetActive(currentHeroLevel != DataLoader.playerData.survivorMaxLevel);
			buttonVideoUpgrade.gameObject.SetActive(currentHeroLevel != DataLoader.playerData.survivorMaxLevel);
		}
		else
		{
			buttonUpgrade2.gameObject.SetActive(false);
			buttonVideoUpgrade.gameObject.SetActive(false);
		}
		UIController.instance.scrollControllers.perkController.UpdateAllContent();
	}

	public IEnumerator HideRewarded()
	{
		Vector2 vector = Vector2.zero;
		float speed = 650f;
		while (buttonVideoUpgrade.image.rectTransform.anchoredPosition != vector)
		{
			buttonVideoUpgrade.image.rectTransform.anchoredPosition = Vector2.MoveTowards(buttonVideoUpgrade.image.rectTransform.anchoredPosition, vector, Time.deltaTime * speed);
			buttonUpgrade2.image.rectTransform.anchoredPosition = Vector2.MoveTowards(buttonUpgrade2.image.rectTransform.anchoredPosition, vector, Time.deltaTime * speed);
			yield return null;
		}
		buttonUpgrade2.image.rectTransform.anchoredPosition = vector;
		buttonVideoUpgrade.image.rectTransform.anchoredPosition = vector;
	}

    public void OnDisable()
	{
		StopAllCoroutines();
	}

    public void SetUpgradeButtonInteractable(bool interactable)
	{
	}

	public int GetCurrentHeroLevel()
	{
		return surviviorContent.heroData.currentLevel;
	}
}
