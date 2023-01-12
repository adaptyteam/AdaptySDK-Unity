using AdaptySDK;
using UnityEngine;

namespace AdaptyExample {
    public class AdaptyRouter : MonoBehaviour {
        public RectTransform LoadingPanel;

        [Header("Sections Prefabs")]
        public RectTransform ContentTransform;

        public GameObject ProfileIdSectionPrefab;

        public GameObject ProfileInfoSectionPrefab;

        public GameObject ExamplePaywallSectionPrefab;

        //[HideInInspector]
        public ProfileIdSection ProfileIdSection;

        //[HideInInspector]
        public ProfileInfoSection ProfileInfoSection;

        //[HideInInspector]
        public PaywallSection ExamplePaywallSection;

        private AdaptyListener listener;
        private Adapty.Profile profile;

        void Start() {
            // UpdateProfile(this.profile);
            this.listener = GetComponent<AdaptyListener>();
            this.ConfigureLayout();

            this.ProfileIdSection.ProfileIdText.SetText("profile_id will be here");
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

            var profileIdSection = profileIdSectionObj.GetComponent<ProfileIdSection>();
            var profileInfoSection = profileInfoSectionObj.GetComponent<ProfileInfoSection>();
            var examplePaywallSection = examplePaywallSectionObj.GetComponent<PaywallSection>();

            profileInfoSection.Listener = this.listener;
            examplePaywallSection.Listener = this.listener;

            this.ProfileIdSection = profileIdSection;
            this.ProfileInfoSection = profileInfoSection;
            this.ExamplePaywallSection = examplePaywallSection;
        }

        public void UpdateProfile(Adapty.Profile profile) {
            Debug.Log($"#AdaptyRouter# UpdateProfile");

            if (this.ProfileInfoSection != null && profile != null) {
                this.ProfileInfoSection.UpdateProfile(profile);
            }
            if (this.ProfileIdSection != null && profile != null) {
                this.ProfileIdSection.UpdateProfile(profile);
            }

            this.profile = profile;
        }

        public void SetIsLoading(bool isLoading) {
            this.LoadingPanel.gameObject.SetActive(isLoading);
        }

        //public void ShowRootView() {
        //	RootPanel.gameObject.SetActive(true);
        //	ProductsPanel.gameObject.SetActive(false);
        //}

        //public void ShowProductsListView() {
        //	RootPanel.gameObject.SetActive(false);
        //	ProductsPanel.gameObject.SetActive(true);
        //}
    }
}
