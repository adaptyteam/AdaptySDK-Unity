using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using AdaptySDK;

namespace AdaptyExample {
	public class AdaptyRootPanelView : MonoBehaviour {
		public AdaptyRouter Router;
		public AdaptyListener Manager;

		public AdaptyNavigationView NavigationView;
		public AdaptyTextInfoView PurchaserInfoView;
		public AdaptyTextInfoView PromoInfoView;

		void Start() {
			this.NavigationView.Configure("Welcome to Adapty!", showBackButton: false);
			this.PromoInfoView.ConfigurePromo(null);
			this.GetPurchaserInfoButtonClick();
		}

		void Update() {
			this.IDFACollectionButtonText.text = string.Format("Set IDFA Collection {0}", _idfaCollectionEnabled ? "disabled" : "enabled");

			this.ExternalAnalyticsButtonText.text = string.Format("Set External Analytics {0}", _externalAnalyticsEnabled ? "disabled" : "enabled");
		}

		#region Actions

		public void GetPurchaserInfoButtonClick() {
			this.Manager.Log("GetPurchaserInfo -->", clearLog: true);

			Adapty.GetPurchaserInfo(true, (purchaserInfo, error) => {
				if (error != null) {
					this.PurchaserInfoView.Configure($"GetPurchaserInfo Error:\n{error}");
					this.Manager.Log($"GetPurchaserInfo <-- Error: {error}", clearLog: false);
					return;
				}

				this.UpdatePurchaserInfo(purchaserInfo);
				this.Manager.Log($"GetPurchaserInfo <-- {purchaserInfo}", clearLog: false);
			});
		}

		public void GetPaywallsButtonClick() {
			this.Router.ShowProductsListView();
			this.Router.ProductsPanel.LoadProductsList();
		}

		public void RestorePurchasesButtonClick() {
			this.Manager.Log("RestorePurchases -->", clearLog: true);

			Adapty.RestorePurchases((response, error) => {
				if (error != null) {
					this.PurchaserInfoView.Configure($"RestorePurchases Error:\n{error}");
					this.Manager.Log($"RestorePurchases <-- Error: {error}", clearLog: false);
					return;
				}

				if (response != null) {
					this.UpdatePurchaserInfo(response.PurchaserInfo);
				}
				this.Manager.Log($"RestorePurchases <-- {response}", clearLog: false);
			});
		}

		public void GetPromoButtonClick() {
			this.Manager.Log("GetPromo -->", clearLog: true);

			Adapty.GetPromo((promo, error) => {
				if (error != null) {
					this.PromoInfoView.Configure($"GetPromo Error:\n{error}");
					this.Manager.Log($"GetPromo <-- Error: {error}", clearLog: false);
					return;
				}

				this.UpdatePromo(promo);
				this.Manager.Log($"GetPromo <-- {promo}", clearLog: false);
			});
		}

		public void LogoutButtonClick() {
			this.Manager.Log("Logout -->", clearLog: true);

			Adapty.Logout((error) => {
				if (error != null) {
					this.Manager.Log($"Logout <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"Logout <-- Success!", clearLog: false);
			});
		}

		#endregion

		public void UpdatePurchaserInfo(Adapty.PurchaserInfo purchaserInfo) {
			PurchaserInfoView.ConfigurePurchaserInfo(purchaserInfo);
		}

		public void UpdatePromo(Adapty.Promo promo) {
			PromoInfoView.ConfigurePromo(promo);
		}

		private bool _idfaCollectionEnabled = true;
		public Text IDFACollectionButtonText;

		public void ToggleIDFACollectionClicked() {
#if UNITY_IOS && !UNITY_EDITOR
			_idfaCollectionEnabled = !_idfaCollectionEnabled;
			Adapty.SetIdfaCollectionDisabled(!_idfaCollectionEnabled);
#endif
		}

		public void GetLogLevelClicked() {
			this.Manager.Log("GetLogLevel -->", clearLog: true);
			var logLevel = Adapty.GetLogLevel();
			this.Manager.Log($"GetPurchaserInfo <-- {logLevel}", clearLog: false);
		}

		public void SetLogLevelClicked() {
			this.Manager.Log("SetLogLevel -->", clearLog: true);
			Adapty.SetLogLevel(Adapty.LogLevel.Verbose);
		}

		public void IdentifyClicked() {
			this.Manager.Log("Identify -->", clearLog: true);

			Adapty.Identify("test_user_id", (error) => {
				if (error != null) {
					this.Manager.Log($"Identify <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"Identify <-- Success!", clearLog: false);
			});
		}

		public void SetFallbackPaywallsClicked() {
			this.Manager.Log("SetFallbackPaywalls -->", clearLog: true);

			Adapty.SetFallbackPaywalls("{\"test_key\": \"test_value\"}", (error) => {
				if (error != null) {
					this.Manager.Log($"SetFallbackPaywalls <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"SetFallbackPaywalls <-- Success!", clearLog: false);
			});
		}

		public void MakeDeferredPurchaseClicked() {
#if UNITY_IOS && !UNITY_EDITOR
			this.Manager.Log("MakeDeferredPurchase -->", clearLog: true);

			Adapty.MakeDeferredPurchase("test_product_id", (result, error) => {
				if (error != null) {
					this.Manager.Log($"MakeDeferredPurchase <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"MakeDeferredPurchase <-- Success!", clearLog: false);
			});
#endif
		}

		public void UpdateProfileClicked() {
			var profileBuilder = new Adapty.ProfileParameterBuilder();
			profileBuilder.FirstName = "TestFirstName";
			profileBuilder.LastName = "TestLastName";
			profileBuilder.SetBirthday(1984, 11, 11);

			this.Manager.Log("UpdateProfile -->", clearLog: true);

			Adapty.UpdateProfile(profileBuilder, (error) => {
				if (error != null) {
					this.Manager.Log($"UpdateProfile <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"UpdateProfile <-- Success!", clearLog: false);
			});
		}

		public void UpdateAttributionClicked() {
			this.Manager.Log("UpdateAttribution -->", clearLog: true);

			Adapty.UpdateAttribution("{\"test_key\": \"test_value\"}", Adapty.AttributionNetwork.Custom, "test_user_id", (error) => {
				if (error != null) {
					this.Manager.Log($"UpdateAttribution <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"UpdateAttribution <-- Success!", clearLog: false);
			});
		}

		private bool _externalAnalyticsEnabled = true;
		public Text ExternalAnalyticsButtonText;

		public void ToggleExternalAnalyticsClicked() {
			_externalAnalyticsEnabled = !_externalAnalyticsEnabled;

			this.Manager.Log("SetExternalAnalyticsEnabled -->", clearLog: true);

			Adapty.SetExternalAnalyticsEnabled(!_externalAnalyticsEnabled, (error) => {
				if (error != null) {
					this.Manager.Log($"SetExternalAnalyticsEnabled <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"SetExternalAnalyticsEnabled <-- Success!", clearLog: false);
			});
		}

		public void SetVariationForTransactionClicked() {
			this.Manager.Log("SetVariationForTransaction -->", clearLog: true);

			Adapty.SetVariationForTransaction("test_variation_id", "test_transaction_id", (error) => {
				if (error != null) {
					this.Manager.Log($"SetVariationForTransaction <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"SetVariationForTransaction <-- Success!", clearLog: false);
			});
		}

		public void GetApnsToken() {
#if UNITY_IOS && !UNITY_EDITOR
			this.Manager.Log("GetApnsToken -->", clearLog: true);
			var token = Adapty.GetApnsToken();
			this.Manager.Log($"GetApnsToken <-- {token}", clearLog: false);
#endif
		}

		public void SetApnsToken() {
#if UNITY_IOS && !UNITY_EDITOR
			this.Manager.Log("SetApnsToken -->", clearLog: true);
			Adapty.SetApnsToken("test_apns_token");
#endif
		}
		public void HandlePushNotification() {
#if UNITY_IOS && !UNITY_EDITOR

			this.Manager.Log("HandlePushNotification -->", clearLog: true);

			Adapty.HandlePushNotification("{\"test_key\": \"test_value\"}", (error) => {
				if (error != null) {
					this.Manager.Log($"HandlePushNotification <-- Error: {error}", clearLog: false);
					return;
				}

				this.Manager.Log($"HandlePushNotification <-- Success!", clearLog: false);
			});
#endif
		}
		public void PresentCodeRedemptionSheet() {
#if UNITY_IOS && !UNITY_EDITOR
			this.Manager.Log("PresentCodeRedemptionSheet -->", clearLog: true);
			Adapty.PresentCodeRedemptionSheet();
#endif
		}

		public void NewPushTokenClicked() {
#if UNITY_ANDROID && !UNITY_EDITOR
			this.Manager.Log("NewPushToken -->", clearLog: true);
			Adapty.NewPushToken("token_string");
#endif
		}

		public void PushReceivedClicked() {
#if UNITY_ANDROID && !UNITY_EDITOR
			this.Manager.Log("PushReceived -->", clearLog: true);
			Adapty.PushReceived("{\"test_key\": \"test_value\"}");
#endif
		}
	}

}