using UnityEngine;
using UnityEngine.UI;
using AdaptySDK;

namespace AdaptyExample {
	public class AdaptyListener : MonoBehaviour, AdaptyEventListener {

		public AdaptyRouter Router;
		public Text LoggerText;

		public void Log(string message) {
			Debug.Log("#AdaptyListener# " + message);
		}

		void Start() {
			this.Router = GetComponent<AdaptyRouter>();
			Adapty.SetEventListener(this);
		}

		public void Log(string message, bool clearLog = false) {
			Debug.Log($"#AdaptyListener# {message}");

			if (clearLog) {
				LoggerText.text = message;
			} else {
				LoggerText.text = $"{LoggerText.text}\n{message}";
			}
		}

		public void ClearLog() {
			LoggerText.text = "";
		}

		public void CopyLogToClipBoard() {
			GUIUtility.systemCopyBuffer = LoggerText.text;
		}

		// – AdaptyEventListener

		public void OnReceiveUpdatedPurchaserInfo(Adapty.PurchaserInfo purchaserInfo) {
			Debug.Log("#AdaptyListener# OnReceiveUpdatedPurchaserInfo called");

			this.Router.RootPanel.UpdatePurchaserInfo(purchaserInfo);
		}

		public void OnReceivePromo(Adapty.Promo promo) {
			Debug.Log("#AdaptyListener# OnReceivePromo called");

			this.Router.RootPanel.UpdatePromo(promo);
		}

		public void OnDeferredPurchasesProduct(Adapty.Product product) {
			Debug.Log("#AdaptyListener# OnDeferredPurchasesProduct called");
		}

		public void OnReceivePaywallsForConfig(Adapty.Paywall[] paywalls) {
			Debug.Log("#AdaptyListener# OnReceivePaywallsForConfig called");
		}
	}

}