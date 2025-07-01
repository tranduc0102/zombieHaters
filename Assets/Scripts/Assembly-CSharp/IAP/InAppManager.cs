using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace IAP
{
	public class InAppManager : MonoBehaviour, IStoreListener
	{
		private static IStoreController m_StoreController;

		private static IExtensionProvider m_StoreExtensionProvider;

		public List<CoinsPurchaseInfo> gemsPurchases;

		public List<HeroesPurchaseInfo> heroesPurchases;

		public List<BoosterPurchaseInfo> boosterPurchases;

		public List<StarterPackPurchaseInfo> packs;

		public PurchaseInfo infinityMultiplier;

		public PurchaseInfo noAds;

		public StarterPackPurchaseInfo starterPack;

		public PurchaseInfo subscription;

		[HideInInspector]
		public LocalSubscriptionManager localSubManager;

		private bool isSubscribed;

		private bool isLocalSubChecked;

		private SubscribedType subscribedType = SubscribedType.NotInitialized;

		public static InAppManager Instance { get; private set; }

		public InAppManager()
		{
			Instance = this;
		}

        private void Start()
		{
			if (m_StoreController == null)
			{
				InitializePurchasing();
			}
			if (!SaveManager.Load(StaticConstants.SubscriptionLocalDataPath, ref localSubManager))
			{
				localSubManager = new LocalSubscriptionManager();
				SaveLocalSubManager();
			}
		}

		public LocalSubscriptionManager GetLocalSubManager()
		{
			if (localSubManager == null && !SaveManager.Load(StaticConstants.SubscriptionLocalDataPath, ref localSubManager))
			{
				localSubManager = new LocalSubscriptionManager();
				SaveLocalSubManager();
			}
			return localSubManager;
		}

		public bool IsSubscribed()
		{
			try
			{
				if (subscribedType == SubscribedType.Subscribed)
				{
					isSubscribed = true;
				}
				else if (!isSubscribed && !isLocalSubChecked)
				{
					isLocalSubChecked = true;
					isSubscribed = localSubManager.IsSubscribed();
				}
				return isSubscribed;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return isSubscribed;
			}
		}

        public void InitializePurchasing()
		{
			if (!IsInitialized())
			{
				ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
				int lastIndex = 0;
				lastIndex = SetPurchases(gemsPurchases, builder, lastIndex);
				lastIndex = SetPurchases(heroesPurchases, builder, lastIndex);
				lastIndex = SetPurchases(boosterPurchases, builder, lastIndex);
				lastIndex = SetPurchases(packs, builder, lastIndex);
				lastIndex = SetPurchase(noAds, builder, lastIndex);
				lastIndex = SetPurchase(subscription, builder, lastIndex);
				lastIndex = SetPurchase(starterPack, builder, lastIndex);
				lastIndex = SetPurchase(infinityMultiplier, builder, lastIndex);
                UnityPurchasing.Initialize(this, builder);
			}
		}

		public void ResetAllPurchases()
		{
			int lastIndex = 0;
			lastIndex = ResetPurchases(gemsPurchases, lastIndex);
			lastIndex = ResetPurchases(heroesPurchases, lastIndex);
			lastIndex = ResetPurchases(boosterPurchases, lastIndex);
			lastIndex = ResetPurchases(packs, lastIndex);
			lastIndex = ResetPurchase(noAds, lastIndex);
			lastIndex = ResetPurchase(subscription, lastIndex);
			lastIndex = ResetPurchase(starterPack, lastIndex);
			lastIndex = ResetPurchase(infinityMultiplier, lastIndex);
		}

		private int ResetPurchases<T>(List<T> purchaseList, int lastIndex) where T : PurchaseInfo
		{
			for (int i = 0; i < purchaseList.Count; i++)
			{
				purchaseList[i].index = lastIndex;
				lastIndex++;
			}
			return lastIndex;
		}

		private int ResetPurchase<T>(T purchase, int lastIndex) where T : PurchaseInfo
		{
			purchase.index = lastIndex;
			return ++lastIndex;
		}

		public int SetPurchases<T>(List<T> purchases, ConfigurationBuilder builder, int lastIndex) where T : PurchaseInfo
		{
			for (int i = 0; i < purchases.Count; i++)
			{
				purchases[i].index = lastIndex;
				builder.AddProduct(purchases[i].purchaseName, purchases[i].productType, new IDs
				{
					{
						purchases[i].purchaseAppStore,
						"AppleAppStore"
					},
					{
						purchases[i].purchaseGooglePlay,
						"GooglePlay"
					}
				});
				lastIndex++;
			}
			return lastIndex;
		}

		public int SetPurchase(PurchaseInfo purchase, ConfigurationBuilder builder, int lastIndex)
		{
			purchase.index = lastIndex;
			builder.AddProduct(purchase.purchaseName, purchase.productType, new IDs
			{
				{ purchase.purchaseAppStore, "AppleAppStore" },
				{ purchase.purchaseGooglePlay, "GooglePlay" }
			});
			return ++lastIndex;
		}

		public void SaveLocalSubManager()
		{
			SaveManager.Save(localSubManager, StaticConstants.SubscriptionLocalDataPath);
		}

		public bool IsInitialized()
		{
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}

        public void BuyProductID(int productIndex)
		{
			if (IsInitialized())
			{
				Product product = GetProduct(productIndex);
				if (product != null && product.availableToPurchase)
				{
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					m_StoreController.InitiatePurchase(product);
				}
				else
				{
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			else
			{
				InitializePurchasing();
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}

		public Product GetProduct(int productIndex)
		{
			Product product;
			if (GetPurchaseInfoProduct(gemsPurchases, productIndex, out product))
			{
				return product;
			}
			if (GetPurchaseInfoProduct(heroesPurchases, productIndex, out product))
			{
				return product;
			}
			if (GetPurchaseInfoProduct(boosterPurchases, productIndex, out product))
			{
				return product;
			}
			if (GetPurchaseInfoProduct(packs, productIndex, out product))
			{
				return product;
			}
			if (infinityMultiplier.index == productIndex)
			{
				return m_StoreController.products.WithID(infinityMultiplier.purchaseName);
			}
			if (starterPack.index == productIndex)
			{
				return m_StoreController.products.WithID(starterPack.purchaseName);
			}
			if (noAds.index == productIndex)
			{
				return m_StoreController.products.WithID(noAds.purchaseName);
			}
			if (subscription.index == productIndex)
			{
				return m_StoreController.products.WithID(subscription.purchaseName);
			}
			return null;
		}

		public bool GetPurchaseInfoProduct<T>(List<T> purchases, int productIndex, out Product product) where T : PurchaseInfo
		{
			product = null;
			foreach (T purchase in purchases)
			{
				if (purchase.index == productIndex)
				{
					product = m_StoreController.products.WithID(purchase.purchaseName);
					return true;
				}
			}
			return false;
		}

		public void RestorePurchases()
		{
			if (!IsInitialized())
			{
				Debug.Log("RestorePurchases FAIL. Not initialized.");
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				Debug.Log("RestorePurchases started ...");
				IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				extension.RestoreTransactions(delegate(bool result)
				{
					Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
			}
			else
			{
				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("OnInitialized: Completed!");
			m_StoreController = controller;
			m_StoreExtensionProvider = extensions;
			GetSubscribedType(subscription);
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			bool flag = true;
			Debug.Log("Purchase type: " + args.purchasedProduct.definition.type);
			/*CrossPlatformValidator crossPlatformValidator = new CrossPlatformValidator(null, null, Application.identifier);
			try
			{
				IPurchaseReceipt[] array = crossPlatformValidator.Validate(args.purchasedProduct.receipt);
				Debug.Log("Receipt is valid. Contents:");
				IPurchaseReceipt[] array2 = array;
				foreach (IPurchaseReceipt purchaseReceipt in array2)
				{
					Debug.Log(purchaseReceipt.productID);
					Debug.Log(purchaseReceipt.purchaseDate);
					Debug.Log(purchaseReceipt.transactionID);
				}
			}
			catch (IAPSecurityException)
			{
				Debug.LogError("Invalid receipt, not unlocking content");
				flag = false;
			}*/
			if (flag)
			{
				int index;
				if (TryToProcessPurchase(args, gemsPurchases, out index))
				{
					GemsPurchased(gemsPurchases[index]);
				}
				else if (TryToProcessPurchase(args, heroesPurchases, out index))
				{
					HeroesPurchased(heroesPurchases[index]);
				}
				else if (TryToProcessPurchase(args, boosterPurchases, out index))
				{
					BoosterPurchased(boosterPurchases[index]);
				}
				else if (TryToProcessPurchase(args, packs, out index))
				{
					PackPurchased(packs[index]);
				}
				else if (TryToProcessPurchase(args, infinityMultiplier))
				{
					InfinityMultiplierPurchased();
				}
				else if (TryToProcessPurchase(args, starterPack))
				{
					StarterPurchased();
				}
				else if (TryToProcessPurchase(args, noAds))
				{
					NoAdsPurchased();
				}
				else if (TryToProcessPurchase(args, subscription))
				{
					SubscriptionPurchased();
				}
				Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
/*				NoAdsManager.instance.CheckNoAds();
*/			}
			return PurchaseProcessingResult.Complete;
		}

		public bool TryToProcessPurchase<T>(PurchaseEventArgs args, List<T> purchases, out int index) where T : PurchaseInfo
		{
			index = 0;
			foreach (T purchase in purchases)
			{
				if (string.Equals(args.purchasedProduct.definition.id, purchase.purchaseName, StringComparison.Ordinal))
				{
					return true;
				}
				index++;
			}
			return false;
		}

		public bool TryToProcessPurchase(PurchaseEventArgs args, PurchaseInfo purchase)
		{
			if (string.Equals(args.purchasedProduct.definition.id, purchase.purchaseName, StringComparison.Ordinal))
			{
				return true;
			}
			return false;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		}

		private void GemsPurchased(CoinsPurchaseInfo p)
		{
			Debug.LogWarning("Purchased " + p.purchaseName + " reward = " + p.reward);
			DataLoader.Instance.RefreshGems(p.reward, true);
			AnalyticsManager.instance.LogPurchaseEvent("GemsPurchased", new Dictionary<string, string>
			{
				{ "PurchaseName", p.purchaseName },
				{
					"Amount",
					p.reward.ToString()
				}
			}, (float)m_StoreController.products.WithID(p.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(p.purchaseName).metadata.isoCurrencyCode);
		}

		private void HeroesPurchased(HeroesPurchaseInfo p)
		{
			Debug.LogWarning("Purchased " + p.purchaseName + " Hero = " + p.heroType);
			if (DataLoader.Instance.OpenHero(p.heroType))
			{
				AnalyticsManager.instance.LogPurchaseEvent("HeroPurchased", new Dictionary<string, string> { { "PurchaseName", p.purchaseName } }, (float)m_StoreController.products.WithID(p.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(p.purchaseName).metadata.isoCurrencyCode);
			}
		}

		private void BoosterPurchased(BoosterPurchaseInfo p)
		{
			Debug.LogWarning("Purchased " + p.purchaseName + " Booster = " + p.boosterType);
			DataLoader.Instance.BuyBoosters(p.boosterType, p.boosterCount);
			if (DataLoader.gui.popUpsPanel.shop.gameObject.activeInHierarchy)
			{
				UiShopBooster[] source = UnityEngine.Object.FindObjectsOfType<UiShopBooster>();
				source.First((UiShopBooster b) => b.boosterType == p.boosterType).PurchaseFx();
			}
			else
			{
				DataLoader.gui.popUpsPanel.ingameShop.UpdateAfterPurchase();
			}
			AnalyticsManager.instance.LogPurchaseEvent("BoosterPurchased", new Dictionary<string, string> { { "PurchaseName", p.purchaseName } }, (float)m_StoreController.products.WithID(p.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(p.purchaseName).metadata.isoCurrencyCode);
		}

		private void PackPurchased(StarterPackPurchaseInfo p)
		{
			Debug.LogWarning("Purchased" + p.purchaseName + "Pack Type = " + p.starterType);
			PlayerPrefs.SetInt(StaticConstants.starterPackPurchased, (int)p.starterType);
			DataLoader.Instance.RefreshGems(p.reward, true);
			DataLoader.gui.popUpsPanel.starterPack.GetNextPack();
			DataLoader.gui.popUpsPanel.starterPack.Show();
			for (int i = 0; i < p.boosters.Count; i++)
			{
				DataLoader.Instance.BuyBoosters(p.boosters[i].boosterType, p.boosters[i].amount);
			}
			AnalyticsManager.instance.LogPurchaseEvent("PackPurchased", new Dictionary<string, string> { 
			{
				"Type",
				p.starterType.ToString()
			} }, (float)m_StoreController.products.WithID(p.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(p.purchaseName).metadata.isoCurrencyCode);
		}

		private void InfinityMultiplierPurchased()
		{
			Debug.LogWarning("Purchased " + infinityMultiplier.purchaseName);
			PlayerPrefs.SetInt(StaticConstants.MultiplierKey, 2);
			DataLoader.dataUpdateManager.UpdateMoneyMultiplier();
			if (!PlayerPrefs.HasKey(StaticConstants.infinityMultiplierPurchased))
			{
				PlayerPrefs.SetInt(StaticConstants.infinityMultiplierPurchased, 1);
				DataLoader.gui.videoMultiplier.UpdateContent();
				AnalyticsManager.instance.LogPurchaseEvent("InfinityMultiplierPurchased", new Dictionary<string, string>(), (float)m_StoreController.products.WithID(infinityMultiplier.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(infinityMultiplier.purchaseName).metadata.isoCurrencyCode);
			}
		}

		private void StarterPurchased()
		{
			Debug.LogWarning("Purchased" + starterPack.purchaseName);
			if (!PlayerPrefs.HasKey(StaticConstants.starterPackPurchased))
			{
				PlayerPrefs.SetInt(StaticConstants.starterPackPurchased, 1);
				DataLoader.Instance.RefreshGems(starterPack.reward, true);
				if (DataLoader.gui.popUpsPanel.starterPack.gameObject.activeInHierarchy)
				{
					DataLoader.gui.popUpsPanel.gameObject.SetActive(false);
				}
				DataLoader.gui.popUpsPanel.starterPack.Show();
				for (int i = 0; i < starterPack.boosters.Count; i++)
				{
					DataLoader.Instance.BuyBoosters(starterPack.boosters[i].boosterType, starterPack.boosters[i].amount);
				}
				AnalyticsManager.instance.LogPurchaseEvent("StarterPackPurchased", new Dictionary<string, string>(), (float)m_StoreController.products.WithID(starterPack.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(starterPack.purchaseName).metadata.isoCurrencyCode);
			}
		}

		private void NoAdsPurchased()
		{
			Debug.Log("Purchased:" + noAds.purchaseName);
			if (!PlayerPrefs.HasKey(StaticConstants.interstitialAdsKey))
			{
				PlayerPrefs.SetInt(StaticConstants.interstitialAdsKey, 1);
				AnalyticsManager.instance.LogPurchaseEvent("NoAdsPurchased", new Dictionary<string, string>(), (float)m_StoreController.products.WithID(noAds.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(noAds.purchaseName).metadata.isoCurrencyCode);
			}
		}

		private void SubscriptionPurchased()
		{
			Debug.Log("Subscription Purchased");
			Product product = GetProduct(subscription.index);
			try
			{
				IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
				extension.SetStorePromotionVisibility(product, AppleStorePromotionVisibility.Show);
				Dictionary<string, string> introductoryPriceDictionary = extension.GetIntroductoryPriceDictionary();
				if (product.receipt != null)
				{
					if (checkIfProductIsAvailableForSubscriptionManager(product.receipt))
					{
						string intro_json = ((introductoryPriceDictionary != null && introductoryPriceDictionary.ContainsKey(product.definition.storeSpecificId)) ? introductoryPriceDictionary[product.definition.storeSpecificId] : null);
						SubscriptionManager subscriptionManager = new SubscriptionManager(product, intro_json);
						SubscriptionInfo subscriptionInfo = subscriptionManager.getSubscriptionInfo();
						localSubManager.purchaseDate = TimeManager.CurrentDateTime;
						localSubManager.expiredDate = TimeManager.CurrentDateTime.Add(subscriptionInfo.getRemainingTime());
						SaveLocalSubManager();
					}
					else
					{
						Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
					}
				}
				else
				{
					Debug.Log("the product should have a valid receipt");
				}
			}
			catch (ReceiptParserException exception)
			{
				Debug.LogException(exception);
			}
			subscribedType = SubscribedType.Subscribed;
			isSubscribed = true;
			DataLoader.gui.popUpsPanel.subscription.UpdateContent();
			AnalyticsManager.instance.LogPurchaseEvent("SubscriptionPurchased", new Dictionary<string, string>(), (float)m_StoreController.products.WithID(subscription.purchaseName).metadata.localizedPrice, m_StoreController.products.WithID(starterPack.purchaseName).metadata.isoCurrencyCode);
		}

		public string GetPrice(string purchaseName)
		{
			if (m_StoreController != null)
			{
				return m_StoreController.products.WithID(purchaseName).metadata.localizedPriceString;
			}
			return string.Empty;
		}

		public PurchaseInfo GetCoinPackPurchase(int index)
		{
			return gemsPurchases[index];
		}

		private void OnDeferred(Product item)
		{
			Debug.Log("Purchase deferred: " + item.definition.id);
		}

		public DateTime GetSubscriptionExpiredDate(PurchaseInfo purchaseInfo)
		{
			DateTime result = DateTime.MinValue;
			if (IsInitialized())
			{
				Product product = GetProduct(purchaseInfo.index);
				try
				{
					IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
					extension.SetStorePromotionVisibility(product, AppleStorePromotionVisibility.Show);
					Dictionary<string, string> introductoryPriceDictionary = extension.GetIntroductoryPriceDictionary();
					if (product.receipt != null)
					{
						if (checkIfProductIsAvailableForSubscriptionManager(product.receipt))
						{
							string intro_json = ((introductoryPriceDictionary != null && introductoryPriceDictionary.ContainsKey(product.definition.storeSpecificId)) ? introductoryPriceDictionary[product.definition.storeSpecificId] : null);
							SubscriptionManager subscriptionManager = new SubscriptionManager(product, intro_json);
							SubscriptionInfo subscriptionInfo = subscriptionManager.getSubscriptionInfo();
							result = subscriptionInfo.getExpireDate();
						}
						else
						{
							Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
						}
					}
					else
					{
						Debug.Log("the product should have a valid receipt");
					}
				}
				catch (ReceiptParserException exception)
				{
					Debug.LogException(exception);
				}
			}
			return result;
		}

		public SubscribedType GetSubscribedType(PurchaseInfo purchaseInfo)
		{
			if (!IsInitialized())
			{
				subscribedType = SubscribedType.NotInitialized;
			}
			else
			{
				subscribedType = SubscribedType.Initialized;
			}
			if (subscribedType == SubscribedType.Initialized)
			{
				Product product = GetProduct(purchaseInfo.index);
				try
				{
					IAppleExtensions extension = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
					extension.SetStorePromotionVisibility(product, AppleStorePromotionVisibility.Show);
					Dictionary<string, string> introductoryPriceDictionary = extension.GetIntroductoryPriceDictionary();
					if (product.receipt != null)
					{
						if (checkIfProductIsAvailableForSubscriptionManager(product.receipt))
						{
							string intro_json = ((introductoryPriceDictionary != null && introductoryPriceDictionary.ContainsKey(product.definition.storeSpecificId)) ? introductoryPriceDictionary[product.definition.storeSpecificId] : null);
							SubscriptionManager subscriptionManager = new SubscriptionManager(product, intro_json);
							SubscriptionInfo subscriptionInfo = subscriptionManager.getSubscriptionInfo();
							Debug.Log("product id is: " + subscriptionInfo.getProductId());
							Debug.Log("purchase date is: " + subscriptionInfo.getPurchaseDate());
							Debug.Log("subscription next billing date is: " + subscriptionInfo.getExpireDate());
							Debug.Log("is subscribed? " + subscriptionInfo.isSubscribed());
							Debug.Log("is expired? " + subscriptionInfo.isExpired());
							Debug.Log("is cancelled? " + subscriptionInfo.isCancelled());
							Debug.Log("product is in free trial peroid? " + subscriptionInfo.isFreeTrial());
							Debug.Log("product is auto renewing? " + subscriptionInfo.isAutoRenewing());
							Debug.Log("subscription remaining valid time until next billing date is: " + subscriptionInfo.getRemainingTime());
							Debug.Log("is this product in introductory price period? " + subscriptionInfo.isIntroductoryPricePeriod());
							Debug.Log("the product introductory localized price is: " + subscriptionInfo.getIntroductoryPrice());
							Debug.Log("the product introductory price period is: " + subscriptionInfo.getIntroductoryPricePeriod());
							Debug.Log("the number of product introductory price period cycles is: " + subscriptionInfo.getIntroductoryPricePeriodCycles());
							localSubManager.purchaseDate = TimeManager.CurrentDateTime;
							localSubManager.expiredDate = TimeManager.CurrentDateTime.Add(subscriptionInfo.getRemainingTime());
							SaveLocalSubManager();
							if (subscriptionInfo.isSubscribed() == Result.True)
							{
								subscribedType = SubscribedType.Subscribed;
							}
							else
							{
								subscribedType = SubscribedType.Expired;
							}
						}
						else
						{
							Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
							subscribedType = SubscribedType.ValidationFailed;
						}
					}
					else
					{
						Debug.Log("the product should have a valid receipt");
						subscribedType = SubscribedType.ValidationFailed;
					}
				}
				catch (ReceiptParserException exception)
				{
					Debug.LogException(exception);
					subscribedType = SubscribedType.ValidationFailed;
				}
			}
			return subscribedType;
		}

		private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
			if (!dictionary.ContainsKey("Store") || !dictionary.ContainsKey("Payload"))
			{
				Debug.Log("The product receipt does not contain enough information");
				return false;
			}
			string text = (string)dictionary["Store"];
			string text2 = (string)dictionary["Payload"];
			if (text2 != null)
			{
				switch (text)
				{
				case "GooglePlay":
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)MiniJson.JsonDecode(text2);
					if (!dictionary2.ContainsKey("json"))
					{
						Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
						return false;
					}
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)MiniJson.JsonDecode((string)dictionary2["json"]);
					if (dictionary3 == null || !dictionary3.ContainsKey("developerPayload"))
					{
						Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
						return false;
					}
					string json = (string)dictionary3["developerPayload"];
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)MiniJson.JsonDecode(json);
					if (dictionary4 == null || !dictionary4.ContainsKey("is_free_trial") || !dictionary4.ContainsKey("has_introductory_price_trial"))
					{
						Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
						return false;
					}
					return true;
				}
				case "AppleAppStore":
				case "AmazonApps":
				case "MacAppStore":
					return true;
				default:
					return false;
				}
			}
			return false;
		}

		public bool IsSubscriptionEnabled(PurchaseInfo purchaseInfo)
		{
			bool result = false;
			Product product = GetProduct(purchaseInfo.index);
			if (product.definition.type != ProductType.Subscription)
			{
				Debug.LogWarning(string.Format("Product type is {0}, expected {1}", product.definition.type, ProductType.Subscription));
				return false;
			}
			try
			{
				GooglePurchaseData googlePurchaseData = new GooglePurchaseData(product.receipt);
				if (product.hasReceipt)
				{
					Debug.Log("Is autoRenewing: " + googlePurchaseData.json.autoRenewing);
					if (googlePurchaseData.json.autoRenewing == "true")
					{
						result = true;
					}
					Debug.Log("Order ID: " + googlePurchaseData.json.orderId);
					Debug.Log("Package name: " + googlePurchaseData.json.packageName);
					Debug.Log("Product ID:" + googlePurchaseData.json.productId);
					Debug.Log("Purchase time: " + googlePurchaseData.json.purchaseTime);
					Debug.Log("Purchase state: " + googlePurchaseData.json.purchaseState);
					Debug.Log("Purchase token: " + googlePurchaseData.json.purchaseToken);
				}
				else
				{
					Debug.Log("Product receipt invalid: " + product.receipt);
				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			return result;
		}

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new NotImplementedException();
        }
    }
}
