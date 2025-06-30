/*using IAP;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsManager : MonoBehaviour
{
	[SerializeField]
	private Animator noAdsButtonAnim;

	[SerializeField]
	private Button shopNoAdsBuyButton;

	[SerializeField]
	private GameObject noAdsDone;

	[SerializeField]
	private GameObject textPriceObj;

	public static NoAdsManager instance { get; private set; }

	public void Awake()
	{
		instance = this;
	}

	public void Start()
	{
		CheckNoAds();
	}

	public void CheckNoAds()
	{
		bool flag = NoAdsPurchased();
		shopNoAdsBuyButton.interactable = !flag;
		noAdsDone.SetActive(flag);
		textPriceObj.SetActive(!flag);
		noAdsButtonAnim.SetBool("IsOpened", !flag);
	}

	public void BuyNoAds()
	{
		InAppManager.Instance.BuyProductID(InAppManager.Instance.noAds.index);
	}

	public bool NoAdsPurchased()
	{
		return PlayerPrefs.HasKey(StaticConstants.interstitialAdsKey);
	}
}
*/