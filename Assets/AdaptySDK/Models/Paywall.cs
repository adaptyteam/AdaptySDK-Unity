using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class Paywall
        {
            /// The identifier of the paywall, configured in Adapty Dashboard.
            public readonly string DeveloperId;

            /// The identifier of the variation, used to attribute purchases to the paywall.
            public readonly string VariationId;

            /// The current revision (version) of the paywall.
            /// Every change within the paywall creates a new revision.
            public readonly int Revision;

            /// Whether this paywall is a part of the Promo Campaign.
            public readonly bool IsPromo;

            /// An array of ProductModel objects related to this paywall.
            public readonly Product[] Products;

            /// The custom JSON formatted data configured in Adapty Dashboard.
            public Dictionary<string, dynamic> CustomPayload;

            /// The custom JSON formatted data configured in Adapty Dashboard.
            /// (String representation)
            public readonly string CustomPayloadString;

            /// Paywall A/B test name
            public readonly string ABTestName;

            /// Paywall name
            public readonly string Name;


            internal Paywall(JSONNode response)
            {

                DeveloperId = response["developer_id"];
                VariationId = response["variation_id"];
                Revision = response["revision"];
                IsPromo = response["is_promo"];
                var products = response["products"];
                if (products != null && !products.IsNull && products.IsArray)
                {
                    var list = new List<Product>();
                    foreach (var item in products)
                    {
                        var value = ProductFromJSON(item);
                        if (value != null)
                        {
                            list.Add(value);
                        }
                    }
                    this.Products = list.ToArray();
                }
                CustomPayloadString = response["custom_payload_string"] ?? response["custom_payload"]; 
                ABTestName = response["ab_test_name"] ?? response["a_b_test_name"];
                Name = response["name"] ?? response["paywall_name"]; 

                if (!string.IsNullOrEmpty(CustomPayloadString))
                {
                    CustomPayload = JSON.Parse(CustomPayloadString).ToDictionary();
                }

             }

            public override string ToString()
            {
                return $"{nameof(DeveloperId)}: {DeveloperId}, " +
                       $"{nameof(VariationId)}: {VariationId}, " +
                       $"{nameof(Revision)}: {Revision}, " +
                       $"{nameof(IsPromo)}: {IsPromo}, " +
                       $"{nameof(Products)}: {Products}, " +
                       $"{nameof(CustomPayloadString)}: {CustomPayloadString}, " +
                       $"{nameof(ABTestName)}: {ABTestName}, " +
                       $"{nameof(Name)}: {Name}";
            }
        }

        public static Paywall PaywallFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try { 
                return new Paywall(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding Paywall: {e} source: {response}");
                return null;
            }
        }
    }
}
