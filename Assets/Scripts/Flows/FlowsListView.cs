using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class FlowsListView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        public TMP_InputField PlacementIdTextField;

        /// <summary>
        /// Used when the placement field is left empty.
        /// </summary>
        /// <remarks>
        /// The iOS keyboard autocapitalises the first letter, so typing a placement id by hand on
        /// a device produces one that does not exist and a fetch failure that looks like an SDK
        /// problem. Leaving the field blank uses this instead.
        /// </remarks>
        public const string DefaultPlacementId = "rt.RegTestPaywall1";

        /// <summary>
        /// The localization the flow view is built with. A flow itself is not localized at fetch time,
        /// so this is passed to AdaptyUICreateFlowViewParameters, not to GetFlow.
        /// </summary>
        public TMP_InputField PlacementLocaleTextField;

        public RectTransform ContentViewTransform;

        public GameObject FlowsItemPrefab;

        private PlacementLoadStrategy m_loadStrategy = PlacementLoadStrategy.LoadElseCache;

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
                    this.m_loadStrategy = PlacementLoadStrategy.LoadElseCache;
                    break;
            }
        }

        public void AddPlacementPressed()
        {
            var placementId = string.IsNullOrEmpty(this.PlacementIdTextField.text)
                ? DefaultPlacementId
                : this.PlacementIdTextField.text;
            var placementLocale = this.PlacementLocaleTextField.text;

            this.AddPlacement(placementId, placementLocale, false);

            this.PlacementIdTextField.text = "";
            this.PlacementLocaleTextField.text = "";
        }

        public void AddPlacementDefaultAudiencePressed()
        {
            var placementId = string.IsNullOrEmpty(this.PlacementIdTextField.text)
                ? DefaultPlacementId
                : this.PlacementIdTextField.text;
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
            var flowItem = Instantiate(this.FlowsItemPrefab, this.ContentViewTransform);
            var flowItemView = flowItem.GetComponent<FlowsItemView>();

            flowItemView.Listener = this.Listener;
            flowItemView.PlacementId = placementId;
            flowItemView.PlacementLocale = placementLocale;
            flowItemView.LoadFlow(this.m_loadStrategy, isDefaultAudience);
        }
    }
}
