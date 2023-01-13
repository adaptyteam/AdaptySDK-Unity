using AdaptySDK;
using TMPro;
using UnityEngine;

public class ProfileIdSection : MonoBehaviour
{

    public TextMeshProUGUI ProfileIdText;
    private Adapty.Profile m_profile;

    public void SetProfile(Adapty.Profile profile)
    {
        this.ProfileIdText.SetText(profile.ProfileId);
        this.m_profile = profile;
    }

    public void CopyProfileIdPressed()
    {
        if (this.m_profile != null)
        {
            GUIUtility.systemCopyBuffer = this.m_profile.ProfileId;
        }
    }
}