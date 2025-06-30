using UnityEngine;

public class RPG : Mine
{
	[SerializeField]
	private int speed = 15;

	private void Start()
	{
		rigid.velocity = new Vector3(base.transform.right.x, 0f, base.transform.right.z) * speed;
	}
}
