using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class Promo
        {
            /// The type of the promo offer.
            public readonly string PromoType;

            /// The identifier of the variation, used to attribute purchases to the promo.
            public readonly string VariationId;

            /// The time when the current promo offer will expire.
            public readonly DateTime? ExpiresAt;

            /// A [Adapty.Paywall] object.
            public readonly Paywall Paywall;


            internal Promo(JSONNode response)
            {
                PromoType = response["promo_type"];
                ExpiresAt = NullableDateTimeFromJSON(response["expires_at"]);
                VariationId = response["variation_id"];
                Paywall = PaywallFromJSON(response["paywall"]);
            }

            public override string ToString()
            {
                return $"{nameof(PromoType)}: {PromoType}, " +
                       $"{nameof(ExpiresAt)}: {ExpiresAt}, " +
                       $"{nameof(VariationId)}: {VariationId}, " +
                       $"{nameof(Paywall)}: {Paywall}";
            }
        }

        public static Promo PromoFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try
            {
                return new Promo(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding Promo: {e} source: {response}");

                return null;
            }
        }
    }
}