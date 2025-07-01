using System.Collections;
using UnityEngine;
using GUI = GuiInGame.GUI;

public class OverlayParticle : MonoBehaviour
{
	public ParticleSystem particleSystemPrefab;

	[Tooltip("For test. Click here only if you KNOW what are you doing!")]
	public bool completed = true;

	private Vector3 position;

	[HideInInspector]
	public ParticleSystem currentPS;

	public void Play(float delay = 0f)
	{
		Invoke("InvokePlay", delay);
	}

	public void InvokePlay()
	{
		if (currentPS == null)
		{
			CreatePS();
			return;
		}
		currentPS.transform.position = ParticleCamera.Instance.GetCameraPosition(base.transform.position);
		currentPS.Play();
		ParticleCamera.Instance.Play(currentPS.main.duration);
	}

	public void Stop()
	{
		CancelInvoke("InvokePlay");
		if (currentPS != null)
		{
			currentPS.Stop();
		}
	}

	public void CreatePS()
	{
		currentPS = Object.Instantiate(particleSystemPrefab, ParticleCamera.Instance.GetCameraPosition(base.transform.position), particleSystemPrefab.transform.rotation, TransformParentManager.Instance.fx);
		base.gameObject.SetActive(false);
		ParticleCamera.Instance.Play(currentPS.main.duration);
	}

	public void Preview()
	{
		StartCoroutine(PreviewCoroutine());
	}

	public IEnumerator PreviewCoroutine()
	{
		completed = false;
		RectTransform canvas = Object.FindObjectOfType<GUI>().GetComponent<RectTransform>();
		Camera cam = Object.FindObjectOfType<ParticleCamera>().GetComponent<Camera>();
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;
		Vector3 viewportPosition = Camera.main.ScreenToViewportPoint(base.transform.position);
		Vector2 anchoredPosition = new Vector2(viewportPosition.x * canvas.sizeDelta.x - canvas.sizeDelta.x / 2f, viewportPosition.y * canvas.sizeDelta.y - canvas.sizeDelta.y / 2f);
		Vector3 currentPosition = base.transform.position;
		base.transform.position = new Vector3(cam.transform.position.x + anchoredPosition.x * width / canvas.sizeDelta.x, cam.transform.position.y + anchoredPosition.y * height / canvas.sizeDelta.y, cam.transform.position.z + 10f);
		GetComponent<ParticleSystem>().Play();
		yield return new WaitForSeconds(particleSystemPrefab.main.duration);
		base.transform.position = currentPosition;
		completed = true;
	}
}
