using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBossCell : MonoBehaviour
{
	public Text textName;

	public Text textKillCount;

	public Text textReward;

	public Image progressImage;

	public Button buttonClaim;

	public GameObject objDone;

	public GameObject objProgress;

	public ParticleSystem claimFx;

	public RawImage rawImage;

	private WavesManager.Boss boss;

	private KilledBosses savedBoss;

	private Camera bossCamera;

	private GameObject bossObj;

	private int index = -1;

	private UIWantedList uIWantedList;

	public void SetContent(WavesManager.Boss boss, int _index)
	{
		this.boss = boss;
		index = _index;
		CreateBossCam(_index + TransformParentManager.Instance.upgradePanelCams.childCount + 10);
		UpdateCell();
	}

	public void UpdateCell(UIWantedList uIWantedList = null)
	{
		this.uIWantedList = uIWantedList;
		textName.font = LanguageManager.instance.currentLanguage.font;
		savedBoss = DataLoader.playerData.killedBosses.Find((KilledBosses kb) => kb.name == boss.prefabBoss.myNameIs);
		textReward.text = string.Format("{0:N0}", GetRewardCount());
		if (savedBoss != null)
		{
			bool flag = savedBoss.rewardedStage >= StaticConstants.bossStages.Length;
			if (!flag)
			{
				textKillCount.text = StaticConstants.bossStages[savedBoss.rewardedStage] - (StaticConstants.bossStages[savedBoss.rewardedStage] - savedBoss.count) + "/" + StaticConstants.bossStages[savedBoss.rewardedStage];
				progressImage.fillAmount = (float)(StaticConstants.bossStages[savedBoss.rewardedStage] - (StaticConstants.bossStages[savedBoss.rewardedStage] - savedBoss.count)) / (float)StaticConstants.bossStages[savedBoss.rewardedStage];
				SetShader(false);
			}
			textName.text = LanguageManager.instance.GetLocalizedText(boss.prefabBoss.myNameIs);
			objDone.SetActive(flag);
			textKillCount.gameObject.SetActive(!flag);
			textReward.gameObject.SetActive(!flag);
			objProgress.SetActive(!flag);
			buttonClaim.gameObject.SetActive(IsReady());
			return;
		}
		progressImage.fillAmount = 0f;
		textKillCount.text = "0/" + StaticConstants.bossStages[0];
		buttonClaim.gameObject.SetActive(false);
		objDone.SetActive(false);
		objProgress.SetActive(true);
		textReward.gameObject.SetActive(true);
		SetShader(!IsPrevBossKilled(index - 1));
		if (index == 0 || IsPrevBossKilled(index - 1))
		{
			textName.text = LanguageManager.instance.GetLocalizedText(boss.prefabBoss.myNameIs);
			SetShader(false);
		}
		else if (GameManager.instance.IsWorldOpen(2) && index == WavesManager.instance.bossesByWorld[0].bosses.Length)
		{
			textName.text = LanguageManager.instance.GetLocalizedText(boss.prefabBoss.myNameIs);
			SetShader(false);
		}
		else
		{
			textName.text = "????";
			SetShader(true);
		}
	}

	private bool IsPrevBossKilled(int _index)
	{
		for (int i = 0; i < WavesManager.instance.bossesByWorld.Length; i++)
		{
			for (int j = 0; j < WavesManager.instance.bossesByWorld[i].bosses.Length; j++)
			{
				if (_index == 0)
				{
					for (int k = 0; k < DataLoader.playerData.killedBosses.Count; k++)
					{
						if (DataLoader.playerData.killedBosses[k].name == WavesManager.instance.bossesByWorld[i].bosses[j].prefabBoss.myNameIs)
						{
							return true;
						}
					}
					return false;
				}
				_index--;
			}
		}
		return false;
	}

	public void CreateBossCam(int index)
	{
		float num = 2f * UIController.instance.scrollControllers.wantedListController.bossCameraPrefab.orthographicSize * UIController.instance.scrollControllers.survivorsController.heroCamPrefab.aspect + 1f;
		Camera camera = Object.Instantiate(UIController.instance.scrollControllers.wantedListController.bossCameraPrefab, new Vector3(10000f + (float)index * (num * 2f), 0f, 0f), Quaternion.identity, TransformParentManager.Instance.bossList);
		GameObject gameObject = Object.Instantiate(boss.prefabBoss.gameObject, camera.transform.GetChild(0));
		Component[] components = gameObject.gameObject.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (!(component is Transform))
			{
				Object.Destroy(component);
			}
		}
		ParticleSystem[] componentsInChildren = gameObject.GetComponentsInChildren<ParticleSystem>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (componentsInChildren[j] != null)
			{
				Object.Destroy(componentsInChildren[j].gameObject);
			}
		}
		RenderTexture renderTexture = new RenderTexture(UIController.instance.scrollControllers.wantedListController.prefabRenderTexture);
		bossCamera = camera;
		bossObj = gameObject;
		rawImage.texture = renderTexture;
		gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
		camera.targetTexture = renderTexture;
		camera.Render();
		camera.enabled = false;
	}

	public void SetShader(bool dark)
	{
		SkinnedMeshRenderer componentInChildren = bossObj.GetComponentInChildren<SkinnedMeshRenderer>();
		MeshRenderer[] componentsInChildren = bossObj.GetComponentsInChildren<MeshRenderer>();
		Shader shader;
		Color color;
		if (dark)
		{
			shader = Shader.Find("Legacy Shaders/VertexLit");
			color = new Color(35f / 51f, 35f / 51f, 35f / 51f);
		}
		else
		{
			shader = Shader.Find("Outlined/Silhouetted Unlit");
			color = new Color(1f, 1f, 1f);
		}
		componentInChildren.material = new Material(componentInChildren.material);
		componentInChildren.material.shader = shader;
		componentInChildren.material.color = color;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material = new Material(componentsInChildren[i].material);
			componentsInChildren[i].material.shader = shader;
			componentsInChildren[i].material.color = color;
		}
		bossCamera.Render();
	}

	public void GetReward()
	{
		claimFx.Play();
		DataLoader.Instance.RefreshGems(GetRewardCount());
		savedBoss.rewardedStage++;
		savedBoss.count = 0;
		Debug.Log("GetReward - " + boss.prefabBoss.myNameIs + "| uiWantedList is " + uIWantedList);
		UpdateCell(uIWantedList);
		if (uIWantedList != null)
		{
			uIWantedList.UpdateKillAllCells();
		}
		UIController.instance.scrollControllers.wantedListController.UpdateCountText();
		DataLoader.Instance.SavePlayerData();
		SoundManager.Instance.PlayClickSound();
		AnalyticsManager.instance.LogEvent("ClaimBossReward", new Dictionary<string, string>
		{
			{
				"BossName",
				boss.prefabBoss.myNameIs
			},
			{
				"Gems",
				GetRewardCount().ToString()
			}
		});
	}

	public bool IsReady()
	{
		if (savedBoss == null)
		{
			return false;
		}
		if (savedBoss.rewardedStage >= StaticConstants.bossStages.Length)
		{
			return false;
		}
		return savedBoss.count >= StaticConstants.bossStages[savedBoss.rewardedStage];
	}

	private int GetRewardCount()
	{
		if (savedBoss == null)
		{
			return boss.gemsReward;
		}
		switch (savedBoss.rewardedStage)
		{
		case 0:
			return boss.gemsReward;
		case 1:
			return 1;
		case 2:
			return 1;
		case 3:
			return 1;
		default:
			return 0;
		}
	}
}
