using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class MakePurchaseResponse
        {
            public readonly PurchaserInfo PurchaserInfo;
            /// iOS specific
            public readonly string Receipt;
            /// Android specific
            public readonly string PurchaseToken;
            public readonly Product Product;

            public MakePurchaseResponse(JSONNode response)
            {
                PurchaserInfo = PurchaserInfoFromJSON(response["purchaser_info"]);
                Receipt = response["receipt"];
                PurchaseToken = response["purchase_token"];
                Product = ProductFromJSON(response["product"]);
            }

            public override string ToString()
            {
                return $"{nameof(PurchaserInfo)}: {PurchaserInfo}, " +
                       $"{nameof(Receipt)}: {Receipt}, " +
                       $"{nameof(PurchaseToken)}: {PurchaseToken}, " +
                       $"{nameof(Product)}: {Product}";
            }
        }

    }
}
