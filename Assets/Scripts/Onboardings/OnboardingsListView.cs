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

        private PlacementLoadStrategy m_loadStrategy = PlacementLoadStrategy.LoadElseCache;

        void Start()
        {
            this.PlacementLocaleTextField.contentType = TMP_InputField.ContentType.Standard;
            this.PlacementLocaleTextField.inputType = TMP_InputField.InputType.Standard;
        }

        void Update() { }

        public void OnDropdownValueChanged(int value)
        {
            switch (value)
            {
                case 0:
                    this.m_loadStrategy = PlacementLoadStrategy.LoadElseCache;
                    break;
                case 1:
                    this.m_loadStrategy = PlacementLoadStrategy.CacheElseLoad;
                    break;
                case 2:
                    this.m_loadStrategy = PlacementLoadStrategy.CacheElseLoadIfExperied_10sec;
                    break;
                case 3:
                    this.m_loadStrategy = PlacementLoadStrategy.CacheElseLoadIfExperied_60sec;
                    break;
                case 4:
                    this.m_loadStrategy = PlacementLoadStrategy.CacheElseLoadIfExperied_600sec;
                    break;
                default:
                    break;
            }
        }

        public void AddPlacementPressed()
        {
            if (string.IsNullOrEmpty(this.PlacementIdTextField.text))
            {
                return;
            }

            var placementId = this.PlacementIdTextField.text;
            var placementLocale = this.PlacementLocaleTextField.text;

            this.AddPlacement(placementId, placementLocale, false);

            this.PlacementIdTextField.text = "";
            this.PlacementLocaleTextField.text = "";
        }

        public void AddPlacementDefaultAudiencePressed()
        {
            if (string.IsNullOrEmpty(this.PlacementIdTextField.text))
            {
                return;
            }

            var placementId = this.PlacementIdTextField.text;
            var placementLocale = this.PlacementLocaleTextField.text;

            this.AddPlacement(placementId, placementLocale, true);

            this.PlacementIdTextField.text = "";
            this.PlacementLocaleTextField.text = "";
        }

        private void AddPlacement(
            string placementId,
            string placementLocale,
            bool isDefaultAudience
        )
        {
            var onboardingItem = Instantiate(this.OnboardingItemPrefab, this.ContentViewTransform);
            var onboardingItemView = onboardingItem.GetComponent<OnboardingsItemView>();

            onboardingItemView.Listener = this.Listener;
            onboardingItemView.PlacementId = placementId;
            onboardingItemView.PlacementLocale = placementLocale;
            onboardingItemView.LoadOnboarding(this.m_loadStrategy, isDefaultAudience);
        }
    }
}
