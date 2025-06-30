using UnityEngine;
using UnityEngine.UI;

public class NotifyController : MonoBehaviour
{
	public Button[] bttns;

	private float currentTime;

	private int id = 1;

	private void Update()
	{
		if (currentTime > 0f)
		{
			currentTime -= Time.deltaTime;
			Button[] array = bttns;
			foreach (Button button in array)
			{
				button.interactable = false;
			}
		}
		else if (currentTime <= 0f)
		{
			Button[] array2 = bttns;
			foreach (Button button2 in array2)
			{
				button2.interactable = true;
			}
		}
	}

	public void turnOnNotif(int time)
	{
		AndroidNotification.SendNotification(1, time, "Отличный заголовок", "Тут может быть ваша реклама!", "Посмотри сюда!", Color.white, true, AndroidNotification.NotificationExecuteMode.Inexact, true, 2, true, 0L, 500L);
		currentTime = time;
	}

	public void turnOnNotifNew(int time)
	{
		AndroidNotification.SendNotification(id, time, "Отличный заголовок", "Тут может быть ваша реклама!", "Посмотри сюда!", Color.white, true, AndroidNotification.NotificationExecuteMode.Inexact, true, 2, true, 0L, 500L);
		id++;
		currentTime = time;
	}
}
