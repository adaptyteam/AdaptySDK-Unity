using UnityEngine;

namespace AdaptyExample {
	public class AdaptyRouter : MonoBehaviour {
		public AdaptyRootPanelView RootPanel;
		public AdaptyProductsListView ProductsPanel;
		public GameObject LoadingView;

		void Start() {
			this.ShowRootView();
		}

		public void SetIsLoading(bool isLoading) {
			LoadingView.SetActive(isLoading);
		}

		public void ShowRootView() {
			RootPanel.gameObject.SetActive(true);
			ProductsPanel.gameObject.SetActive(false);
		}

		public void ShowProductsListView() {
			RootPanel.gameObject.SetActive(false);
			ProductsPanel.gameObject.SetActive(true);
		}
	}
}
