using System;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;

public class ProfileInfoSection : MonoBehaviour {
    public AdaptyListener Listener;

    public TextMeshProUGUI IsPremiumText;
    public TextMeshProUGUI IsLifetimeText;
    public TextMeshProUGUI ActivatedAtText;
    public TextMeshProUGUI RenewedAtText;
    public TextMeshProUGUI ExpiresAtText;
    public TextMeshProUGUI WillRenewText;
    public TextMeshProUGUI UnsubscribedAtText;
    public TextMeshProUGUI BillingIssueAtText;
    public TextMeshProUGUI CancellationReasonText;
    public TextMeshProUGUI SubscriptionsText;
    public TextMeshProUGUI NonSubscriptionsText;


    public void SetProfile(AdaptyProfile profile) {
        if (profile.AccessLevels == null || profile.AccessLevels.Count == 0) {
            Debug.Log($"#ProfileSection# UpdateProfile null");

            SetBoolValue(IsPremiumText, false);
            SetNullValue(IsLifetimeText);
            SetNullValue(ActivatedAtText);
            SetNullValue(RenewedAtText);
            SetNullValue(ExpiresAtText);
            SetNullValue(WillRenewText);
            SetNullValue(UnsubscribedAtText);
            SetNullValue(BillingIssueAtText);
            SetNullValue(CancellationReasonText);
            SetIntegerValue(SubscriptionsText, profile.Subscriptions.Count);
            SetIntegerValue(NonSubscriptionsText, profile.NonSubscriptions.Count);

            return;
        }

        var premium = profile.AccessLevels["premium"];

        Debug.Log($"#ProfileSection# UpdateProfile not null");

        SetBoolValue(IsPremiumText, premium.IsActive);
        SetBoolValue(IsLifetimeText, premium.IsLifetime);
        SetDateValue(ActivatedAtText, premium.ActivatedAt);
        SetDateValue(RenewedAtText, premium.RenewedAt);
        SetDateValue(ExpiresAtText, premium.ExpiresAt);
        SetBoolValue(WillRenewText, premium.WillRenew);
        SetDateValue(UnsubscribedAtText, premium.UnsubscribedAt);
        SetDateValue(BillingIssueAtText, premium.BillingIssueDetectedAt);
        SetStringValue(CancellationReasonText, premium.CancellationReason);
        SetIntegerValue(SubscriptionsText, profile.Subscriptions.Count);
        SetIntegerValue(NonSubscriptionsText, profile.NonSubscriptions.Count);
    }

    private void SetNullValue(TextMeshProUGUI text) {
        text.SetText("null");
    }

    private void SetBoolValue(TextMeshProUGUI text, bool value) {
        text.SetText(value ? "true" : "false");
    }

    private void SetDateValue(TextMeshProUGUI text, DateTime? value) {
        text.SetText(value?.ToShortDateString() ?? "null");
    }

    private void SetStringValue(TextMeshProUGUI text, string value) {
        text.SetText(value);
    }

    private void SetIntegerValue(TextMeshProUGUI text, int value) {
        text.SetText(value.ToString());
    }

    public void GetProfile() {
        this.Listener.GetProfile();
    }
}
