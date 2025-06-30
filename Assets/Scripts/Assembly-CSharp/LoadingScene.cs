using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class LoadingScene : MonoBehaviour
{
	[SerializeField]
	private Image[] loadingImages;

	[SerializeField]
	private Image backGround;

	[SerializeField]
	private GameObject camX;

	[SerializeField]
	private GameObject noInternetPanel;

	private Vector2[] startpos;

	private Coroutine loading;

	private int currentImg;

	private bool camOnPos;

	private AsyncOperation operation;

	private Vector2 startSize;

	private void Start()
	{
		startSize = loadingImages[0].rectTransform.sizeDelta;
		for (int i = 0; i < loadingImages.Length; i++)
		{
			loadingImages[i].rectTransform.sizeDelta = Vector2.zero;
		}
/*		StartCoroutine(ConnectionCheck(1f));
*/	}

	private IEnumerator EndLoading()
	{
		yield return null;
		while (!operation.isDone)
		{
			yield return null;
		}
		Object.Destroy(camX);
		camOnPos = false;
		yield return new WaitForSeconds(0.1f);
		DetectCam();
		while (!camOnPos || !DataLoader.initialized)
		{
			yield return null;
		}
		StopCoroutine(loading);
		StartCoroutine(Decrease(loadingImages[GetCorrectID(loadingImages.Length, currentImg - 1)]));
		StatusBarManager.BarAnim(1);
		StatusBarManager.Show(SetCanvasBounds.Instance.isBoundsChanged);
		while (backGround.color.a > 0f)
		{
			Color color = backGround.color;
			color.a -= Time.deltaTime;
			backGround.color = color;
			yield return null;
		}
		SceneManager.UnloadSceneAsync("Loading");
	}

	private void DetectCam()
	{
		StartCoroutine("CamSpeed");
	}

	private IEnumerator CamSpeed()
	{
		GameObject cam = Object.FindObjectOfType<FreeLookCam>().gameObject;
		Vector3 oldPos2 = cam.transform.position;
		do
		{
			oldPos2 = cam.transform.position;
			yield return new WaitForFixedUpdate();
		}
		while (oldPos2 != cam.transform.position);
		camOnPos = true;
	}

	public void StartLoading()
	{
		StartCoroutine(Loading());
	}

	private IEnumerator Loading()
	{
		yield return new WaitForSeconds(0.5f);
		loading = StartCoroutine(LoadingAnimation());
		//while (!GPGSCloudSave.syncWithCloud)
		//{
		//	yield return null;
		//}
		operation = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
		StartCoroutine(EndLoading());
	}

	private IEnumerator LoadingAnimation()
	{
		while (true)
		{
			Image img = loadingImages[GetCorrectID(loadingImages.Length, currentImg)];
			img.rectTransform.sizeDelta = startSize;
			img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
			loadingImages[GetCorrectID(loadingImages.Length, currentImg)].rectTransform.sizeDelta = startSize;
			StartCoroutine(Decrease(loadingImages[GetCorrectID(loadingImages.Length, currentImg - 1)]));
			currentImg++;
			if (currentImg == loadingImages.Length)
			{
				currentImg = 0;
			}
			yield return new WaitForSeconds(0.08f);
		}
	}

	private IEnumerator Decrease(Image img)
	{
		Color color = img.color;
		while (img.rectTransform.sizeDelta.x > 0f)
		{
			float t = Time.deltaTime * 75f;
			color.a -= Time.deltaTime;
			img.color = color;
			img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x - t, img.rectTransform.sizeDelta.y - t);
			yield return null;
		}
	}

	private IEnumerator SetOnPos(RectTransform rect, Vector2 pos)
	{
		while (rect.anchoredPosition != pos)
		{
			rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, pos, Time.deltaTime * 250f);
			yield return null;
		}
	}

	private int GetCorrectID(int maxLength, int nextID)
	{
		if (nextID >= maxLength)
		{
			nextID -= maxLength;
		}
		if (nextID < 0)
		{
			nextID += maxLength;
		}
		return nextID;
	}

	private IEnumerator ConnectionCheck(float everySeconds)
	{
		if (!StaticConstants.NeedInternetConnection)
		{
			yield break;
		}
		yield return new WaitForSeconds(2f);
		while (true)
		{
			if (StaticConstants.IsConnectedToInternet())
			{
				noInternetPanel.SetActive(false);
			}
			else
			{
				noInternetPanel.SetActive(true);
			}
			yield return new WaitForSecondsRealtime(everySeconds);
		}
	}
}
