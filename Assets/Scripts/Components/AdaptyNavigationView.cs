using UnityEngine;
using UnityEngine.UI;

public class AdaptyNavigationView : MonoBehaviour
{

    public Text BackButtonText;
    public Button BackButton;
    public Text TitleText;


    public void Configure(string title, bool showBackButton)
    {
        TitleText.text = title;
        BackButton.gameObject.SetActive(showBackButton);
    }
}
