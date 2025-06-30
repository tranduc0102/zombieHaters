using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class Loading : MonoBehaviour
{
	[SerializeField]
	private Image[] loadingImages;

	[SerializeField]
	private Image backGround;

	private Vector2 startSize = Vector2.zero;

	private int currentImg;

	private Coroutine loadingAnim;

	public void StartLoading()
	{
		backGround.color += new Color(backGround.color.r, backGround.color.g, backGround.color.b, 1.1f);
		if (startSize == Vector2.zero)
		{
			startSize = loadingImages[0].rectTransform.sizeDelta;
		}
		for (int i = 0; i < loadingImages.Length; i++)
		{
			loadingImages[i].rectTransform.sizeDelta = Vector2.zero;
		}
		base.gameObject.SetActive(true);
		StatusBarManager.BarAnim(0);
		StatusBarManager.Show(false);
		loadingAnim = StartCoroutine(LoadingAnimation());
	}

	public void EndLoading()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(CamSpeed());
		}
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
			yield return new WaitForSecondsRealtime(0.08f);
		}
	}

	private IEnumerator Decrease(Image img)
	{
		Color color = img.color;
		while (img.rectTransform.sizeDelta.x > 0f)
		{
			float t = 6f;
			color.a -= 0.08f;
			img.color = color;
			img.rectTransform.sizeDelta = new Vector2(img.rectTransform.sizeDelta.x - t, img.rectTransform.sizeDelta.y - t);
			yield return new WaitForSecondsRealtime(0.02f);
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

	private IEnumerator CamSpeed()
	{
		GameObject cam = Object.FindObjectOfType<FreeLookCam>().gameObject;
		Vector3 cameraTargetPos = Object.FindObjectOfType<CameraTarget>().transform.position;
		cam.transform.position = new Vector3(cameraTargetPos.x, cam.transform.position.y, cameraTargetPos.z);
		if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
		{
			Time.timeScale = 1f;
			yield return null;
			Time.timeScale = 0f;
		}
		StopCoroutine(loadingAnim);
		StartCoroutine(Decrease(loadingImages[GetCorrectID(loadingImages.Length, currentImg - 1)]));
		StatusBarManager.BarAnim(1);
		StatusBarManager.Show(SetCanvasBounds.Instance.isBoundsChanged);
		while (backGround.color.a > 0f)
		{
			Color color = backGround.color;
			color.a -= 0.05f * color.a + 0.02f;
			backGround.color = color;
			yield return new WaitForSecondsRealtime(0.02f);
		}
		StopAllCoroutines();
		base.gameObject.SetActive(false);
		if (GameManager.instance.currentGameMode == GameManager.GameModes.Arena)
		{
			GameManager.instance.ArenaReadyToStart();
		}
		if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
		{
			GameManager.instance.PVPReadyToStart();
		}
	}
}
