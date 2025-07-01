using UnityEngine;

public class TreeFx : MonoBehaviour
{
	private ParticleSystem particles;

	private void Start()
	{
		particles = GetComponentInChildren<ParticleSystem>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == null) return;
		if (other.tag == "Survivor" && !particles.isPlaying)
		{
			particles.Play();
		}
	}
}
