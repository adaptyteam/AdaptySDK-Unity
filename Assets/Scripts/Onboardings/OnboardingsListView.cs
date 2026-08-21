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

        public const string DefaultPlacementId = "4681-onboarding-animate";

        private PlacementLoadStrategy m_loadStrategy = PlacementLoadStrategy.LoadElseCache;

        public void OnDropdownValueChanged(int value)
        {
            switch (value)
            {
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
                    this.m_loadStrategy = PlacementLoadStrategy.LoadElseCache;
                    break;
            }
        }

        public void AddPlacementPressed()
        {
            this.AddPlacement(false);
        }

        public void AddPlacementDefaultAudiencePressed()
        {
            this.AddPlacement(true);
        }

        private void AddPlacement(bool isDefaultAudience)
        {
            var placementId = string.IsNullOrEmpty(this.PlacementIdTextField.text)
                ? DefaultPlacementId
                : this.PlacementIdTextField.text;
            var placementLocale = this.PlacementLocaleTextField.text;

            var item = Instantiate(this.OnboardingItemPrefab, this.ContentViewTransform);
            var itemView = item.GetComponent<OnboardingsItemView>();

            itemView.Listener = this.Listener;
            itemView.PlacementId = placementId;
            itemView.PlacementLocale = placementLocale;
            itemView.LoadOnboarding(this.m_loadStrategy, isDefaultAudience);

            this.PlacementIdTextField.text = "";
            this.PlacementLocaleTextField.text = "";
        }
    }
}
