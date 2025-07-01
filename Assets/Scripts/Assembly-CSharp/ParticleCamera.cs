using UnityEngine;
using GUI = GuiInGame.GUI;

public class ParticleCamera : MonoBehaviour
{
    public new Camera camera;

	private RectTransform canvas;

	private float height;

	private float width;

	private int particleCount;

	public static ParticleCamera Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		GetObjects();
	}

	public void GetObjects()
	{
		camera = GetComponent<Camera>();
		GUI gUI = Object.FindObjectOfType<GUI>();
		canvas = gUI.GetComponent<RectTransform>();
		RenderTexture renderTexture = new RenderTexture((int)canvas.sizeDelta.x, (int)canvas.sizeDelta.y, 1);
		camera.targetTexture = renderTexture;
		gUI.rawImage.texture = renderTexture;
		height = 2f * camera.orthographicSize;
		width = height * camera.aspect;
	}

	public Vector3 GetCameraPosition(Vector3 position)
	{
		Vector3 vector = Camera.main.ScreenToViewportPoint(position);
		Vector2 vector2 = new Vector2(vector.x * canvas.sizeDelta.x - canvas.sizeDelta.x / 2f, vector.y * canvas.sizeDelta.y - canvas.sizeDelta.y / 2f);
		return new Vector3(base.transform.position.x + vector2.x * width / canvas.sizeDelta.x, base.transform.position.y + vector2.y * height / canvas.sizeDelta.y, base.transform.position.z + 10f);
	}

	public void Play(float playTime)
	{
		particleCount++;
		Invoke("DecreaseParticles", playTime);
	}

	public void DecreaseParticles()
	{
		particleCount--;
		if (particleCount <= 0)
		{
			particleCount = 0;
		}
	}
}
