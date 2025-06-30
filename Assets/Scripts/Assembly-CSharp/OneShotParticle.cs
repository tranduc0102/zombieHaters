using UnityEngine;

public class OneShotParticle : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem ps;

	private void OnEnable()
	{
		Invoke("DisableGameObject", ps.main.duration);
	}

	private void DisableGameObject()
	{
		base.gameObject.SetActive(false);
	}
}
