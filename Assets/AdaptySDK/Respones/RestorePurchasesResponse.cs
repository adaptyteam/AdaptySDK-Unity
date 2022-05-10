using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class RestorePurchasesResponse
        {
            public readonly PurchaserInfo PurchaserInfo;
            /// iOS specific
            public readonly string Receipt;
            public RestorePurchasesResponse(JSONNode response)
            {
                PurchaserInfo = PurchaserInfoFromJSON(response["purchaser_info"]);
                Receipt = response["receipt"];
            }

            public override string ToString()
            {
                return $"{nameof(PurchaserInfo)}: {PurchaserInfo}, " +
                       $"{nameof(Receipt)}: {Receipt}";         
            }

        }
    }
}
