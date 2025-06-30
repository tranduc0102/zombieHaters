using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UIShopCoinPack : MonoBehaviour
{
	public Text lbl_amount;

	public Text lbl_price;

	public GameObject priceLoader;

	private Coroutine priceCor;

	[SerializeField]
	private Button btn_Buy;

	private PurchaseInfo p;

	public void GetPackPrice(PurchaseInfo purchase)
	{
		UIController.instance.StartGetPrice(lbl_price, priceLoader, purchase, ref priceCor);
		if (p == null || p != purchase)
		{
			p = purchase;
			btn_Buy.onClick.RemoveAllListeners();
			btn_Buy.onClick.AddListener(delegate
			{
				InAppManager.Instance.BuyProductID(p.index);
			});
		}
	}
}
