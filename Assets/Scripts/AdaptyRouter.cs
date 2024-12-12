using AdaptySDK;
using UnityEngine;

namespace AdaptyExample {
    public class AdaptyRouter : MonoBehaviour {
        public RectTransform LoadingPanel;
        public AlertPanel AlertPanel;

        [Header("Sections Prefabs")]
        public RectTransform ContentTransform;

        public GameObject ProfileIdSectionPrefab;
        public GameObject IdentifySectionPrefab;
        public GameObject ProfileInfoSectionPrefab;
        public GameObject ExamplePaywallSectionPrefab;
        public GameObject CustomPaywallSectionPrefab;
        public GameObject OtherActionsSectionPrefab;

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

        void Start() {
            this.listener = GetComponent<AdaptyListener>();
            this.ConfigureLayout();
        }

        void ConfigureLayout() {
            var offset = 20.0f;

            var profileIdSectionObj = Instantiate(this.ProfileIdSectionPrefab);
            var profileIdSectionRect =
                profileIdSectionObj.GetComponent<RectTransform>();
            profileIdSectionRect.SetParent(this.ContentTransform);
            profileIdSectionRect.anchoredPosition =
                new Vector3(profileIdSectionRect.position.x, -offset);

            offset += profileIdSectionRect.rect.height + 20.0f;

            var identifySectionObj = Instantiate(this.IdentifySectionPrefab);
            var identifySectionRect =
                identifySectionObj.GetComponent<RectTransform>();
            identifySectionRect.SetParent(this.ContentTransform);
            identifySectionRect.anchoredPosition =
                new Vector3(identifySectionRect.position.x, -offset);

            offset += identifySectionRect.rect.height + 20.0f;

            var profileInfoSectionObj =
                Instantiate(this.ProfileInfoSectionPrefab);
            var profileInfoSectionRect =
                profileInfoSectionObj.GetComponent<RectTransform>();
            profileInfoSectionRect.SetParent(this.ContentTransform);
            profileInfoSectionRect.anchoredPosition =
                new Vector3(profileInfoSectionRect.position.x, -offset);

            offset += profileInfoSectionRect.rect.height + 20.0f;

            var examplePaywallSectionObj =
                Instantiate(this.ExamplePaywallSectionPrefab);
            var examplePaywallSectionRect =
                examplePaywallSectionObj.GetComponent<RectTransform>();
            examplePaywallSectionRect.SetParent(this.ContentTransform);
            examplePaywallSectionRect.anchoredPosition =
                new Vector3(examplePaywallSectionRect.position.x, -offset);

            offset += examplePaywallSectionRect.rect.height + 20.0f;

            var customPaywallSectionObj = Instantiate(this.CustomPaywallSectionPrefab);
            var customPaywallSectionRect = customPaywallSectionObj.GetComponent<RectTransform>();
            customPaywallSectionRect.SetParent(this.ContentTransform);
            customPaywallSectionRect.anchoredPosition = new Vector3(customPaywallSectionRect.position.x, -offset);

            offset += customPaywallSectionRect.rect.height + 20.0f;

            var actionsSectionObj = Instantiate(this.OtherActionsSectionPrefab);
            var actionsSectionRect = actionsSectionObj.GetComponent<RectTransform>();
            actionsSectionRect.SetParent(this.ContentTransform);
            actionsSectionRect.anchoredPosition = new Vector3(actionsSectionRect.position.x, -offset);

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
        }

        public void SetProfile(AdaptyProfile profile) {
            if (this.ProfileInfoSection != null && profile != null) {
                this.ProfileInfoSection.SetProfile(profile);
            }
            if (this.ProfileIdSection != null && profile != null) {
                this.ProfileIdSection.SetProfile(profile);
            }

            if (this.IdentifySection != null && profile != null) {
                this.IdentifySection.SetProfile(profile);
            }

            this.profile = profile;
        }

        public void SetIsLoading(bool isLoading) {
            this.LoadingPanel.gameObject.SetActive(isLoading);
        }

        public void ShowAlertPanel(string text) {
            this.AlertPanel.Text.SetText(text);
            this.AlertPanel.gameObject.SetActive(true);
        }

        public void HideAlertPanel() {
            this.AlertPanel.gameObject.SetActive(false);
        }
    }
}
