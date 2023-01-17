using System.Collections;
using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;

public class IdentifySection : MonoBehaviour {
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public TMP_InputField TextField;


    public void SetProfile(Adapty.Profile profile) {
        Debug.Log(string.Format("#IdentifySection# UpdateProfile {0}", profile.CustomerUserId));

        if (profile.CustomerUserId != null) {
            this.TextField.SetTextWithoutNotify(profile.CustomerUserId);
        }
    }

    public void IdentifyPressed() {
        this.Router.SetIsLoading(true);

        this.Listener.Identify(this.TextField.text, (error) => {
            this.Router.SetIsLoading(false);
        });
    }
}
