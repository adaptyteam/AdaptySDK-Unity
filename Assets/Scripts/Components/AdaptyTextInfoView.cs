using UnityEngine;
using UnityEngine.UI;
using AdaptySDK;

public class AdaptyTextInfoView : MonoBehaviour {
	static Color green = new Color(109.0f / 255.0f, 255.0f / 255.0f, 88.0f / 255.0f, 0.5f);
	static Color red = new Color(255.0f / 255.0f, 88.0f / 255.0f, 99.0f / 255.0f, 0.8f);

	public Text InfoText;
	public Image backgroundImage;

	public void ConfigurePurchaserInfo(Adapty.PurchaserInfo info) {
		if (info == null) {
			InfoText.text = "Purchaser Info: null";
			backgroundImage.color = red;
			return;
		}

		var levels = "";
		var isActive = false;
		var now = System.DateTime.Now;

		if (info.AccessLevels != null) {
			foreach (var level in info.AccessLevels) {
				levels += string.Format("{0} until {1}", level.Value.Id, level.Value.ExpiresAt);
				isActive = isActive || level.Value.ExpiresAt > now;
			}
		}

		InfoText.text = string.Format("Purchaser Info:\nAccess Levels: {0}\nSubscriptions: {1}\nNon Subscriptions: {2}", levels, info.Subscriptions.Count, info.NonSubscriptions.Count);
		backgroundImage.color = isActive ? green : red;
	}

	public void ConfigurePromo(Adapty.Promo promo) {
		if (promo == null) {
			InfoText.text = "No promo found.";
			backgroundImage.color = red;
			return;
		}

		InfoText.text = string.Format("Promo:\nVariation Id: {0}\nPaywall Id: {2}", promo.VariationId, promo.Paywall.DeveloperId);
	}

	public void Configure(string text) {
		InfoText.text = text;
	}
}
