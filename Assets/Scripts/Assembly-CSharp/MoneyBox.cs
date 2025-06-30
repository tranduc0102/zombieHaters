using IAP;
using UnityEngine;

public class MoneyBox : MonoBehaviour
{
	[HideInInspector]
	public bool isBigCoin;

	private bool complete;

	[SerializeField]
	private ParticleSystem pickUpFx;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Survivor") && !complete)
		{
			if (!isBigCoin)
			{
				DataLoader.Instance.SavePickedMoneyBox(base.transform.position);
			}
			else
			{
				double num = MoneyCoinManager.instance.coinMoney * 10.0 * (double)(1f + PassiveAbilitiesManager.bonusHelper.GoldBonus) * (double)((!InAppManager.Instance.IsSubscribed()) ? 1f : 1.5f);
				DataLoader.Instance.UpdateInGameMoneyCounter(num);
				FloatingText floatingText = Object.Instantiate(GameManager.instance.floatingTextPrefab, base.transform.position, Quaternion.identity, TransformParentManager.Instance.fx);
				floatingText.StartBaseAnimation(AbbreviationUtility.AbbreviateNumber(num));
			}
			pickUpFx.transform.parent = null;
			pickUpFx.Play();
			Object.Destroy(pickUpFx.gameObject, 1f);
			Object.Destroy(base.gameObject);
			complete = true;
		}
	}
}
