using System.Collections.Generic;
using AdaptySDK.SimpleJSON;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    public interface AdaptyEventListener
    {
        void OnReceiveUpdatedPurchaserInfo(Adapty.PurchaserInfo purchaserInfo);
        void OnReceivePromo(Adapty.Promo promo);
        void OnDeferredPurchasesProduct(Adapty.Product product);
        void OnReceivePaywallsForConfig(Adapty.Paywall[] paywalls);
    }

    public static partial class Adapty
    {
        private static AdaptyEventListener m_Listener;

        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        internal static void OnMessage(string type, string json)
        {
            if (string.IsNullOrEmpty(json) || m_Listener == null) return;
            var response = JSON.Parse(json);
            if (response == null || response.IsNull) return;
            switch (type)
            {
                case "purchaser_info_update":
                    var info = Adapty.PurchaserInfoFromJSON(response);
                    if (info == null) return;
                    m_Listener.OnReceiveUpdatedPurchaserInfo(info);
                    return;
                case "promo_received":
                    var promo = Adapty.PromoFromJSON(response);
                    if (promo == null) return;
                    m_Listener.OnReceivePromo(promo);
                    return;
                case "deferred_purchase":
                    var product = Adapty.ProductFromJSON(response);
                    if (product == null) return;
                    m_Listener.OnDeferredPurchasesProduct(product);
                    return;
                case "remote_config_update":
                    if (!response.IsArray) return;
                    var list = new List<Adapty.Paywall>();
                    foreach (var node in response.Children)
                    {
                        var value = Adapty.PaywallFromJSON(node);
                        if (value != null)
                        {
                            list.Add(value);
                        }
                    }
                    m_Listener.OnReceivePaywallsForConfig(list.ToArray());
                    return;
            }
        }
    }
}