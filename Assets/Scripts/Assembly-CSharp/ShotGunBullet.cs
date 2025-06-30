using UnityEngine;

public class ShotGunBullet : MonoBehaviour
{
	public float damage = 25f;

	[SerializeField]
	private float spread = 25f;

	[SerializeField]
	private float lifetime = 3f;

	[HideInInspector]
	public bool isCriticalDamage;

	private void Start()
	{
		BaseBullet[] componentsInChildren = GetComponentsInChildren<BaseBullet>();
		float num = spread * 2f / (float)componentsInChildren.Length;
		float num2 = (0f - num) * (float)(componentsInChildren.Length / 2);
		BaseBullet[] array = componentsInChildren;
		foreach (BaseBullet baseBullet in array)
		{
			baseBullet.damage = damage;
			baseBullet.isCriticalDamage = isCriticalDamage;
			baseBullet.transform.Rotate(0f, num2 + Random.Range(-3f, 3f), 0f);
			num2 += num;
			baseBullet.gameObject.SetActive(true);
		}
		Object.Destroy(base.gameObject, lifetime);
	}
}
