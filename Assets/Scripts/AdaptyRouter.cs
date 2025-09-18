using System.Collections;
using AdaptySDK;
using UnityEngine;

namespace AdaptyExample
{
    public class AdaptyRouter : MonoBehaviour
    {
        public RectTransform LoadingPanel;
        public AlertPanel AlertPanel;

        public RectTransform MainContentTransform;
        public RectTransform MainContentScrollRect;
        public PaywallsListView PaywallsListView;
        public OnboardingsListView OnboardingsListView;

        [Header("Sections Prefabs")]
        public GameObject ProfileIdSectionPrefab;
        public GameObject IdentifySectionPrefab;
        public GameObject ProfileInfoSectionPrefab;
        public GameObject ExamplePaywallSectionPrefab;
        public GameObject CustomPaywallSectionPrefab;
        public GameObject OtherActionsSectionPrefab;
        public GameObject VisualPaywallSectionPrefab;

        [HideInInspector]
        public ProfileIdSection ProfileIdSection;

        [HideInInspector]
        public IdentifySection IdentifySection;

        [HideInInspector]
        public ProfileInfoSection ProfileInfoSection;

        [HideInInspector]
        public PaywallSection ExamplePaywallSection;

        [HideInInspector]
        public CustomPaywallSection CustomPaywallSection;

        [HideInInspector]
        public ActionsSection ActionsSection;

        private AdaptyListener listener;
        private AdaptyProfile profile;

        void Start()
        {
            this.listener = this.GetComponent<AdaptyListener>();

            this.PaywallsListView.Listener = this.listener;
            this.OnboardingsListView.Listener = this.listener;

            this.ConfigureLayout();
            this.SelectActiveTab(0);
        }

        public void SelectActiveTab(int tabIndex)
        {
            this.MainContentScrollRect.gameObject.SetActive(tabIndex == 0);
            this.PaywallsListView.gameObject.SetActive(tabIndex == 1);
            this.OnboardingsListView.gameObject.SetActive(tabIndex == 2);
        }

        void ConfigureLayout()
        {
            // Ensure MainContentTransform uses Unity's layout system
            var content = this.MainContentTransform;
            // var vertical = content.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
            // if (vertical == null)
            // {
            //     vertical = content.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            //     vertical.childControlHeight = true;
            //     vertical.childControlWidth = true;
            //     vertical.childForceExpandHeight = false;
            //     vertical.childForceExpandWidth = true;
            //     vertical.spacing = 64.0f;
            //     vertical.padding = new RectOffset(0, 0, 20, 0);
            // }

            // var fitter = content.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            // if (fitter == null)
            // {
            //     fitter = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            // }
            // fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            // Helper to parent and stretch a section under the content
            System.Action<RectTransform> AttachSection = (RectTransform sectionRect) =>
            {
                sectionRect.SetParent(this.MainContentTransform, false);
                sectionRect.anchorMin = new Vector2(0, 1);
                sectionRect.anchorMax = new Vector2(1, 1);
                sectionRect.offsetMin = new Vector2(0, sectionRect.offsetMin.y);
                sectionRect.offsetMax = new Vector2(0, sectionRect.offsetMax.y);
                sectionRect.localScale = Vector3.one;

                var sectionLayout = sectionRect.GetComponent<UnityEngine.UI.LayoutElement>();
                if (sectionLayout == null)
                {
                    sectionLayout =
                        sectionRect.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
                }
                sectionLayout.minWidth = 0;
                sectionLayout.flexibleWidth = 1;
            };

            var profileIdSectionObj = Instantiate(this.ProfileIdSectionPrefab);
            var profileIdSectionRect = profileIdSectionObj.GetComponent<RectTransform>();
            AttachSection(profileIdSectionRect);

            var identifySectionObj = Instantiate(this.IdentifySectionPrefab);
            var identifySectionRect = identifySectionObj.GetComponent<RectTransform>();
            AttachSection(identifySectionRect);

            var profileInfoSectionObj = Instantiate(this.ProfileInfoSectionPrefab);
            var profileInfoSectionRect = profileInfoSectionObj.GetComponent<RectTransform>();
            AttachSection(profileInfoSectionRect);

            var examplePaywallSectionObj = Instantiate(this.ExamplePaywallSectionPrefab);
            var examplePaywallSectionRect = examplePaywallSectionObj.GetComponent<RectTransform>();
            AttachSection(examplePaywallSectionRect);

            var customPaywallSectionObj = Instantiate(this.CustomPaywallSectionPrefab);
            var customPaywallSectionRect = customPaywallSectionObj.GetComponent<RectTransform>();
            AttachSection(customPaywallSectionRect);

            var actionsSectionObj = Instantiate(this.OtherActionsSectionPrefab);
            var actionsSectionRect = actionsSectionObj.GetComponent<RectTransform>();
            AttachSection(actionsSectionRect);

            var profileIdSection = profileIdSectionObj.GetComponent<ProfileIdSection>();
            var identifySection = identifySectionObj.GetComponent<IdentifySection>();
            var profileInfoSection = profileInfoSectionObj.GetComponent<ProfileInfoSection>();
            var examplePaywallSection = examplePaywallSectionObj.GetComponent<PaywallSection>();
            var customPaywallSection = customPaywallSectionObj.GetComponent<CustomPaywallSection>();
            var actionsSection = actionsSectionObj.GetComponent<ActionsSection>();

            identifySection.Listener = this.listener;
            identifySection.Router = this;
            profileInfoSection.Listener = this.listener;
            examplePaywallSection.Listener = this.listener;
            examplePaywallSection.Router = this;
            customPaywallSection.Listener = this.listener;
            customPaywallSection.Router = this;
            actionsSection.Listener = this.listener;
            actionsSection.Router = this;

            this.IdentifySection = identifySection;
            this.ProfileIdSection = profileIdSection;
            this.ProfileInfoSection = profileInfoSection;
            this.ExamplePaywallSection = examplePaywallSection;
            this.CustomPaywallSection = customPaywallSection;
            this.ActionsSection = actionsSection;

            // Rebuild to apply layout immediately
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.MainContentTransform);
        }

        public void SetProfile(AdaptyProfile profile)
        {
            if (this.ProfileInfoSection != null && profile != null)
            {
                this.ProfileInfoSection.SetProfile(profile);
            }

            if (this.ProfileIdSection != null && profile != null)
            {
                this.ProfileIdSection.SetProfile(profile);
            }

            if (this.IdentifySection != null && profile != null)
            {
                this.IdentifySection.SetProfile(profile);
            }

            this.profile = profile;
        }

        public void SetIsLoading(bool isLoading)
        {
            this.LoadingPanel.gameObject.SetActive(isLoading);
        }

        public void ShowAlertPanel(string text)
        {
            StartCoroutine(DelayedShowAlertPanel(text));
        }

        private IEnumerator DelayedShowAlertPanel(string text)
        {
            yield return new WaitForEndOfFrame();
            this.AlertPanel.Text.SetText(text);
            this.AlertPanel.gameObject.SetActive(true);
        }

        public void HideAlertPanel()
        {
            StartCoroutine(DelayedHideAlertPanel());
        }

        private IEnumerator DelayedHideAlertPanel()
        {
            yield return new WaitForEndOfFrame();
            this.AlertPanel.gameObject.SetActive(false);
        }
    }
}
