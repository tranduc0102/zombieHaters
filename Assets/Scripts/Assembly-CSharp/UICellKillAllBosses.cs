using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICellKillAllBosses : MonoBehaviour
{
	public Text textKill;

	public Text textReward;

	public Text textWorldName;

	public Text progressText;

	public Button btnClaim;

	public Image progresImage;

	public GameObject objDone;

	public ParticleSystem claimFx;

	private WavesManager.Bosses bosses;

	private int worldIndex;

	public void SetContent(WavesManager.Bosses bosses, int index)
	{
		this.bosses = bosses;
		worldIndex = index;
		textReward.text = string.Format("{0:N0}", bosses.allBossesReward);
		UpdateContent();
	}

	public void UpdateContent()
	{
		Debug.Log("UpdateContent " + textWorldName.text);
		textKill.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Kill_all_bosses);
		textKill.font = LanguageManager.instance.currentLanguage.font;
		textWorldName.text = LanguageManager.instance.GetLocalizedText(GameManager.instance.worldNames[worldIndex]);
		textWorldName.font = LanguageManager.instance.currentLanguage.font;
		int killedBossesCount = GetKilledBossesCount();
		progressText.text = killedBossesCount + "/" + bosses.bosses.Length;
		progresImage.fillAmount = (float)killedBossesCount / (float)bosses.bosses.Length;
		if (progresImage.fillAmount >= 1f)
		{
			btnClaim.gameObject.SetActive(!PlayerPrefs.HasKey(StaticConstants.allBossesRewardedkey + worldIndex));
			objDone.SetActive(!btnClaim.gameObject.activeInHierarchy);
			textReward.gameObject.SetActive(btnClaim.gameObject.activeInHierarchy);
			progresImage.transform.parent.gameObject.SetActive(btnClaim.gameObject.activeInHierarchy);
		}
		else
		{
			progresImage.transform.parent.gameObject.SetActive(true);
			objDone.SetActive(false);
			btnClaim.gameObject.SetActive(false);
			textReward.gameObject.SetActive(true);
		}
		btnClaim.interactable = !PlayerPrefs.HasKey(StaticConstants.allBossesRewardedkey + worldIndex);
	}

	public int GetKilledBossesCount()
	{
		int num = 0;
		for (int i = 0; i < bosses.bosses.Length; i++)
		{
			foreach (KilledBosses killedBoss in DataLoader.playerData.killedBosses)
			{
				if (killedBoss.name == bosses.bosses[i].prefabBoss.myNameIs && killedBoss.rewardedStage > 0)
				{
					num++;
					break;
				}
			}
		}
		return num;
	}

	public void GetReward()
	{
		PlayerPrefs.SetInt(StaticConstants.allBossesRewardedkey + worldIndex, 1);
		claimFx.Play();
		UpdateContent();
		UIController.instance.scrollControllers.wantedListController.UpdateCountText();
		SoundManager.Instance.PlayClickSound();
		DataLoader.Instance.RefreshGems((int)bosses.allBossesReward, true);
		Debug.Log("AllBossesReward: " + GameManager.instance.worldNames[worldIndex]);
		AnalyticsManager.instance.LogEvent("ClaimAllBossesReward", new Dictionary<string, string>
		{
			{
				"World",
				GameManager.instance.worldNames[worldIndex]
			},
			{
				"Gems",
				bosses.allBossesReward.ToString()
			}
		});
	}

	public bool IsReady()
	{
		return !PlayerPrefs.HasKey(StaticConstants.allBossesRewardedkey + worldIndex) && GetKilledBossesCount() >= bosses.bosses.Length;
	}
}
