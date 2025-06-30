using UnityEngine;

public class UIObjectFollow : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	private void LateUpdate()
	{
		base.transform.position = target.position;
	}
}
