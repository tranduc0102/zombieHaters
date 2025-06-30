using System;

[Serializable]
public class ScrollContentControllers
{
	public UISurvivorUpgradePanel survivorsController;

	public UIAchievementsPanel achievementsController;

	public UIHeroInfoPerkController perkController;

	public UIDailyPanel dailyController;

	public UIWantedList wantedListController;

	public UIRatingPanel ratingContoller;

	public UIPassiveAbilitiesPanel passiveAbilitiesController;

	public UILeaderboardPanel leaderboardController;

	public UIBottomScrollPanel bottomScrollController;

	public void CreateAllCells()
	{
		perkController.CreateCells();
		achievementsController.CreateCells();
		survivorsController.CreateCells();
		dailyController.CreateCells();
		wantedListController.CreateCells();
		ratingContoller.CreateCells();
		passiveAbilitiesController.CreateCells();
		leaderboardController.CreateCells();
		bottomScrollController.CreateCells();
	}
}
