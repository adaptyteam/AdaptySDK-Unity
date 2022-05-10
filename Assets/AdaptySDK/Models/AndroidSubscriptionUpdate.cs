using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum AndroidSubscriptionUpdateProrationMode
        {
            ImmediateWithTimeProration = 0,
            ImmediateAndChargeProratedPrice = 1,
            ImmediateWithoutProration = 2,
            Deferred = 3,
            ImmediateAndChargeFullPrice = 4
        }

        public class AndroidSubscriptionUpdate
        {
            

            public string OldSubVendorProductId;
            public AndroidSubscriptionUpdateProrationMode ProrationMode;

            public AndroidSubscriptionUpdate(string oldSubVendorProductId, AndroidSubscriptionUpdateProrationMode prorationMode)
            {
                OldSubVendorProductId = oldSubVendorProductId;
                ProrationMode = prorationMode;
            }

            public AndroidSubscriptionUpdate(AndroidSubscriptionUpdateProrationMode prorationMode): this(null, prorationMode)
            {}

            public override string ToString()
            {
                return $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, " +
                       $"{nameof(ProrationMode)}: {ProrationMode}";
            }

            public string ToJSONString()
            {
                try { 
                    JSONNode node = new JSONObject();
                    if (OldSubVendorProductId != null)
                    {
                        node.Add("old_sub_vendor_product_id", OldSubVendorProductId);
                    }
                    node.Add("proration_mode", AndroidSubscriptionUpdateProrationModeToString(ProrationMode));
                    return node.ToString();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception on encoding AndroidSubscriptionUpdate: {e} source: {ToString()}");
                    return null;
                }
            }
        }
     

        public static string AndroidSubscriptionUpdateProrationModeToString(this AndroidSubscriptionUpdateProrationMode value)
        {
            switch (value)
            {
                case AndroidSubscriptionUpdateProrationMode.ImmediateWithTimeProration:
                    return "immediate_with_time_proration";
                case AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeProratedPrice:
                    return "immediate_and_charge_prorated_price";
                case AndroidSubscriptionUpdateProrationMode.ImmediateWithoutProration:
                    return "immediate_without_proration";
                case AndroidSubscriptionUpdateProrationMode.Deferred:
                    return "deferred";
                case AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeFullPrice:
                    return "immediate_and_charge_full_price";
            }

            throw new Exception($"AndroidSubscriptionUpdateProrationMode unknown value: {value}");
        }

        public static int AndroidSubscriptionUpdateProrationModeToInt(AndroidSubscriptionUpdateProrationMode value)
        {
            return (int)value;
        }
    }
}
