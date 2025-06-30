using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
	[SerializeField]
	private SaveData.BoostersData.BoosterType abilityType;

	[SerializeField]
	private int cooldownTime = 10;

	[SerializeField]
	private Image filledImage;

	[SerializeField]
	private Text countText;

	private Button button;

	public AudioClip sound;

	[Header("Inc Speed Settings")]
	[SerializeField]
	private float speedMultiplier = 1.5f;

	[SerializeField]
	private float workTime = 5f;

	[SerializeField]
	private ParticleSystem useFx;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void Reset()
	{
		StopCoroutine(FillBar());
		filledImage.transform.parent.gameObject.SetActive(false);
		Refresh();
	}

	public void Refresh()
	{
		if (abilityType != SaveData.BoostersData.BoosterType.IncSpeed)
		{
			countText.text = DataLoader.Instance.GetBoostersCount(abilityType).ToString();
		}
		if (abilityType == SaveData.BoostersData.BoosterType.IncSpeed || DataLoader.Instance.GetBoostersCount(abilityType) > 0)
		{
			button.interactable = true;
			return;
		}
		StopCoroutine(FillBar());
		countText.text = "+10";
		filledImage.transform.parent.gameObject.SetActive(false);
	}

	public void DoIt()
	{
		if (!PlayerPrefs.HasKey(StaticConstants.AbilityTutorialCompleted) || !DataLoader.gui.pauseReady)
		{
			return;
		}
		if (abilityType != SaveData.BoostersData.BoosterType.IncSpeed && DataLoader.Instance.GetBoostersCount(abilityType) == 0)
		{
			DataLoader.gui.popUpsPanel.ingameShop.OpenShop(abilityType, Refresh);
			return;
		}
		switch (abilityType)
		{
		case SaveData.BoostersData.BoosterType.KillAll:
			GameManager.instance.KillAll();
			break;
		case SaveData.BoostersData.BoosterType.IncSpeed:
			SurvivorHuman.moveForceAbilityMultiplier = speedMultiplier;
			Invoke("StopIncSpeed", workTime);
			break;
		default:
			SpawnManager.instance.OneMoreParashute();
			break;
		}
		useFx.Play();
		if (abilityType != SaveData.BoostersData.BoosterType.IncSpeed)
		{
			DataLoader.Instance.UseBoosters(abilityType);
		}
		Refresh();
		SoundManager.Instance.PlaySound(sound);
		if (cooldownTime > -1 && (abilityType == SaveData.BoostersData.BoosterType.IncSpeed || DataLoader.Instance.GetBoostersCount(abilityType) > 0))
		{
			StartCoroutine(FillBar());
		}
	}

	private IEnumerator FillBar()
	{
		float cooldown = cooldownTime;
		filledImage.fillAmount = 0f;
		filledImage.transform.parent.gameObject.SetActive(true);
		button.interactable = false;
		while (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
			filledImage.fillAmount += Time.deltaTime / (float)cooldownTime;
			yield return null;
		}
		filledImage.fillAmount = 1f;
		filledImage.transform.parent.gameObject.SetActive(false);
		Refresh();
	}

	private void StopIncSpeed()
	{
		SurvivorHuman.moveForceAbilityMultiplier = 1f;
	}
}
