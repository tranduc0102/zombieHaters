using System.Collections;
using UnityEngine;

public class DropPlace : MonoBehaviour
{
	private int countEntries;

	[SerializeField]
	private GameObject prefabBomb;

	[SerializeField]
	private int bangSize = 5;

	[SerializeField]
	private int bombStartHeight = 50;

	[SerializeField]
	private AudioClip explosionSound;

	[SerializeField]
	private GameObject spriteObject;

	private SphereCollider sphereCollider;

	private ParticleSystem bangFx;

	private float tryTime = 5f;

	private void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		bangFx = GetComponentInChildren<ParticleSystem>();
		Invoke("ReadyToCheck", 0.5f);
	}

	private void ReadyToCheck()
	{
		StartCoroutine(CheckPlace());
	}

	private IEnumerator CheckPlace()
	{
		float startTry = Time.time;
		while (countEntries > 0)
		{
			base.transform.position = new Vector3(base.transform.position.x + (float)Random.Range(-1, 2), base.transform.position.y, base.transform.position.z + (float)Random.Range(-1, 2));
			if (Time.time > startTry + tryTime)
			{
				Object.Destroy(base.gameObject);
				yield break;
			}
			yield return null;
		}
		Object.Instantiate(prefabBomb, new Vector3(base.transform.position.x, bombStartHeight, base.transform.position.z), prefabBomb.transform.rotation).GetComponent<Rigidbody>().AddForce(0f, -5000f, 0f);
		spriteObject.SetActive(true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Obstacle")
		{
			countEntries++;
		}
		if (other.tag == "Cage" && spriteObject.activeSelf)
		{
			Object.Destroy(other.gameObject);
			StartCoroutine(Bang());
			spriteObject.SetActive(false);
			bangFx.Play();
			if (explosionSound != null && !SoundManager.Instance.soundIsMuted)
			{
				AudioSource.PlayClipAtPoint(explosionSound, base.transform.position);
			}
			Object.Destroy(base.gameObject, 1f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Obstacle")
		{
			countEntries--;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Zombie" || collision.gameObject.tag == "ZombieBoss")
		{
			BaseHuman component = collision.gameObject.GetComponent<BaseHuman>();
			float num = Vector3.Distance(base.transform.position, collision.transform.position);
			float num2 = component.maxCountHealth / num * 4f;
			if (component.TakeDamage((int)num2) < 0f)
			{
				DataLoader.Instance.SaveDeadByCapsule(collision.gameObject.GetComponent<ZombieHuman>().zombieType);
			}
		}
	}

	private IEnumerator Bang()
	{
		base.gameObject.layer = LayerMask.NameToLayer("DropPlace");
		sphereCollider.isTrigger = false;
		for (int i = 0; i < bangSize; i++)
		{
			sphereCollider.radius += 1f;
			yield return null;
		}
		sphereCollider.enabled = false;
	}
}
