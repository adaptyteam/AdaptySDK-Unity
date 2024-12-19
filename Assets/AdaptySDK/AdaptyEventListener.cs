using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public interface AdaptyEventListener
    {
        void OnLoadLatestProfile(AdaptyProfile profile);
        void PaywallViewDidPerformAction(AdaptyUIView view, AdaptyUIUserAction action);
        void PaywallViewDidSelectProduct(AdaptyUIView view, string productId);
        void PaywallViewDidStartPurchase(AdaptyUIView view, AdaptyPaywallProduct product);
        void PaywallViewDidFinishPurchase(AdaptyUIView view, AdaptyPaywallProduct product, AdaptyPurchaseResult purchasedResult);
        void PaywallViewDidFailPurchase(AdaptyUIView view, AdaptyPaywallProduct product, AdaptyError error);
        void PaywallViewDidStartRestore(AdaptyUIView view);
        void PaywallViewDidFinishRestore(AdaptyUIView view, AdaptyProfile profile);
        void PaywallViewDidFailRestore(AdaptyUIView view, AdaptyError error);
        void PaywallViewDidFailRendering(AdaptyUIView view, AdaptyError error);
        void PaywallViewDidFailLoadingProducts(AdaptyUIView view, AdaptyError error);
    }

    public static partial class Adapty
    {
        private static AdaptyEventListener m_Listener;

        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        internal static void OnMessage(string id, string json)
        {
            if (string.IsNullOrEmpty(json) || m_Listener == null) return;
            var response = JSONNode.Parse(json);
            if (response == null || response.IsNull) return;
            if (!response.IsObject) return;
            var parapeters = response.AsObject;
            switch (id)
            {
                case "did_load_latest_profile":
                    {
                        var profile = parapeters.GetAdaptyProfile("profile");
                        m_Listener.OnLoadLatestProfile(profile);
                        return;
                    }
                case "paywall_view_did_perform_action":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var action = parapeters.GetAdaptyUIUserAction("action");
                        m_Listener.PaywallViewDidPerformAction(view, action);
                        return;
                    }
                case "paywall_view_did_select_product":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var productId = parapeters.GetString("product_id");
                        m_Listener.PaywallViewDidSelectProduct(view, productId);
                        return;
                    }
                case "paywall_view_did_start_purchase":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var product = parapeters.GetAdaptyPaywallProduct("product");
                        m_Listener.PaywallViewDidStartPurchase(view, product);
                        return;
                    }
                case "paywall_view_did_finish_purchase":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var product = parapeters.GetAdaptyPaywallProduct("product");
                        var purchaseResult = parapeters.GetAdaptyPurchaseResult("purchased_result");
                        m_Listener.PaywallViewDidFinishPurchase(view, product, purchaseResult);
                        return;
                    }
                case "paywall_view_did_fail_purchase":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var product = parapeters.GetAdaptyPaywallProduct("product");
                        var error = parapeters.GetAdaptyError("error");
                        m_Listener.PaywallViewDidFailPurchase(view, product, error);
                        return;
                    }
                case "paywall_view_did_start_restore":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        m_Listener.PaywallViewDidStartRestore(view);
                        return;
                    }
                case "paywall_view_did_finish_restore":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var profile = parapeters.GetAdaptyProfile("profile");
                        m_Listener.PaywallViewDidFinishRestore(view, profile);
                        return;
                    }
                case "paywall_view_did_fail_restore":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var error = parapeters.GetAdaptyError("error");
                        m_Listener.PaywallViewDidFailRestore(view, error);
                        return;
                    }
                case "paywall_view_did_fail_rendering":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var error = parapeters.GetAdaptyError("error");
                        m_Listener.PaywallViewDidFailRendering(view, error);
                        return;
                    }
                case "paywall_view_did_fail_loading_products":
                    {
                        var view = parapeters.GetAdaptyUIView("view");
                        var error = parapeters.GetAdaptyError("error");
                        m_Listener.PaywallViewDidFailLoadingProducts(view, error);
                        return;
                    }
                default:
                    return;
            }
        }
    }
}