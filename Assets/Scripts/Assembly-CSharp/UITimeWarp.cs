using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimeWarp : MonoBehaviour
{
	[SerializeField]
	private Text lbl_coinsAmount;

	[SerializeField]
	private RectTransform arrowH;

	[SerializeField]
	private RectTransform arrowM;

	[SerializeField]
	private float speed;

	[SerializeField]
	private Image plume;

	[SerializeField]
	private Text popupName;

	private double m_money;

	private bool m_hideAllPopups;

	private void OnEnable()
	{
		StartCoroutine(UIController.instance.Scale(base.transform));
	}

	public void Open(int seconds, bool hideAllPopups = false)
	{
		popupName.text = LanguageManager.instance.GetLocalizedText((seconds != 3600) ? LanguageKeysEnum.six_hours_time_warp : LanguageKeysEnum.one_hour_time_warp);
		popupName.font = LanguageManager.instance.currentLanguage.font;
		base.gameObject.SetActive(true);
		m_money = DataLoader.Instance.GetTimeWarpGoldCount(seconds);
		m_hideAllPopups = hideAllPopups;
		lbl_coinsAmount.text = AbbreviationUtility.AbbreviateNumber(m_money);
		StartCoroutine(AnimateText());
		StartCoroutine(AnimateArrows());
		AnalyticsManager.instance.LogEvent("TimeWarpOpened", new Dictionary<string, string> { 
		{
			"Hours",
			(seconds / 60 / 60).ToString()
		} });
	}

	public void Claim()
	{
		if (m_hideAllPopups)
		{
			DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnDisable()
	{
		DataLoader.Instance.RefreshMoney(m_money, true);
	}

	protected IEnumerator AnimateText()
	{
		lbl_coinsAmount.text = "0";
		float speed = 30f;
		double delta = m_money;
		for (int i = 1; (float)i <= speed; i++)
		{
			yield return new WaitForSecondsRealtime(0.05f);
			lbl_coinsAmount.text = AbbreviationUtility.AbbreviateNumber(delta / (double)speed * (double)i);
		}
		lbl_coinsAmount.text = AbbreviationUtility.AbbreviateNumber(delta);
	}

	private IEnumerator AnimateArrows()
	{
		plume.rectTransform.localEulerAngles = Vector3.zero;
		arrowM.localRotation = Quaternion.identity;
		arrowH.localRotation = Quaternion.identity;
		arrowM.Rotate(new Vector3(0f, 0f, -1f) * Time.deltaTime * speed);
		arrowH.Rotate(new Vector3(0f, 0f, -1f) * Time.deltaTime * speed / 4f);
		yield return null;
		float prevAngle = arrowH.localEulerAngles.z;
		while (arrowH.localEulerAngles.z <= prevAngle)
		{
			yield return null;
			prevAngle = arrowH.localEulerAngles.z;
			arrowM.Rotate(new Vector3(0f, 0f, -1f) * Time.deltaTime * speed);
			arrowH.Rotate(new Vector3(0f, 0f, -1f) * Time.deltaTime * speed / 4f);
		}
		arrowM.localRotation = Quaternion.identity;
		arrowH.localRotation = Quaternion.identity;
		plume.rectTransform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * speed);
		yield return null;
		while (plume.rectTransform.localEulerAngles.z < 150f)
		{
			plume.rectTransform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * speed);
			yield return null;
		}
	}
}
