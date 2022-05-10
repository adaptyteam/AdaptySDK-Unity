using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using AdaptySDK;

namespace AdaptyExample {

	public class AdaptyProductsListView : MonoBehaviour {
		public AdaptyRouter Router;

		public GameObject ProductsListHeaderPrefab;
		public GameObject ProductItemPrefab;

		public AdaptyNavigationView NavigationView;
		public RectTransform ScrollViewContent;
		public Text StateMessageText;

		private Adapty.Paywall[] _paywalls = null;
		private Adapty.Product[] _products = null;

		void Start() {
			NavigationView.Configure("Products", showBackButton: true);
		}

		void SetIsLoading(bool isLoading) {
			this.Router.SetIsLoading(isLoading);
		}

		public void LoadProductsList() {
			SetIsLoading(true);

			LayoutCleanUp();
			LayoutState("");

			Adapty.GetPaywalls(true, (response, error) => {
				this.SetIsLoading(false);

				if (error != null || response == null) {
					this.LayoutState(string.Format("Error: {0}", error));
					return;
				}

				this._paywalls = response.Paywalls;
				this._products = response.Products;

				this.LayoutPaywallsAndProducts();
			});
		}


		public void MakePurchaseButtonClick(Adapty.Product product) {
			SetIsLoading(true);
			Adapty.MakePurchase(product.VendorProductId, null, null, null, (result, error) => {
				this.SetIsLoading(false);
			});
		}

		void LayoutCleanUp() {
			foreach (Transform t in ScrollViewContent) {
				if (t.gameObject != ScrollViewContent.gameObject) {
					GameObject.Destroy(t.gameObject);
				}
			}
		}

		void LayoutState(string message) {
			StateMessageText.text = message;
			StateMessageText.gameObject.SetActive(true);
			ScrollViewContent.gameObject.SetActive(false);
		}

		float LayoutListHeader(string title, float offset) {
			var headerObject = GameObject.Instantiate(ProductsListHeaderPrefab);
			var headerText = headerObject.GetComponent<Text>();
			headerText.text = title;

			var rt = headerObject.GetComponent<RectTransform>();

			rt.SetParent(ScrollViewContent);
			rt.anchoredPosition = new Vector2(0.0f, offset);
			rt.sizeDelta = new Vector2(Mathf.Max(ScrollViewContent.sizeDelta.x - 40.0f, 100.0f), 70.0f);

			return -70.0f;
		}

		void LayoutPaywallsAndProducts() {
			LayoutCleanUp();

			var offset = -30.0f;

			if (_paywalls.Length > 0) {
				offset += LayoutListHeader("Paywalls", offset);
			}

			foreach (Adapty.Paywall paywall in _paywalls) {
				var productItemObject = GameObject.Instantiate(ProductItemPrefab);
				var productItem = productItemObject.GetComponent<AdaptyProductItem>();
				productItem.ConfigurePaywall(paywall);

				var rt = productItemObject.GetComponent<RectTransform>();

				rt.SetParent(ScrollViewContent);
				rt.anchoredPosition = new Vector2(0.0f, offset);
				rt.sizeDelta = new Vector2(ScrollViewContent.sizeDelta.x - 40.0f, 240.0f);

				offset -= 240.0f + 20.0f;
			}

			if (_products.Length > 0) {
				offset += LayoutListHeader("Products", offset);
			}

			foreach (Adapty.Product product in _products) {
				var productItemObject = GameObject.Instantiate(ProductItemPrefab);
				var productItem = productItemObject.GetComponent<AdaptyProductItem>();
				productItem.ConfigureProduct(product);

				var button = productItem.GetComponent<Button>();
				button.onClick.AddListener(delegate { MakePurchaseButtonClick(product); });

				var rt = productItemObject.GetComponent<RectTransform>();

				rt.SetParent(ScrollViewContent);
				rt.anchoredPosition = new Vector2(0.0f, offset);
				rt.sizeDelta = new Vector2(ScrollViewContent.sizeDelta.x - 40.0f, 240.0f);

				offset -= 240.0f + 20.0f;
			}

			ScrollViewContent.sizeDelta = new Vector2(ScrollViewContent.sizeDelta.x, -offset + 30.0f);

			StateMessageText.gameObject.SetActive(false);
			ScrollViewContent.gameObject.SetActive(true);
		}
	}
}