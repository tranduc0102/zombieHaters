using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardAchievementsPopup : MonoBehaviour
{
	[SerializeField]
	private Button btn_leaderboard;

	[SerializeField]
	private Button btn_achievements;

	private void Start()
	{
		UIController.instance.scrollControllers.leaderboardController.scrollRect.verticalNormalizedPosition = 1f;
		UIController.instance.scrollControllers.achievementsController.scrollRect.verticalNormalizedPosition = 1f;
	}

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
		if (UIController.instance.scrollControllers.achievementsController.newAchievementsCount > 0)
		{
			OpenAchievements();
		}
		else
		{
			OpenLeaderboard();
		}
	}

	public void OpenAchievements()
	{
		UIController.instance.scrollControllers.leaderboardController.Disable();
		UIController.instance.scrollControllers.achievementsController.gameObject.SetActive(true);
		UIController.instance.scrollControllers.achievementsController.Sort();
		btn_achievements.image.sprite = UIController.instance.multiplyImages.leaderboardAchievementsButtons.active;
		btn_leaderboard.image.sprite = UIController.instance.multiplyImages.leaderboardAchievementsButtons.inactive;
	}

	public void OpenLeaderboard()
	{
		UIController.instance.scrollControllers.leaderboardController.gameObject.SetActive(true);
		UIController.instance.scrollControllers.achievementsController.gameObject.SetActive(false);
		btn_leaderboard.image.sprite = UIController.instance.multiplyImages.leaderboardAchievementsButtons.active;
		btn_achievements.image.sprite = UIController.instance.multiplyImages.leaderboardAchievementsButtons.inactive;
	}
}
