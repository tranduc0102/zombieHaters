using UnityEngine;

public class NewSurvivor : MonoBehaviour
{
	public SaveData.HeroData.HeroType heroType;

	private SurvivorHuman survivor;

	private Collider survivorCollider;

	private bool complete;

	[SerializeField]
	private AudioClip takeSound;

	private void Start()
	{
		survivor = Object.Instantiate(DataLoader.Instance.GetSurvivorPrefab(heroType), base.transform).GetComponent<SurvivorHuman>();
		survivor.transform.localPosition = new Vector3(0f, 0f, -2.5f);
		survivor.body.rotation = new Quaternion(0f, 0.5f, 0f, 0f);
		survivor.enabled = false;
		survivorCollider = survivor.GetComponent<Collider>();
		survivorCollider.enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Survivor") && !complete && !(other.gameObject == survivor.gameObject))
		{
			survivor.transform.parent = null;
			survivor.enabled = true;
			survivorCollider.enabled = true;
			survivor.PlayWakeUpFx();
			SoundManager.Instance.PlaySound(takeSound);
			Object.Destroy(base.gameObject);
			complete = true;
			GameManager.instance.newSurvivorsLeft--;
			DataLoader.Instance.AddPickedUpCount(heroType);
		}
	}
}
