using UnityEngine.UI;

public class UILeaderboardCell : UIScrollCell
{
	public Text lbl_playerName;

	public Text lbl_playerScore;

	public Text lbl_playerPlace;

	public Image img_topCup;

	public override void SetContent(int index)
	{
		base.SetContent(index);
		if (index < 3)
		{
			img_topCup.sprite = UIController.instance.multiplyImages.leaderboardCups[index];
		}
		else
		{
			img_topCup.enabled = false;
		}
		lbl_playerPlace.gameObject.SetActive(!img_topCup.enabled);
		lbl_playerPlace.text = (index + 1).ToString();
	}

	public void UpdateContent(string name, long score)
	{
		lbl_playerName.text = name;
		lbl_playerScore.text = string.Format("{0:N0}", score);
	}
}
