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

        [Header("Sections Scripts")]
        public ProfileIdSection ProfileIdSection;
        public ProfileInfoSection ProfileInfoSection;

        public IdentifySection IdentifySection;

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
