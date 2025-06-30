using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
	private const string ttt = "paz ";

	private Button initReport;

	private Button sendEvent;

	private Button sendActive;

	private Button setProperty;

	private Button initStorm;

	private Button loadReward;

	private Button rewardIsReady;

	private Button rewardShow;

	private Button loadInter;

	private Button interIsReady;

	private Button interShow;

	private Button showBanner;

	private Button destroyBanner;

	private ParticleSystem particle;

	private Text text;

	private InputField inputField;

	private string slotId = string.Empty;

	private void Start()
	{
		Log("啦啦啦");
		initReport = GameObject.Find("initReport").GetComponent<Button>();
		sendEvent = GameObject.Find("sendEvent").GetComponent<Button>();
		sendActive = GameObject.Find("sendActive").GetComponent<Button>();
		setProperty = GameObject.Find("setProperty").GetComponent<Button>();
		initStorm = GameObject.Find("initStorm").GetComponent<Button>();
		loadReward = GameObject.Find("loadReward").GetComponent<Button>();
		rewardIsReady = GameObject.Find("rewardIsReady").GetComponent<Button>();
		rewardShow = GameObject.Find("rewardShow").GetComponent<Button>();
		loadInter = GameObject.Find("loadInter").GetComponent<Button>();
		interIsReady = GameObject.Find("interIsReady").GetComponent<Button>();
		interShow = GameObject.Find("interShow").GetComponent<Button>();
		showBanner = GameObject.Find("ShowBanner").GetComponent<Button>();
		destroyBanner = GameObject.Find("DestroyBanner").GetComponent<Button>();
		particle = GameObject.Find("ParticleSystem").GetComponent<ParticleSystem>();
		text = GameObject.Find("textView").GetComponent<Text>();
		inputField = GameObject.Find("inputField").GetComponent<InputField>();
		initReport.onClick.AddListener(OnInitClick);
		sendEvent.onClick.AddListener(OnSendEventClick);
		sendActive.onClick.AddListener(OnSendActiveClick);
		setProperty.onClick.AddListener(OnSetPropertyClick);
		initStorm.onClick.AddListener(OnInitStormClick);
		loadReward.onClick.AddListener(OnLoadRewardClick);
		rewardIsReady.onClick.AddListener(OnRewardIsReadyClick);
		rewardShow.onClick.AddListener(OnRewardShowClick);
		loadInter.onClick.AddListener(OnLoadInterClick);
		interIsReady.onClick.AddListener(OnInterIsReadyClick);
		interShow.onClick.AddListener(OnInterShowClick);
		showBanner.onClick.AddListener(OnBannerShowClick);
		destroyBanner.onClick.AddListener(OnDestroyBannerClick);
		inputField.onValueChanged.AddListener(OnValueChanged);
	}

	public void OnValueChanged(string val)
	{
		slotId = val;
	}

	private void OnInitClick()
	{
		particle.Stop();
		Reporter.Init("http://192.168.5.222:11011", "ELctKLYrDm4fb6desm4gmm", "400", "gp", "bba9c62404", "0DNm0loY2qrkLUvNpU", "kRJCSN1RAo6TFOY4FV", "0DNm0loY2qrkLUvNpUJORsciTuZ0gIEwunX9", string.Empty);
		Reporter.SetDebug(true);
	}

	private void OnSendEventClick()
	{
		Reporter.SendEvent("catcatcat", "actactact", "lablablab", "valvalval", "extra", "eid");
	}

	private void OnSendActiveClick()
	{
		Reporter.SendRealActiveEvent();
	}

	private void OnSetPropertyClick()
	{
		Reporter.SetProperty("namename", "valuevalue");
	}

	private void OnInitStormClick()
	{
		/*DCGAds.InitializeSDK("Main Camera", "http://192.168.5.222:13107", "508", "FvijHlomwg8xwxds");
		DCGRewardsAds.onLoadSuccessHandler += rewardAdOnLoadSuccess;
		DCGRewardsAds.onLoadErrorHandler += rewardAdOnLoadError;
		DCGRewardsAds.onShowHandler += rewardAdOnShowStart;
		DCGRewardsAds.onCloseHandler += rewardAdOnShowFinish;
		DCGRewardsAds.onShowErrorHandler += rewardAdOnShowError;
		DCGInterstitialAds.onLoadSuccessHandler += interAdOnLoadSuccess;
		DCGInterstitialAds.onLoadErrorHandler += interAdOnLoadError;
		DCGInterstitialAds.onCloseHandler += interAdOnClose;
		DCGInterstitialAds.onClickedHandler += interAdOnClicked;
		DCGBannerAds.onLoadSuccessHandler += bannerAdOnLoadSuccess;
		DCGBannerAds.onLoadErrorHandler += bannerAdOnLoadError;
		DCGBannerAds.onClickedHandler += bannerAdOnClicked;
		DCGBannerAds.onCloseHandler += bannerAdOnClose;*/
	}

	private void OnLoadRewardClick()
	{
/*		DCGRewardsAds.LoadRewardAd("Main Camera", slotId);
*/	}

	private void OnRewardIsReadyClick()
	{
/*		bool flag = DCGRewardsAds.IsAdReady("Main Camera", slotId);
*//*		Log("reward ad " + slotId + " isReady: " + flag);
*/	}

	private void OnRewardShowClick()
	{
/*		DCGRewardsAds.PlayRewardAd("Main Camera", slotId);
*/	}

	private void OnLoadInterClick()
	{
/*		DCGInterstitialAds.LoadAd("Main Camera", slotId);
*/	}

	private void OnInterIsReadyClick()
	{
		//bool flag = DCGInterstitialAds.IsAdReady("Main Camera", slotId);
/*		Log("interstitial ad " + slotId + " isReady: " + flag);
*/	}

	private void OnInterShowClick()
	{
		/*DCGInterstitialAds.ShowAd("Main Camera", slotId);*/
	}

	private void OnBannerShowClick()
	{
/*		DCGBannerAds.ShowAd("Main Camera", slotId, BannerAd.POSTION_BOTTOM);
*/	}

	private void OnDestroyBannerClick()
	{
/*		DCGBannerAds.DestroyAd("Main Camera", slotId);
*/	}

	private void rewardAdOnLoadSuccess(string slotId)
	{
		Log("reward ad on load success , slotid: " + slotId);
	}

	private void rewardAdOnLoadError(string slotId, string error)
	{
		Log("reward ad on load error, slotid: " + slotId + " ,error: " + error);
	}

	private void rewardAdOnShowStart(string slotId)
	{
		Log("reward ad on show start , slotid: " + slotId);
	}

	private void rewardAdOnShowFinish(string slotId, bool isReward)
	{
		Log("reward ad on show finish , slotid: " + slotId + " ,isReward: " + isReward);
		particle.Play();
	}

	private void rewardAdOnShowError(string slotId, string error)
	{
		Log("reward ad on show error, slotid: " + slotId + " ,error: " + error);
	}

	private void interAdOnLoadSuccess(string slotId)
	{
		Log("interstitial ad on laod success, slotId:" + slotId);
	}

	private void interAdOnLoadError(string slotId, string error)
	{
		Log("interstitial ad on laod error, slotId:" + slotId + " ,error: " + error);
	}

	private void interAdOnClose(string slotId)
	{
		Log("interstitial ad on close, slotId:" + slotId);
	}

	private void interAdOnClicked(string slotId)
	{
		Log("interstitial ad on clicked, slotId:" + slotId);
	}

	private void bannerAdOnLoadSuccess(string slotId)
	{
		Log("interstitial ad on laod success, slotId:" + slotId);
	}

	private void bannerAdOnLoadError(string slotId, string error)
	{
		Log("interstitial ad on laod error, slotId:" + slotId + " ,error: " + error);
	}

	private void bannerAdOnClose(string slotId)
	{
		Log("interstitial ad on close, slotId:" + slotId);
	}

	private void bannerAdOnClicked(string slotId)
	{
		Log("interstitial ad on clicked, slotId:" + slotId);
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
	}

	private void Log(string msg)
	{
		Debug.Log("paz " + msg);
	}
}
