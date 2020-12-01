using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdaptySDK;

public class AdaptyListener : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
		Adapty.setOnPromoReceivedListener(this);
		Adapty.setOnPurchaserInfoUpdatedListener(this);
	}

	// Button Clicks

	public void GetPaywallsButtonClick() {
		Adapty.getPaywalls(this);
	}

	public void RestorePurchasesButtonClick() {
		Adapty.restorePurchases(this);
	}

	public void GetPurchaserInfoButtonClick() {
		Adapty.getPurchaserInfo(this);
	}

	public void GetPromoButtonClick() {
		Adapty.getPromo(this);
	}

	public void IdentifyButtonClick() {
		Adapty.identify("my_unity_user_id", this);
	}

	public void LogoutButtonClick() {
		Adapty.logout(this);
	}

	List<ProductModel> retrievedProducts = new List<ProductModel>();
	public void MakePurchaseButtonClick() {
		if (retrievedProducts.Count > 0) {
			Adapty.makePurchase(retrievedProducts[0], null, this);
			retrievedProducts.RemoveAt(0);
		}
	}

	public void SyncPurchasesButtonClick() {
		Adapty.syncPurchases(this);
	}

	string retrievedReceipt = null;
	public void ValidateReceiptButtonClick() {
		if (retrievedReceipt != null) {
			Adapty.validateAppleReceipt(retrievedReceipt, this);
		}
	}

	public void UpdateAttributionButtonClick() {
		Dictionary<string, string> attribution = new Dictionary<string, string>();
		attribution.Add("some_key", "some_value");
		Adapty.updateAttribution(attribution, AttributionNetwork.Adjust, "user_id", this);
	}

	public void UpdateProfileButtonClick() {
		ProfileParameterBuilder profile = new ProfileParameterBuilder()
			.withBirthday("1970-01-01")
			.withEmail("some@gmail.com")
			.withFirstName("Somename")
			.withLastName("Somelast")
			.withGender(Gender.Male)
			.withPhoneNumber("+11234567890");
		Adapty.updateProfile(profile, this);
	}

	// Adapty Callbacks

	public void OnIdentify(AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		}
	}

	public void OnLogout(AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		}
	}

	public void OnGetPaywalls(PaywallModel[] paywalls, ProductModel[] products, DataState state, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			Debug.Log("State: " + state);
			foreach (PaywallModel paywall in paywalls) {
				Debug.Log("Paywall developerId: " + paywall.developerId + ", products: " + paywall.products.Length);
			}
			foreach (ProductModel product in products) {
				retrievedProducts.Add(product);
				Debug.Log("Product price: " + product.localizedPrice + ", vendorProductId: " + product.vendorProductId + ", skuId: " + product.skuId);
			}
		}
	}

	public void OnMakePurchase(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (purchaserInfo != null) {
				Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
			}
			Debug.Log("Receipt/PurchaseToken: " + receipt);
			if (product != null) {
				Debug.Log("Product price: " + product.localizedPrice + ", vendorProductId: " + product.vendorProductId + ", skuId: " + product.skuId);
			}
			if (retrievedReceipt == null) {
				retrievedReceipt = receipt;
			}
			if (validationResult != null) {
				foreach (KeyValuePair<string, object> item in validationResult) {
					Debug.Log("ValidationResult " + item.Key + ": " + item.Value);
				}
			}
		}
	}

	public void OnRestorePurchases(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object>[] validationResults, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (purchaserInfo != null) {
				Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
			}
			Debug.Log("Receipt/PurchaseToken: " + receipt);
			if (retrievedReceipt == null) {
				retrievedReceipt = receipt;
			}
			if (validationResults != null && validationResults.Length > 0) {
				foreach (KeyValuePair<string, object> item in validationResults[0]) {
					Debug.Log("ValidationResult " + item.Key + ": " + item.Value);
				}
			}
		}
	}

	public void OnSyncPurchases(AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		}
	}

	public void OnValidateAppleReceipt(PurchaserInfoModel purchaserInfo, Dictionary<string, object> validationResult, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (purchaserInfo != null) {
				Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
			}
			if (validationResult != null) {
				foreach (KeyValuePair<string, object> item in validationResult) {
					Debug.Log("ValidationResult " + item.Key + ": " + item.Value);
				}
			}
		}
	}

	public void OnValidateGooglePurchase(PurchaserInfoModel purchaserInfo, Dictionary<string, object> validationResult, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (purchaserInfo != null) {
				Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
			}
			foreach (KeyValuePair<string, object> item in validationResult) {
				Debug.Log("ValidationResult " + item.Key + ": " + item.Value);
			}
		}
	}

	public void OnGetPurchaserInfo(PurchaserInfoModel purchaserInfo, DataState state, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (purchaserInfo != null) {
				Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
			}
			Debug.Log("State: " + state);
		}
	}

	public void OnPurchaserInfoUpdated(PurchaserInfoModel purchaserInfo) {
		Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
	}

	public void OnUpdateAttribution(AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		}
	}

	public void OnUpdateProfile(AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		}
	}

	public void OnGetPromo(PromoModel promo, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			if (promo != null) {
				Debug.Log("Promo promoType:" + promo.promoType + ", expiresAt:" + promo.expiresAt);
			} else {
				Debug.Log("No promo");
			}
		}
	}

	public void OnPromoReceived(PromoModel promo) {
		Debug.Log("Promo promoType:" + promo.promoType + ", expiresAt:" + promo.expiresAt);
	}

	public void OnMakeDeferredPurchase(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error) {
		if (error != null) {
			Debug.Log("Error message: " + error.message + ", code: " + error.code);
		} else {
			Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId + ", accessLevels: " + purchaserInfo.accessLevels.Count + ", subscriptions: " + purchaserInfo.subscriptions.Count + ", nonSubscriptions: " + purchaserInfo.nonSubscriptions.Count);
		}
	}
}
