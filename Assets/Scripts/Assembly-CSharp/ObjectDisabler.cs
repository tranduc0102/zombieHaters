using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
	[SerializeField]
	private float time;

	public void OnEnable()
	{
		Invoke("DisableGameObject", time);
	}

	public void DisableGameObject()
	{
		base.gameObject.SetActive(false);
	}

	public void OnDisable()
	{
		CancelInvoke();
	}
}
