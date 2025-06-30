using UnityEngine;

public class ZombieDamageZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Zombie")
		{
			ZombieHuman component = other.GetComponent<ZombieHuman>();
			component.TakeDamage(component.maxCountHealth + 10f);
		}
	}
}
