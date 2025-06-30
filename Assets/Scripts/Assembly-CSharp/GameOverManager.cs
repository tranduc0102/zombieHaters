using UnityEngine;

public class GameOverManager : MonoBehaviour
{
	public RectTransform popUpRect;

	public UIPresentController presentController;

	[SerializeField]
	private UIGamePlayGO gamePlayGameOver;

	[SerializeField]
	private UIArenaGO arenaGameOver;

	[SerializeField]
	private UIPvpGO pvpGameOver;

	[HideInInspector]
	public bool gameOverOpened;

	public void SetGameOverMenu(double newcoins, int newHaters, int exp, long score, int time)
	{
		gameOverOpened = true;
		switch (GameManager.instance.currentGameMode)
		{
		case GameManager.GameModes.GamePlay:
			arenaGameOver.Hide();
			gamePlayGameOver.Show();
			pvpGameOver.Hide();
			gamePlayGameOver.SetContent(newcoins, newHaters, exp, score, time);
			break;
		case GameManager.GameModes.Arena:
			gamePlayGameOver.Hide();
			arenaGameOver.Show();
			pvpGameOver.Hide();
			arenaGameOver.SetContent(newcoins, newHaters, exp, score, time);
			break;
		case GameManager.GameModes.PVP:
			gamePlayGameOver.Hide();
			arenaGameOver.Hide();
			pvpGameOver.Show();
			pvpGameOver.SetContent(newcoins, newHaters, exp, score, time);
			break;
		default:
			Debug.LogError("Game over wrong gamemode: " + GameManager.instance.currentGameMode);
			break;
		}
	}
}
