using System;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;

public class InstallationDetailsSection : MonoBehaviour
{
    public AdaptyListener Listener;

    public TextMeshProUGUI StatusText;
    public TextMeshProUGUI InstallIdText;
    public TextMeshProUGUI InstallTimeText;
    public TextMeshProUGUI AppLaunchCountText;
    public TextMeshProUGUI PayloadText;

    public void SetInstallation(AdaptyInstallationStatus status)
    {
        SetStringValue(StatusText, GetStatusName(status));

        if (status is AdaptyInstallationStatusDetermined determined)
        {
            var details = determined.Details;
            SetStringValue(InstallIdText, ShortenUuid(details.InstallId));
            SetDateValue(InstallTimeText, details.InstallTime);
            SetIntegerValue(AppLaunchCountText, details.AppLaunchCount);
            SetStringValue(PayloadText, details.Payload);
        }
        else
        {
            SetNullValue(InstallIdText);
            SetNullValue(InstallTimeText);
            SetNullValue(AppLaunchCountText);
            SetNullValue(PayloadText);
        }
    }

    public void GetInstallationDetails()
    {
        Listener.GetInstallationDetails(
            (status, error) =>
            {
                if (error == null)
                {
                    SetInstallation(status);
                }
            }
        );
    }

    // HELPER METHODS
    private string ShortenUuid(string uuid)
    {
        if (string.IsNullOrEmpty(uuid))
        {
            return uuid;
        }

        // Show first 8 characters and last 4 characters with "..." in between
        if (uuid.Length > 12)
        {
            return $"{uuid.Substring(0, 8)}...{uuid.Substring(uuid.Length - 4)}";
        }

        return uuid;
    }

    private string GetStatusName(AdaptyInstallationStatus status)
    {
        switch (status)
        {
            case AdaptyInstallationStatusNotAvailable:
                return "Not Available";
            case AdaptyInstallationStatusNotDetermined:
                return "Not Determined";
            case AdaptyInstallationStatusDetermined:
                return "Determined";
            default:
                return status.GetType().Name;
        }
    }

    private void SetNullValue(TextMeshProUGUI text)
    {
        text.SetText("null");
    }

    private void SetBoolValue(TextMeshProUGUI text, bool value)
    {
        text.SetText(value ? "true" : "false");
    }

    private void SetDateValue(TextMeshProUGUI text, DateTime? value)
    {
        text.SetText(value?.ToShortDateString() ?? "null");
    }

    private void SetStringValue(TextMeshProUGUI text, string value)
    {
        text.SetText(value);
    }

    private void SetIntegerValue(TextMeshProUGUI text, int value)
    {
        text.SetText(value.ToString());
    }
}
