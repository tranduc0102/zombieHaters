using System;
using System.Collections;
using UnityEngine;

public class JumpingZombie : ZombieHuman
{
	private bool readyToThrow = true;

	[SerializeField]
	private float jumpDelay = 1f;

	[SerializeField]
	private float jumpDistance = 10f;

	[SerializeField]
	private LayerMask maskJumpZombie;

	private new void FixedUpdate()
	{
		base.FixedUpdate();
		if (!(targetMove == null) && readyToThrow && Vector3.Distance(base.transform.position, targetMove.position) < jumpDistance && Vector3.Distance(base.transform.position, targetMove.position) > 3f)
		{
			Vector3 direction = new Vector3(targetMove.position.x - base.transform.position.x, 0f, targetMove.position.z - base.transform.position.z);
			if (!Physics.Raycast(base.transform.position, direction, Vector3.Distance(base.transform.position, targetMove.position), maskJumpZombie))
			{
				StartCoroutine(Jump());
				readyToThrow = false;
				Invoke("ReadyToThrow", jumpDelay);
			}
		}
	}

	private void ReadyToThrow()
	{
		readyToThrow = true;
	}

	private IEnumerator Jump()
	{
		Vector3 targetPosition = targetMove.position;
		float maxHeight = 2f;
		float timeToMove = Vector3.Distance(base.transform.position, targetPosition) / 15f;
		Vector3 currentPos = base.transform.position;
		float t = 0f;
		while (t < 1.5f)
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
				if (base.transform.position.y <= 0f)
				{
					break;
				}
			}
			vv.y += Mathf.Sin(t * (float)Math.PI) * maxHeight;
			base.transform.position = vv;
			yield return null;
		}
	}
}
