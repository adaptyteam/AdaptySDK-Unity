using System;
using System.Globalization;
using AdaptySDK;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Leaves a listener registered just before Play Mode starts, so a Play Mode test can tell whether
/// the SDK's own reset ran.
/// </summary>
/// <remarks>
/// With Domain Reload disabled a listener registered by the previous run survives into the next
/// one, and this seed is that leftover: <c>InitializeOnEnterPlayMode</c> runs before
/// <c>RuntimeInitializeLoadType.SubsystemRegistration</c>, so the SDK's reset sees it exactly as it
/// would see a real one. The point is not that the reset clears the field — a desktop test already
/// calls it directly — but that Unity calls it at all.
/// </remarks>
public static class AdaptyPlayModeSeed
{
    private sealed class Sink : IAdaptyEventListener
    {
        public void OnLoadLatestProfile(AdaptyProfile profile) { }

        public void OnReceivePromotedPurchase(AdaptyPromotedProduct product) { }

        public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details) { }

        public void OnInstallationDetailsFail(AdaptyError error) { }
    }

    /// <summary>
    /// Registers the listener and names the entry it was registered for.
    /// </summary>
    /// <remarks>
    /// The marker is that entry's own id rather than a flag or a timestamp of this method's
    /// choosing: <see cref="AdaptyPlayModeEntryStamp"/> rotates the id on the way into every entry,
    /// so a marker this method did not write for the entry now under way cannot match. The
    /// timestamp beside it is a second bound, for the case where the rotation stops happening.
    /// </remarks>
    [InitializeOnEnterPlayMode]
    private static void Seed(EnterPlayModeOptions options)
    {
        Adapty.SetEventListener(new Sink());

        var noDomainReload =
            EditorSettings.enterPlayModeOptionsEnabled
            && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);

        PlayerPrefs.SetString(
            AdaptyPlayModeEntryStamp.SeedKey,
            string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}:{2}",
                PlayerPrefs.GetString(AdaptyPlayModeEntryStamp.EntryKey, string.Empty),
                DateTime.UtcNow.Ticks,
                noDomainReload ? 1 : 0
            )
        );
        PlayerPrefs.Save();
    }
}
