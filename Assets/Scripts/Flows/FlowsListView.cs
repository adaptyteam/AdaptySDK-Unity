using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class FlowsListView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        public TMP_InputField PlacementIdTextField;
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
            if (string.IsNullOrEmpty(this.PlacementIdTextField.text))
            {
                return;
            }

            var placementId = this.PlacementIdTextField.text;

            this.AddPlacement(placementId, false);

            this.PlacementIdTextField.text = "";
        }

        public void AddPlacementDefaultAudiencePressed()
        {
            if (string.IsNullOrEmpty(this.PlacementIdTextField.text))
            {
                return;
            }

            var placementId = this.PlacementIdTextField.text;

            this.AddPlacement(placementId, true);

            this.PlacementIdTextField.text = "";
        }

        private void AddPlacement(string placementId, bool isDefaultAudience)
        {
            var flowItem = Instantiate(this.FlowsItemPrefab, this.ContentViewTransform);
            var flowItemView = flowItem.GetComponent<FlowsItemView>();

            flowItemView.Listener = this.Listener;
            flowItemView.PlacementId = placementId;
            flowItemView.LoadFlow(this.m_loadStrategy, isDefaultAudience);
        }
    }
}
