using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class OnboardingsListView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        public TMP_InputField PlacementIdTextField;
        public TMP_InputField PlacementLocaleTextField;
        public RectTransform ContentViewTransform;

        public GameObject OnboardingItemPrefab;

        void Start()
        {
            this.AddPlacement("onb_test_alexey", null);
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
            var onboardingItem = Instantiate(this.OnboardingItemPrefab, this.ContentViewTransform);
            var onboardingItemView = onboardingItem.GetComponent<OnboardingsItemView>();

            onboardingItemView.PlacementId = placementId;
            onboardingItemView.PlacementLocale = placementLocale;
            onboardingItemView.LoadOnboarding();
        }
    }
}
