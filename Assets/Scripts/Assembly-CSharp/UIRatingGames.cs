using UnityEngine;

public class UIRatingGames : MonoBehaviour
{
	[SerializeField]
	private UIArenaMode carMode;

	[SerializeField]
	private UIArenaMode pvpMode;

	[SerializeField]
	private GameObject tutorial;

	private bool tutorialAlreadyShown;

	public void OnEnable()
	{
		carMode.CheckLevel();
		pvpMode.CheckLevel();
	}

	public void StartTutorial()
	{
		if (!tutorialAlreadyShown)
		{
			tutorialAlreadyShown = true;
			DataLoader.gui.popUpsPanel.OpenPopup();
			tutorialAlreadyShown = true;
			tutorial.SetActive(true);
		}
	}

	public void TutorialCompleted()
	{
		PlayerPrefs.SetInt(StaticConstants.LeagueTutorialCompleted, 1);
		PlayerPrefs.Save();
	}

	public bool IsTutorial()
	{
		return tutorial.activeInHierarchy;
	}
}
