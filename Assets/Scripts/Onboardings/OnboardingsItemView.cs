using System;
using System.Collections;
using System.Collections.Generic;
using AdaptySDK;
using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class OnboardingsItemView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        [HideInInspector]
        public string PlacementId;

        [HideInInspector]
        public string PlacementLocale;

        public RectTransform LoadingTransform;

        public TextMeshProUGUI PlacementIdText;
        public TextMeshProUGUI RequestLocaleText;
        public TextMeshProUGUI StatusText;
        public RectTransform DetailsContainerTransform;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI AudienceNameText;
        public TextMeshProUGUI VariationIdText;
        public TextMeshProUGUI RemoteConfigText;
        public TextMeshProUGUI ErrorText;

        void Start()
        {
            this.SetLoading(false);
        }

        void Update()
        {
            this.PlacementIdText.SetText(this.PlacementId);
            this.RequestLocaleText.SetText(
                string.IsNullOrEmpty(this.PlacementLocale) ? "null" : this.PlacementLocale
            );
        }

        void SetLoading(bool loading)
        {
            this.LoadingTransform.gameObject.SetActive(loading);
        }

        private AdaptyOnboarding m_onboarding;

        public void LoadOnboarding(PlacementLoadStrategy loadStrategy, bool isDefaultAudience)
        {
            if (string.IsNullOrEmpty(this.PlacementId))
            {
                this.UpdateOnboardingError("OnboardingId is empty");
                this.SetLoading(false);
                return;
            }

            var placementLocale = string.IsNullOrEmpty(this.PlacementLocale)
                ? null
                : this.PlacementLocale;

            this.SetLoading(true);

            var fetchPolicy = loadStrategy.ToFetchPolicy();

            Action<AdaptyOnboarding, AdaptyError> onLoadOnboarding = (onboarding, error) =>
            {
                if (error != null)
                {
                    this.UpdateOnboardingError(error.Message);
                }
                else
                {
                    this.m_onboarding = onboarding;
                    this.UpdateOnboardingData(onboarding);
                }

                this.SetLoading(false);
            };

            if (isDefaultAudience)
            {
                Adapty.GetOnboardingForDefaultAudience(
                    this.PlacementId,
                    placementLocale,
                    fetchPolicy,
                    onLoadOnboarding
                );
            }
            else
            {
                Adapty.GetOnboarding(
                    this.PlacementId,
                    placementLocale,
                    fetchPolicy,
                    null,
                    onLoadOnboarding
                );
            }
        }

        public void PresentOnboardingPressed(bool fullScreen)
        {
            if (this.m_onboarding == null)
            {
                return;
            }

            this.Listener.CreateOnboardingView(
                this.m_onboarding,
                (view) =>
                {
                    view.Present(
                        fullScreen
                            ? AdaptyUIIOSPresentationStyle.FullScreen
                            : AdaptyUIIOSPresentationStyle.PageSheet,
                        (error) => { }
                    );
                }
            );
        }

        private void UpdateOnboardingData(AdaptyOnboarding onboarding)
        {
            this.StatusText.SetText("OK");
            this.StatusText.color = Color.green;

            this.DetailsContainerTransform.gameObject.SetActive(true);
            this.NameText.SetText(onboarding.Name);
            this.AudienceNameText.SetText(onboarding.Placement.AudienceName);
            this.VariationIdText.SetText(onboarding.VariationId);
            this.RemoteConfigText.SetText(onboarding.RemoteConfig?.Locale ?? "null");

            this.ErrorText.gameObject.SetActive(false);
        }

        private void UpdateOnboardingError(string error)
        {
            this.StatusText.SetText("FAIL");
            this.StatusText.color = Color.red;

            this.DetailsContainerTransform.gameObject.SetActive(false);
            this.ErrorText.gameObject.SetActive(true);

            this.ErrorText.SetText("Error: " + error);
        }
    }
}
