using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
	public static BombManager instance;

	[SerializeField]
	private GameObject bombardmentWarningUI;

	[SerializeField]
	private GameObject dropPlace;

	[SerializeField]
	private Transform cameraTarget;

	[SerializeField]
	private float dropDistance = 5f;

	[SerializeField]
	private bool oneTime = true;

	[SerializeField]
	private float bombardmentDelay = 20f;

	[SerializeField]
	private float bombardmentTime = 10f;

	[SerializeField]
	private float dropDelay = 2f;

	[SerializeField]
	private float delayInOneDrop = 0.5f;

	[SerializeField]
	private int minCountBombs = 1;

	[SerializeField]
	private int maxCountBombs = 3;

	[Header("Ability Settings")]
	[SerializeField]
	private float dropAbilityDistance = 5f;

	[SerializeField]
	private int minAbilityCountBombs = 1;

	[SerializeField]
	private int maxAbilityCountBombs = 3;

	private float startBombardmentTime;

	private void Awake()
	{
		instance = this;
	}

	public void StartGame()
	{
		startBombardmentTime = Time.time + bombardmentDelay;
		Invoke("DropBomb", bombardmentDelay);
		bombardmentWarningUI.SetActive(false);
	}

	private void DropBomb()
	{
		StartCoroutine(CreateBombs(cameraTarget.position));
		if (Time.time - startBombardmentTime >= bombardmentTime)
		{
			if (!oneTime)
			{
				Invoke("DropBomb", bombardmentDelay);
			}
			startBombardmentTime = Time.time + bombardmentDelay;
			Invoke("HideWarning", 2f);
		}
		else
		{
			Invoke("DropBomb", dropDelay);
			bombardmentWarningUI.SetActive(true);
		}
	}

	private void HideWarning()
	{
		bombardmentWarningUI.SetActive(false);
	}

	public void OneMoreBomb()
	{
		StartCoroutine(CreateAbilityBombs(cameraTarget.position));
	}

	private IEnumerator CreateBombs(Vector3 position)
	{
		int bombsCount = Random.Range(minCountBombs, maxCountBombs + 1);
		for (int i = 0; i < bombsCount; i++)
		{
			Vector3 dropPos = new Vector3(position.x + Random.Range(0f - dropDistance, dropDistance), 100f, position.z + Random.Range(0f - dropDistance, dropDistance));
			Object.Instantiate(dropPlace, new Vector3(dropPos.x, dropPlace.transform.position.y, dropPos.z), dropPlace.transform.rotation);
			yield return new WaitForSeconds(delayInOneDrop);
		}
	}

	private IEnumerator CreateAbilityBombs(Vector3 position)
	{
		int bombsCount = Random.Range(minAbilityCountBombs, maxAbilityCountBombs + 1);
		for (int i = 0; i < bombsCount; i++)
		{
			Vector3 dropPos = new Vector3(position.x + Random.Range(0f - dropAbilityDistance, dropAbilityDistance), 100f, position.z + Random.Range(0f - dropAbilityDistance, dropAbilityDistance));
			Object.Instantiate(dropPlace, new Vector3(dropPos.x, dropPlace.transform.position.y, dropPos.z), dropPlace.transform.rotation);
			yield return new WaitForSeconds(delayInOneDrop);
		}
	}

	public void StopIt()
	{
		CancelInvoke("DropBomb");
		StopAllCoroutines();
		CancelInvoke("HideWarning");
		bombardmentWarningUI.SetActive(false);
	}
}
