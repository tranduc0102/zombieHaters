using UnityEngine;

public class PVPLootBox : MonoBehaviour
{
	[SerializeField]
	private LootObject lootPrefab;

	public float hitsCount;

	private bool lootDropped;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag != "Survivor")
		{
			TakeDamage();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag != "Survivor")
		{
			TakeDamage();
		}
	}

	public void TakeDamage()
	{
		hitsCount -= 1f;
		if (hitsCount <= 0f)
		{
			DropLoot();
		}
	}

	private void DropLoot()
	{
		if (!lootDropped)
		{
			lootDropped = true;
			PVPManager.Instance.RemoveLootBox(this);
			SpawnLoot();
		}
	}

	private void SpawnLoot()
	{
		int num = Random.Range(15, 25);
		for (int i = 0; i < num; i++)
		{
			LootObject lootObject = Object.Instantiate(lootPrefab, base.transform.position, Quaternion.identity);
			lootObject.MoveToPosition(base.transform.position, base.transform.position + GetPositionInCircle(), 1f);
		}
	}

	private Vector3 GetPositionInCircle()
	{
		Vector3 result = Random.onUnitSphere * 3.5f;
		result.y = 0.4f;
		return result;
	}
}
