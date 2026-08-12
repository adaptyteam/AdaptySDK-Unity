using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Gives every entry into Play Mode an identity of its own, and wipes what the entry before it left
/// behind — both before <see cref="AdaptyPlayModeSeed"/> gets to write anything.
/// </summary>
/// <remarks>
/// This is deliberately not part of the seed. The failure it defends against is the seed not
/// running: an entry that seeded and then never reached Play Mode leaves its marker on disk, and
/// <see cref="PlayerPrefs"/> outlive not just the entry but the Editor session. A marker is
/// therefore accepted only while it names the entry now under way, and the seed cannot name it
/// without running — while whether it ran is exactly what the Play Mode test is asking.
/// </remarks>
[InitializeOnLoad]
public static class AdaptyPlayModeEntryStamp
{
    /// <summary>
    /// The entry now under way. Rotated on the way into every Play Mode entry, never consumed.
    /// </summary>
    public const string EntryKey = "adapty.playmode.entry";

    /// <summary>
    /// What the seed leaves for the test: <c>&lt;entry&gt;:&lt;UTC ticks&gt;:&lt;1 if Domain Reload
    /// is off&gt;</c>. Owned here rather than by the seed, so removing the seed cannot take the
    /// cleanup with it.
    /// </summary>
    public const string SeedKey = "adapty.playmode.seed";

    static AdaptyPlayModeEntryStamp()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.ExitingEditMode)
        {
            return;
        }

        PlayerPrefs.SetString(EntryKey, Guid.NewGuid().ToString("N"));
        PlayerPrefs.DeleteKey(SeedKey);
        PlayerPrefs.Save();
    }
}
