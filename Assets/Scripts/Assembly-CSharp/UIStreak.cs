using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIStreak : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private Text textFront;

	[SerializeField]
	private Text textBack;

	[SerializeField]
	private int[] streakCount;

	[SerializeField]
	private float streakDelay;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private ParticleSystem fx;

	[SerializeField]
	private LanguageKeysEnum[] streakKeys;

	[SerializeField]
	private float delayBetweenShow = 20f;

	private bool isAllowToShow = true;

	private int currentStreak;

	private int lastStreakIndex;

	private float speed = 5f;

	private Coroutine textCoroutine;

	private Coroutine fxPlay;

	public static UIStreak instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		text.gameObject.SetActive(false);
	}

	private void SetStreak()
	{
		if (GameManager.instance.currentGameMode != GameManager.GameModes.PVP && currentStreak >= streakCount[0] && isAllowToShow)
		{
			StartTextCoroutine(LanguageManager.instance.GetLocalizedText(streakKeys[Random.Range(0, streakKeys.Length)]));
			ResetStreak();
			isAllowToShow = false;
			Invoke("ResetDelay", delayBetweenShow);
		}
	}

	public void ResetStreak()
	{
		currentStreak = 0;
		lastStreakIndex = 0;
	}

	private void StartTextCoroutine(string _text)
	{
		text.text = _text;
		textFront.text = text.text;
		textBack.text = text.text;
		anim.Play("Two");
		if (fxPlay != null)
		{
			StopCoroutine(fxPlay);
		}
		fxPlay = StartCoroutine(OnEnablePlay());
	}

	private void SwitchIndex(int index)
	{
		switch (index)
		{
		case 0:
			text.gameObject.SetActive(false);
			break;
		case 1:
			StartTextCoroutine("Killing Spree");
			break;
		case 2:
			StartTextCoroutine("Brutal");
			break;
		case 3:
			StartTextCoroutine("Rampage");
			break;
		case 4:
			StartTextCoroutine("Killing Frenzy");
			break;
		case 5:
			StartTextCoroutine("Overkill");
			break;
		case 6:
			StartTextCoroutine("Unstoppable");
			break;
		case 7:
			StartTextCoroutine("Killpocalypse");
			break;
		case 8:
			StartTextCoroutine("Killionaire");
			break;
		}
	}

	private void ResetDelay()
	{
		isAllowToShow = true;
	}

	public void IncreaseStreak()
	{
		currentStreak++;
		SetStreak();
		CancelInvoke("ResetStreak");
		Invoke("ResetStreak", streakDelay);
	}

	public IEnumerator TextCor()
	{
		text.gameObject.SetActive(true);
		text.transform.localScale = Vector3.zero;
		yield return null;
		while (text.transform.localScale != Vector3.one * 1.5f)
		{
			text.transform.localScale = Vector3.MoveTowards(text.transform.localScale, Vector3.one * 1.5f, Time.deltaTime * speed);
			yield return null;
		}
		while (text.transform.localScale != Vector3.one)
		{
			text.transform.localScale = Vector3.MoveTowards(text.transform.localScale, Vector3.one, Time.deltaTime * speed);
			yield return null;
		}
		yield return new WaitForSeconds(1.8f);
		while (text.transform.localScale != Vector3.zero)
		{
			text.transform.localScale = Vector3.MoveTowards(text.transform.localScale, Vector3.zero, Time.deltaTime * speed);
			yield return null;
		}
	}

	public void IsStreakKill()
	{
		int streakIndex = GetStreakIndex(currentStreak);
		if (lastStreakIndex < streakIndex)
		{
			lastStreakIndex = streakIndex;
			SwitchIndex(lastStreakIndex);
		}
	}

	public int GetStreakIndex(int count)
	{
		int i;
		for (i = 0; i < streakCount.Length && count >= streakCount[i]; i++)
		{
		}
		return i;
	}

	private IEnumerator OnEnablePlay()
	{
		while (!fx.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		fx.Play();
		while (fx.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		yield return null;
	}
}
