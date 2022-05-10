using System.Collections.Generic;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class GetPaywallsResponse
        {
            public readonly Paywall[] Paywalls;
            public readonly Product[] Products;

            public GetPaywallsResponse(JSONNode response)
            {
                var paywalls = new List<Paywall>();
                var products = new List<Product>();


                var paywallsArray = response["paywalls"];
                if (paywallsArray != null && !paywallsArray.IsNull && paywallsArray.IsArray)
                {
                    foreach (var item in paywallsArray)
                    {
                        var value = PaywallFromJSON(item);
                        if (value != null) paywalls.Add(value);
                        
                    }
                    this.Paywalls = paywalls.ToArray();
                }


                var productsArray = response["products"];
                if (productsArray != null && !productsArray.IsNull && productsArray.IsArray)
                {
                    foreach (var item in productsArray)
                    {
                        var product = ProductFromJSON(item);
                        if (product != null) products.Add(product);
                    }
                    this.Products = products.ToArray();
                }
            }

            public override string ToString()
            {
                return $"{nameof(Paywalls)}: {Paywalls}, " +
                       $"{nameof(Products)}: {Products}";
            }
        }

    }
}
