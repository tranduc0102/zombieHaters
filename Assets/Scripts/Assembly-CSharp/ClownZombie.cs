using System;
using System.Collections;
using UnityEngine;

public class ClownZombie : ZombieHuman
{
	private bool readyToThrow = true;

	[SerializeField]
	private float throwDelay = 1f;

	[SerializeField]
	private float throwDistance = 10f;

	[SerializeField]
	private LayerMask maskThrowZombie;

	private TrailRenderer zombieTrail;

	private ParticleSystem flyEndFx;

	private void OnTriggerStay(Collider other)
	{
		if (!(other == null) && !(targetMove == null) && readyToThrow && other.tag == "Zombie" && Vector3.Distance(base.transform.position, targetMove.position) < throwDistance && Vector3.Distance(base.transform.position, targetMove.position) > 3f)
		{
			Vector3 direction = new Vector3(targetMove.position.x - other.transform.position.x, 0f, targetMove.position.z - other.transform.position.z);
			if (other.GetComponent<ZombieHuman>().size != ZombieSize.Big && !Physics.Raycast(other.transform.position, direction, Vector3.Distance(other.transform.position, targetMove.position), maskThrowZombie))
			{
				StartCoroutine(ThrowZombie(other.transform, targetMove.position));
				readyToThrow = false;
				Invoke("ReadyToThrow", throwDelay);
			}
		}
	}

	private void ReadyToThrow()
	{
		readyToThrow = true;
	}

	private IEnumerator ThrowZombie(Transform zombie, Vector3 targetPosition)
	{
		if (zombieTrail == null)
		{
			zombieTrail = UnityEngine.Object.Instantiate(GameManager.instance.prefabZombieTrail);
		}
		float maxHeight = 5f;
		float timeToMove = Vector3.Distance(zombie.position, targetPosition) / 10f;
		Vector3 currentPos = zombie.position;
		float t = 0f;
		CancelInvoke("DisableZombieTrail");
		while (t < 1.5f && zombie != null)
		{
			t += Time.deltaTime / timeToMove;
			Vector3 vv;
			if (t < 1f)
			{
				vv = Vector3.Lerp(currentPos, targetPosition, t);
			}
			else
			{
				vv = Vector3.Lerp(targetPosition, targetPosition - currentPos + targetPosition, t - 1f);
				if (zombie.position.y <= 0.1f)
				{
					break;
				}
			}
			vv.y += Mathf.Sin(t * (float)Math.PI) * maxHeight;
			zombie.position = vv;
			zombieTrail.transform.position = vv;
			zombieTrail.enabled = true;
			yield return null;
		}
		if (zombie != null)
		{
			if (flyEndFx == null)
			{
				flyEndFx = UnityEngine.Object.Instantiate(GameManager.instance.prefabFlyEndFx).GetComponent<ParticleSystem>();
			}
			flyEndFx.transform.position = new Vector3(zombie.position.x, flyEndFx.transform.position.y, zombie.position.z);
			flyEndFx.Play();
		}
		Invoke("DisableZombieTrail", 0.5f);
	}

	private void DisableZombieTrail()
	{
		zombieTrail.enabled = false;
	}

	private void OnDestroy()
	{
		if (zombieTrail != null)
		{
			UnityEngine.Object.Destroy(zombieTrail, 1f);
		}
		if (flyEndFx != null)
		{
			UnityEngine.Object.Destroy(flyEndFx, 1f);
		}
	}
}
