using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
	public float damage;

	public float damageDelay;

	private List<BaseHuman> targets = new List<BaseHuman>();

	private void OnEnable()
	{
		InvokeRepeating("ApplyDamage", damageDelay, damageDelay);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Zombie" || other.tag == "ZombieBoss")
		{
			targets.Add(other.GetComponent<BaseHuman>());
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.tag == "Zombie" || other.tag == "ZombieBoss")
		{
			targets.Remove(other.GetComponent<BaseHuman>());
		}
	}

	private void ApplyDamage()
	{
		for (int i = 0; i < targets.Count; i++)
		{
			targets[i].TakeDamage(damage);
		}
	}

	private void OnDisable()
	{
		targets.Clear();
		CancelInvoke();
	}
}
