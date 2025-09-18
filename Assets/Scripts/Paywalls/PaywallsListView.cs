using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class PaywallsListView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        public TMP_InputField PlacementIdTextField;
        public TMP_InputField PlacementLocaleTextField;
        public RectTransform ContentViewTransform;

        public GameObject PaywallsItemPrefab;

        void Start()
        {
            this.AddPlacement("test_alexey", null);
        }

        void Update() { }

        public void AddPlacementPressed()
        {
            if (string.IsNullOrEmpty(this.PlacementIdTextField.text))
            {
                return;
            }

            var placementId = this.PlacementIdTextField.text;
            var placementLocale = this.PlacementLocaleTextField.text;

            this.AddPlacement(placementId, placementLocale);

            this.PlacementIdTextField.text = "";
            this.PlacementLocaleTextField.text = "";
        }

        private void AddPlacement(string placementId, string placementLocale)
        {
            var paywallItem = Instantiate(this.PaywallsItemPrefab, this.ContentViewTransform);
            var paywallItemView = paywallItem.GetComponent<PaywallsItemView>();

            paywallItemView.Listener = this.Listener;
            paywallItemView.PlacementId = placementId;
            paywallItemView.PlacementLocale = placementLocale;
            paywallItemView.LoadPaywall();
        }
    }
}
