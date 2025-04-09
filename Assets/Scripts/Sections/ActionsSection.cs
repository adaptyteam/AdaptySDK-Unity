using System;
using System.Collections;
using System.Collections.Generic;
using AdaptyExample;
using TMPro;
using UnityEngine;

public class ActionsSection : MonoBehaviour {
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public TextMeshProUGUI SDKVersionText;

    public void Start() {
        // TODO: 
        SDKVersionText.SetText("SDK version: " + AdaptySDK.Adapty.SDKVersion);
    }

    public void RestorePurchasesPressed() {
        this.Router.SetIsLoading(true);
        this.Listener.RestorePurchases((error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void UpdateAppStoreCollectingRefundDataConsent(Boolean value) {
        this.Router.SetIsLoading(true);
        this.Listener.UpdateAppStoreCollectingRefundDataConsent(value, (error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void UpdateAppStoreRefundPreference(int value) {
        this.Router.SetIsLoading(true);
        this.Listener.UpdateAppStoreRefundPreference(value, (error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void UpdateProfilePressed() {
        this.Router.SetIsLoading(true);
        this.Listener.UpdateProfile((error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void SetIntegrationIdentifierPressed() {
        this.Router.SetIsLoading(true);
        this.Listener.SetIntegrationIdentifier((error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void UpdateAttributionPressed() {
        this.Router.SetIsLoading(true);
        this.Listener.UpdateAttribution((error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void SendOnboardingPressed(int value) {
        this.Router.SetIsLoading(true);
        this.Listener.LogShowOnboarding(value, (error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void PresentCodeRedemptionSheetPressed() {
        this.Listener.PresentCodeRedemptionSheet();
    }

    public void LogoutPressed() {
        this.Router.SetIsLoading(true);
        this.Listener.Logout((error) => {
            this.Router.SetIsLoading(false);
        });
    }
}
