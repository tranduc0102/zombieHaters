using UnityEngine;

public class NoRotatable : MonoBehaviour
{
	[SerializeField]
	private bool x;

	[SerializeField]
	private bool y = true;

	[SerializeField]
	private bool z;

	private void Update()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (x)
		{
			localEulerAngles.x = 0f - base.transform.parent.eulerAngles.x;
		}
		if (y)
		{
			localEulerAngles.y = 0f - base.transform.parent.eulerAngles.y;
		}
		if (z)
		{
			localEulerAngles.z = 0f - base.transform.parent.eulerAngles.z;
		}
		base.transform.localEulerAngles = localEulerAngles;
	}
}
