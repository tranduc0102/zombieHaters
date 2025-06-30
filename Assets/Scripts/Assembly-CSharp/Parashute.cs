using UnityEngine;

public class Parashute : MonoBehaviour
{
	private SurvivorHuman survivor;

	private Collider survivorCollider;

	private Rigidbody survivorRigidbody;

	private Rigidbody rigid;

	private CameraTarget cameraTarget;

	[SerializeField]
	private Transform survivorPlace;

	[SerializeField]
	private float speed = 10f;

	private void Start()
	{
		rigid = GetComponent<Rigidbody>();
		cameraTarget = Object.FindObjectOfType<CameraTarget>();
		survivor = Object.Instantiate(DataLoader.Instance.GetSurvivorPrefab(SpawnManager.instance.GetRandomSurvivorType()), survivorPlace).GetComponent<SurvivorHuman>();
		survivor.transform.localPosition = Vector3.zero;
		survivor.animator.SetBool("Rest", false);
		survivor.body.rotation = default(Quaternion);
		survivor.enabled = false;
		survivorCollider = survivor.GetComponent<Collider>();
		survivorCollider.enabled = false;
		survivorRigidbody = survivor.GetComponent<Rigidbody>();
		survivorRigidbody.isKinematic = true;
	}

	private void FixedUpdate()
	{
		if (base.transform.position.y <= 0f)
		{
			survivor.transform.parent = null;
			survivor.enabled = true;
			survivor.transform.rotation = default(Quaternion);
			survivorCollider.enabled = true;
			survivorRigidbody.isKinematic = false;
			Object.Destroy(base.gameObject);
		}
		rigid.velocity = Vector3.MoveTowards(Vector3.zero, (cameraTarget.transform.position - base.transform.position) * 10000f, 1f) * speed;
		float y = base.transform.eulerAngles.y;
		base.transform.LookAt(cameraTarget.transform);
		base.transform.eulerAngles = new Vector3(0f, Mathf.Lerp(y, base.transform.eulerAngles.y, 0.5f), 0f);
	}
}
