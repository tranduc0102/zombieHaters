using System;
using System.Collections;
using UnityEngine;

public class MoneyBoxManager : MonoBehaviour
{
	public static MoneyBoxManager instance;

	[HideInInspector]
	private MoneyBoxSpawn[] places;

	[SerializeField]
	private GameObject prefabMoneyBox;

	[NonSerialized]
	public bool moneyBoxPicked;

	private int randomPlace;

	[NonSerialized]
	public string currentHelpText = string.Empty;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public void StartGame()
	{
		places = UnityEngine.Object.FindObjectsOfType<MoneyBoxSpawn>();
		bool flag = false;
		MoneyBoxSpawn[] array = places;
		foreach (MoneyBoxSpawn moneyBoxSpawn in array)
		{
			if (moneyBoxSpawn.openAtLevel <= DataLoader.Instance.GetCurrentPlayerLevel() && moneyBoxSpawn.worldNumber == GameManager.instance.currentWorldNumber)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("Compatible places for spawn MoneyBox Not Found!");
			return;
		}
		do
		{
			randomPlace = UnityEngine.Random.Range(0, places.Length);
		}
		while (places[randomPlace].openAtLevel > DataLoader.Instance.GetCurrentPlayerLevel() || places[randomPlace].worldNumber != GameManager.instance.currentWorldNumber);
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabMoneyBox, places[randomPlace].transform.position, default(Quaternion), TransformParentManager.Instance.moneyBox);
		gameObject.GetComponent<MoneyBox>().isBigCoin = true;
	}

	public void EndGame()
	{
		MoneyBox[] array = UnityEngine.Object.FindObjectsOfType<MoneyBox>();
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i].gameObject);
		}
	}

	public IEnumerator WaitForGui()
	{
		while (!DataLoader.initialized)
		{
			yield return new WaitForSeconds(0.5f);
		}
		TrySpawnBox();
	}

	public void TrySpawnBox()
	{
	}

	private bool CanSpawnBox()
	{
		if (GameManager.instance.isTutorialNow)
		{
			return false;
		}
		if (!TimeManager.gotDateTime)
		{
			DataLoader.gui.secretAnimator.SetBool("IsOpened", false);
			return false;
		}
		if (PlayerPrefs.HasKey(StaticConstants.MoneyBoxKey))
		{
			DateTime date = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.MoneyBoxKey)), DateTimeKind.Utc).Date;
			if (DateTime.Compare(TimeManager.CurrentDateTime.Date, date) == 0)
			{
				Debug.Log("You already got MoneyBox today");
				return false;
			}
			return true;
		}
		if (StaticConstants.NeedInternetConnection)
		{
			return StaticConstants.IsConnectedToInternet();
		}
		return true;
	}

	public void UpdateSecret()
	{
		if (!CanSpawnBox())
		{
			DataLoader.gui.newSecret.SetActive(false);
			moneyBoxPicked = true;
			currentHelpText = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.You_already_got_moneybox_today);
		}
		else
		{
			currentHelpText = LanguageManager.instance.GetLocalizedText(places[randomPlace].helpText);
			DataLoader.gui.newSecret.SetActive(true);
		}
	}
}
