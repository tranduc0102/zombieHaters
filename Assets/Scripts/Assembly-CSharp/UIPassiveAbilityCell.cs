using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIPassiveAbilityCell : UIScrollCell
{
	[SerializeField]
	private Text lbl_abilityName;

	[SerializeField]
	private Text descriptionText;

	[SerializeField]
	private Text lbl_percentage;

	[SerializeField]
	private Text lbl_upgrade;

	[SerializeField]
	private Text buttonBuyText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Button buttonBuy;

	[SerializeField]
	private GameObject locked;

	[SerializeField]
	private Image imageIcon;

	[SerializeField]
	private ParticleSystem upgradePs;

	private AbilityData ability;

	public override void SetContent(int index)
	{
		base.SetContent(index);
		ability = DataLoader.Instance.passiveAbilitiesManager.GetAbility(PassiveAbilitiesManager.sequence[index]);
		descriptionText.text = ability.abilityType.ToString();
	}

	public void UpgradeAbility()
	{
		if (DataLoader.Instance.passiveAbilitiesManager.UpgradeAbility(ability.abilityType))
		{
			AnalyticsManager.instance.LogEvent("UpgradeAbility", new Dictionary<string, string>
			{
				{
					"Ability",
					ability.abilityType.ToString()
				},
				{
					"level",
					ability.savedAbility.abilityLevel.ToString()
				}
			});
			UIController.instance.scrollControllers.passiveAbilitiesController.UpdateAllContent();
			UIController.instance.scrollControllers.survivorsController.UpdateAllContent();
			upgradePs.Play();
		}
	}

	private void OnEnable()
	{
		UpdateLocalization();
	}

	public void UpdateLocalization()
	{
		descriptionText.font = LanguageManager.instance.currentLanguage.font;
		lbl_upgrade.font = LanguageManager.instance.currentLanguage.font;
		lbl_abilityName.font = LanguageManager.instance.currentLanguage.font;
		switch (ability.abilityType)
		{
		case PassiveAbilityTypes.CriticalHit:
			lbl_abilityName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Critical_shot);
			descriptionText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.num_chance_of_double_damage);
			break;
		case PassiveAbilityTypes.AttackSpeed:
			lbl_abilityName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Quick_shot);
			descriptionText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.num_attack_speed_to_all_characters);
			break;
		case PassiveAbilityTypes.Damage:
			lbl_abilityName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Amplified_shot);
			descriptionText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.num_damage_to_all_characters);
			break;
		case PassiveAbilityTypes.Gold:
			lbl_abilityName.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Bonus_to_gold);
			descriptionText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.num_gold_profit);
			break;
		}
		descriptionText.text = descriptionText.text.Replace("(NUM)", string.Empty);
		lbl_percentage.text = ability.GetBonusBylevel(ability.savedAbility.abilityLevel) * 100f + "%";
		lbl_upgrade.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Upgrade) + " " + ability.GetBonusBylevel(ability.savedAbility.abilityLevel + 1) * 100f + "%";
		lbl_upgrade.gameObject.SetActive(ability.savedAbility.abilityLevel < DataLoader.Instance.passiveAbilitiesManager.maxAbilityLevel);
		UpdatePrice();
	}

	public void GoToShop()
	{
		DataLoader.gui.popUpsPanel.shop.gameObject.SetActive(true);
		DataLoader.gui.popUpsPanel.shop.ScrollToCoins();
		UIController.instance.scrollControllers.passiveAbilitiesController.gameObject.SetActive(false);
		AnalyticsManager.instance.LogEvent("GoToShopAbilities", new Dictionary<string, string> { 
		{
			"Gems",
			DataLoader.playerData.gems.ToString()
		} });
	}

	public void UpdateContent()
	{
		UpdateLocalization();
		locked.SetActive(!ability.IsOpened());
		imageIcon.sprite = UIController.instance.multiplyImages.passiveAbilityIcons.First((PassiveAbilityIcon item) => item.abilityType == ability.abilityType).images[(ability.savedAbility.abilityLevel != 0) ? (ability.savedAbility.abilityLevel - 1) : 0];
	}

	public void UpdatePrice()
	{
		if (ability.IsMaxLevel())
		{
			priceText.gameObject.SetActive(false);
			buttonBuy.image.sprite = UIController.instance.multiplyImages.abilitiesUpgradeButtons[4];
			buttonBuyText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Max_Maximum);
			return;
		}
		int currentPrice = ability.GetCurrentPrice();
		priceText.text = currentPrice.ToString();
		buttonBuy.onClick.RemoveAllListeners();
		if (currentPrice > DataLoader.playerData.gems)
		{
			buttonBuy.onClick.AddListener(GoToShop);
			priceText.color = Color.red;
			if (ability.IsOpened())
			{
				buttonBuy.image.sprite = UIController.instance.multiplyImages.abilitiesUpgradeButtons[3];
				buttonBuyText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Buy_now);
			}
			else
			{
				buttonBuy.image.sprite = UIController.instance.multiplyImages.abilitiesUpgradeButtons[1];
				buttonBuyText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Upgrade);
			}
		}
		else
		{
			buttonBuy.onClick.AddListener(UpgradeAbility);
			priceText.color = Color.white;
			if (ability.IsOpened())
			{
				buttonBuy.image.sprite = UIController.instance.multiplyImages.abilitiesUpgradeButtons[0];
				buttonBuyText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Upgrade);
			}
			else
			{
				buttonBuy.image.sprite = UIController.instance.multiplyImages.abilitiesUpgradeButtons[2];
				buttonBuyText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Upgrade);
			}
		}
		buttonBuyText.font = LanguageManager.instance.currentLanguage.font;
	}
}
