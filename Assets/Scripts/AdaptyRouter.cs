using System.Collections;
using AdaptySDK;
using UnityEngine;
using UnityEngine.Profiling;

namespace AdaptyExample
{
    public class AdaptyRouter : MonoBehaviour
    {
        public RectTransform LoadingPanel;

        [Header("Sections Prefabs")]
        public RectTransform ContentTransform;

        public GameObject ProfileIdSectionPrefab;

        public GameObject ProfileInfoSectionPrefab;

        public GameObject ExamplePaywallSectionPrefab;

        [HideInInspector]
        public ProfileIdSection ProfileIdSection;

        [HideInInspector]
        public ProfileInfoSection ProfileInfoSection;

        private Adapty.Profile profile;

        void Start()
        {
            // UpdateProfile(this.profile);
            this.ConfigureLayout();
        }

        void ConfigureLayout()
        {
            var offset = 20.0f;

            var profileIdSectionObj = Instantiate(this.ProfileIdSectionPrefab);
            var profileIdSectionRect =
                profileIdSectionObj.GetComponent<RectTransform>();
            profileIdSectionRect.SetParent(this.ContentTransform);
            profileIdSectionRect.anchoredPosition =
                new Vector3(profileIdSectionRect.position.x, -offset);

            this.ProfileIdSection =
                profileIdSectionObj.GetComponent<ProfileIdSection>();

            offset += profileIdSectionRect.rect.height + 20.0f;

            var profileInfoSectionObj =
                Instantiate(this.ProfileInfoSectionPrefab);
            var profileInfoSectionRect =
                profileInfoSectionObj.GetComponent<RectTransform>();
            profileInfoSectionRect.SetParent(this.ContentTransform);
            profileInfoSectionRect.anchoredPosition =
                new Vector3(profileInfoSectionRect.position.x, -offset);

            this.ProfileInfoSection =
                profileInfoSectionObj.GetComponent<ProfileInfoSection>();

            offset += profileInfoSectionRect.rect.height + 20.0f;

            var examplePaywallSectionObj =
                Instantiate(this.ExamplePaywallSectionPrefab);
            var examplePaywallSectionRect =
                examplePaywallSectionObj.GetComponent<RectTransform>();
            examplePaywallSectionRect.SetParent(this.ContentTransform);
            examplePaywallSectionRect.anchoredPosition =
                new Vector3(examplePaywallSectionRect.position.x, -offset);
        }

        public void UpdateProfile(Adapty.Profile profile)
        {
            if (this.ProfileInfoSection != null && profile != null)
            {
                this.ProfileInfoSection.UpdateProfile(profile);
            }
            if (this.ProfileIdSection != null && profile != null)
            {
                this.ProfileIdSection.UpdateProfile(profile);
            }

            this.profile = profile;
        }

        public void SetIsLoading(bool isLoading)
        {
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
