using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIPresent : MonoBehaviour
{
	[SerializeField]
	protected ParticleSystem rewardFx;

	[SerializeField]
	protected AudioClip rewardClip;

	[SerializeField]
	protected Button skipBtn;

	[SerializeField]
	protected Button videoBtn;

	[SerializeField]
	protected Text skipText;

	[SerializeField]
	protected float delayBeforeShowSkip = 3.5f;

	private Coroutine particleCor;

	public virtual void SetContent(int money)
	{
		DataLoader.gui.OnAnimationCompleted("Present", "Present", OnPresentLoaded);
		ResetButtons();
	}

	public abstract string GetSkipEventName();

	public virtual void OnEscape()
	{
		Skip();
	}

	public void Skip()
	{
		GoToGameOver();
		AnalyticsManager.instance.LogEvent(GetSkipEventName(), new Dictionary<string, string>());
	}

	public void GoToGameOver()
	{
		DataLoader.gui.ChangeAnimationState("GameOver");
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && base.gameObject.activeInHierarchy)
		{
			OnEscape();
		}
	}

	protected void StartSimulation()
	{
		StopSimulation();
		particleCor = StartCoroutine(SimulateParticle());
	}

	protected void StopSimulation()
	{
		if (particleCor != null)
		{
			StopCoroutine(particleCor);
		}
	}

	private IEnumerator SimulateParticle()
	{
		rewardFx.Play();
		while (true)
		{
			rewardFx.Simulate(Time.unscaledDeltaTime, true, false);
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
		}
	}

	protected virtual void OnPresentLoaded()
	{
		StartCoroutine(DelayedShowSkip());
	}

	protected virtual void ResetButtons()
	{
		videoBtn.gameObject.SetActive(true);
		videoBtn.image.rectTransform.anchoredPosition = new Vector2(0f, videoBtn.image.rectTransform.anchoredPosition.y);
		skipBtn.image.rectTransform.anchoredPosition = new Vector2(200f, skipBtn.image.rectTransform.anchoredPosition.y);
		skipBtn.image.color = new Color(skipBtn.image.color.r, skipBtn.image.color.g, skipBtn.image.color.b, 0f);
		skipText.color = new Color(1f, 1f, 1f, 0f);
		skipBtn.interactable = false;
	}

	protected IEnumerator DelayedShowSkip()
	{
		yield return new WaitForSecondsRealtime(delayBeforeShowSkip);
		Vector2 target = new Vector2(-200f, videoBtn.image.rectTransform.anchoredPosition.y);
		bool visibleStarted = false;
		while (videoBtn.image.rectTransform.anchoredPosition != target)
		{
			videoBtn.image.rectTransform.anchoredPosition = Vector2.MoveTowards(videoBtn.image.rectTransform.anchoredPosition, target, 500f * Time.unscaledDeltaTime);
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime / 2f);
			if (!visibleStarted && StaticConstants.GetPercentageBetweenPoints(videoBtn.image.rectTransform.anchoredPosition.x, 0f, -200f) > 0.65f)
			{
				StartCoroutine(MakeSkipVisible());
				visibleStarted = true;
			}
		}
	}

	protected IEnumerator MakeSkipVisible()
	{
		while (skipBtn.image.color.a < 1f)
		{
			skipBtn.image.color = new Color(skipBtn.image.color.r, skipBtn.image.color.g, skipBtn.image.color.b, skipBtn.image.color.a + 5f * Time.unscaledDeltaTime);
			skipText.color = new Color(1f, 1f, 1f, skipBtn.image.color.a);
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
		}
		skipBtn.interactable = true;
	}
}
