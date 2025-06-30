using System.Collections;
using UnityEngine;

public class BossSpawnManager : MonoBehaviour
{
	public Camera cam;

	private LayerMask groundLayerMask;

	private LayerMask obstacleLayerMask;

	private Vector3[,] viewportPoints = new Vector3[2, 2];

	[SerializeField]
	private GameObject bossSpawnFxPrefab;

	[SerializeField]
	private bool testMode;

	public static BossSpawnManager instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		groundLayerMask = 65536;
		obstacleLayerMask = 4112;
		if (testMode)
		{
			StartCoroutine(TestCoroutine());
		}
	}

	public Vector3 GetViewportPoint(float x, float y)
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(x, y, 0f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray.origin, ray.direction, out hitInfo, 100f, groundLayerMask) && hitInfo.transform.tag == "Ground")
		{
			return hitInfo.point;
		}
		Debug.LogError("Cant find Ground");
		return Vector3.zero;
	}

	public IEnumerator TestCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			Vector3 spawnVector;
			if (GetSpawnPosition(out spawnVector))
			{
				SpawnFx(spawnVector);
			}
		}
	}

	public bool GetSpawnPosition(out Vector3 spawnPosition)
	{
		spawnPosition = Vector3.zero;
		viewportPoints[0, 0] = GetViewportPoint(0f, 0f);
		viewportPoints[1, 1] = GetViewportPoint(1f, 1f);
		for (float num = 0.65f; num > 0f; num -= 0.05f)
		{
			if (!(num > 0.05f) || !(num < 0.45f))
			{
				for (float num2 = 0f; num2 < 1f; num2 += 0.05f)
				{
					spawnPosition = new Vector3(GetXPercentage(num2, viewportPoints[0, 0], viewportPoints[1, 1]), 0f, GetZPercentage(num, viewportPoints[0, 0], viewportPoints[1, 1]));
					if (Physics.OverlapSphere(spawnPosition, 0.5f, obstacleLayerMask, QueryTriggerInteraction.Collide).Length == 0)
					{
						Vector3 vector = new Vector3(spawnPosition.x, 1f, spawnPosition.z) - new Vector3(base.transform.position.x, 1f, base.transform.position.z);
						Vector3 direction = vector / vector.magnitude;
						float maxDistance = Vector3.Distance(new Vector3(spawnPosition.x, 1f, spawnPosition.z), new Vector3(base.transform.position.x, 1f, base.transform.position.z));
						Ray ray = new Ray(new Vector3(base.transform.position.x, 1f, base.transform.position.z), direction);
						if (!Physics.Raycast(ray, maxDistance, obstacleLayerMask) && Vector3.Distance(base.transform.position, spawnPosition) > 2.5f)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public float GetXPercentage(float percentage, Vector3 min, Vector3 max)
	{
		float num = Vector3.Distance(max, min);
		return min.x + num * percentage;
	}

	public float GetZPercentage(float percentage, Vector3 min, Vector3 max)
	{
		float num = Vector3.Distance(max, min);
		return min.z + num * percentage;
	}

	public void SpawnFx(Vector3 position)
	{
		Object.Destroy(Object.Instantiate(bossSpawnFxPrefab, position, Quaternion.identity, TransformParentManager.Instance.fx), 3f);
	}
}
