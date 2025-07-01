using System;
using System.Collections;
using UnityEngine;

public class DataUpdateManager : MonoBehaviour
{
	private bool isPaused;

	private bool firstUpdateCompleted;

	private Coroutine dailyMultiplier;

	private bool offlineTimeLoaded;

	public void StartUpdate()
	{
		isPaused = false;
		offlineTimeLoaded = false;
		StartCoroutine(ConnectionCheck(3f));
		if (StaticConstants.IsConnectedToInternet())
		{
			UpdateMoneyMultiplier();
			UpdateOfflineMoney();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			DataLoader.Instance.SavePlayerData();
			SaveOfflineTime();
		}
		else if (isPaused)
		{
			if (!StaticConstants.IsConnectedToInternet())
			{
				DataLoader.gui.NoInternetPanel(false);
			}
			else
			{
				UpdateAfterConnect();
			}
		}
		isPaused = pause;
	}

	private void OnApplicationQuit()
	{
		DataLoader.Instance.SavePlayerData();
		SaveOfflineTime();
	}

	public void UpdateMoneyMultiplier()
	{
		StartCoroutine(StartUpdateMoneyMultiplier());
	}

	public IEnumerator StartUpdateMoneyMultiplier()
	{
		while (!DataLoader.initialized)
		{
			yield return null;
		}
		if (dailyMultiplier != null)
		{
			StopCoroutine(dailyMultiplier);
		}
		if (PlayerPrefs.HasKey(StaticConstants.DailyMultiplierTime))
		{
			DataLoader.Instance.moneyMultiplier = PlayerPrefs.GetInt(StaticConstants.MultiplierKey);
			if (PlayerPrefs.HasKey(StaticConstants.infinityMultiplierPurchased) && DataLoader.Instance.moneyMultiplier <= 2)
			{
				SetInfinityMultiplierTime();
				yield break;
			}
			DataLoader.gui.videoMultiplier.infinityTimerGreen.SetActive(false);
			DataLoader.gui.videoMultiplier.infinityTimerImage.SetActive(false);
			DataLoader.gui.videoMultiplier.menuTimeLeftText.gameObject.SetActive(true);
			DataLoader.gui.videoMultiplier.popupTimeLeftText.gameObject.SetActive(true);
			if (TimeManager.gotDateTime)
			{
				dailyMultiplier = StartCoroutine(MoneyMultiplier(DataLoader.Instance.GetMultiplierTime()));
				yield break;
			}
			DataLoader.Instance.moneyMultiplier = 1;
			//DataLoader.gui.videoMultiplier.redCircle.SetActive(true);
/*			DataLoader.gui.videoAnim.SetBool("IsOpened", true);
*/		}
		else
		{
			firstUpdateCompleted = true;
/*			DataLoader.gui.videoAnim.SetBool("IsOpened", true);
*/			DataLoader.Instance.moneyMultiplier = 1;
			DataLoader.gui.videoMultiplier.popupTimeLeftText.text = "00:00:00";
			DataLoader.gui.videoMultiplier.redCircle.SetActive(true);
			if (PlayerPrefs.HasKey(StaticConstants.infinityMultiplierPurchased))
			{
				SetInfinityMultiplierTime();
				DataLoader.gui.videoMultiplier.redCircle.SetActive(false);
			}
		}
	}

	public void SetInfinityMultiplierTime()
	{
		DataLoader.Instance.moneyMultiplier = 2;
		DataLoader.gui.videoMultiplier.SetInfinityPurchased();
	}

	private IEnumerator MoneyMultiplier(TimeSpan time)
	{
		//DataLoader.gui.videoAnim.SetBool("IsOpened", false);
		//DataLoader.gui.multiplierAnim.SetBool("IsOpened", true);
		TimeSpan prevSpan = GetTimeSpan(DataLoader.gui.videoMultiplier.popupTimeLeftText.text);
		int addedMinutes = StaticConstants.MultiplierDurationInSeconds / 60;
		float speed = 0.05f;
		DataLoader.gui.videoMultiplier.redCircle.SetActive(false);
		if (prevSpan.TotalMinutes < time.TotalMinutes && firstUpdateCompleted)
		{
			while (prevSpan.TotalMinutes < time.TotalMinutes)
			{
				prevSpan = prevSpan.Add(TimeSpan.FromMinutes((float)addedMinutes * speed));
				DataLoader.gui.videoMultiplier.menuTimeLeftText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", prevSpan.Hours + prevSpan.Days * 24, prevSpan.Minutes, prevSpan.Seconds);
				DataLoader.gui.videoMultiplier.popupTimeLeftText.text = DataLoader.gui.videoMultiplier.menuTimeLeftText.text;
				yield return null;
			}
		}
		firstUpdateCompleted = true;
		if (DataLoader.Instance.moneyMultiplier != 1)
		{
			DataLoader.gui.multiplierPanelImage.sprite = UIController.instance.multiplyImages.activeMultiplier[MultiplyImages.GetMultiplierSpriteID(DataLoader.Instance.moneyMultiplier)];
			while (time.TotalSeconds > 0.0)
			{
				DataLoader.gui.videoMultiplier.menuTimeLeftText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours + time.Days * 24, time.Minutes, time.Seconds);
				DataLoader.gui.videoMultiplier.popupTimeLeftText.text = DataLoader.gui.videoMultiplier.menuTimeLeftText.text;
				time = time.Add(TimeSpan.FromSeconds(-1.0));
				yield return new WaitForSeconds(1f);
			}
		}
		else
		{
			Debug.LogWarning("MULTIPLIER 1");
		}
		DataLoader.gui.videoMultiplier.redCircle.SetActive(true);
		//DataLoader.gui.videoAnim.SetBool("IsOpened", true);
		yield return new WaitForSeconds(0.5f);
		//DataLoader.gui.multiplierAnim.SetBool("IsOpened", false);
		DataLoader.gui.videoMultiplier.popupTimeLeftText.text = "00:00:00";
		DataLoader.Instance.moneyMultiplier = 1;
		PlayerPrefs.SetInt(StaticConstants.MultiplierKey, 1);
		DataLoader.gui.videoMultiplier.UpdatePercentage();
	}

	public TimeSpan GetTimeSpan(string text)
	{
		string[] array = text.Split(':');
		return new TimeSpan(int.Parse(array[0]) / 24, int.Parse(array[0]) - int.Parse(array[0]) / 24 * 24, int.Parse(array[1]), int.Parse(array[2]));
	}

	public void SaveOfflineTime()
	{
		if (TimeManager.gotDateTime)
		{
			PlayerPrefs.SetString(StaticConstants.LastOnlineTime, TimeManager.CurrentDateTime.Ticks.ToString());
		}
	}

	public double LoadOfflineTime()
	{
		double num = 0.0;
		if (TimeManager.gotDateTime && PlayerPrefs.HasKey(StaticConstants.LastOnlineTime) && PlayerPrefs.HasKey(StaticConstants.TutorialCompleted))
		{
			DateTime value = new DateTime(Convert.ToInt64(PlayerPrefs.GetString(StaticConstants.LastOnlineTime)), DateTimeKind.Utc);
			num = TimeManager.CurrentDateTime.Subtract(value).TotalSeconds;
		}
		else
		{
			num = 0.0;
		}
		offlineTimeLoaded = true;
		return num;
	}

	public void UpdateOfflineMoney()
	{
		offlineTimeLoaded = false;
		StartCoroutine(WaitForSquadSpawn());
	}

	private IEnumerator WaitForSquadSpawn()
	{
		while (!DataLoader.initialized)
		{
			yield return new WaitForSeconds(1f);
		}
		if (TimeManager.CurrentDateTime != DateTime.MinValue)
		{
			DataLoader.gui.GetOfflineMoney();
			PlayerPrefs.SetString(StaticConstants.LastOnlineTime, TimeManager.CurrentDateTime.Ticks.ToString());
		}
	}

	private IEnumerator ConnectionCheck(float everySeconds)
	{
		if (!StaticConstants.NeedInternetConnection)
		{
			yield break;
		}
		yield return new WaitForSeconds(6f);
		while (true)
		{
			if (StaticConstants.IsConnectedToInternet())
			{
				if (offlineTimeLoaded && TimeManager.CurrentDateTime != DateTime.MinValue)
				{
					PlayerPrefs.SetString(StaticConstants.LastOnlineTime, TimeManager.CurrentDateTime.Ticks.ToString());
				}
			}
			else
			{
				DataLoader.gui.NoInternetPanel(false);
				while (!StaticConstants.IsConnectedToInternet())
				{
					yield return new WaitForSecondsRealtime(1f);
				}
				DataLoader.gui.NoInternetPanel(true);
			}
			yield return new WaitForSecondsRealtime(everySeconds);
		}
	}

	public void UpdateAfterConnect()
	{
		UpdateOfflineMoney();
		UpdateMoneyMultiplier();
	}
}
