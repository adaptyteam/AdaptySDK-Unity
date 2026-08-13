using System;
using System.Collections.Generic;
using AdaptySDK.Serialization;
using Newtonsoft.Json.Linq;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        /// <summary>
        /// Adapty allows you remotely configure onboarding screens that will be displayed in your app.
        /// This way you don't have to hardcode the onboarding content and can dynamically change it or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="loadTimeout">The timeout for the onboarding loading.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete("The legacy onboarding API is deprecated in favor of Flows. Use GetFlow instead.")]
        public static void GetOnboarding(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            TimeSpan? loadTimeout,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();

            parameters["placement_id"] = placementId;

            if (locale != null)
            {
                parameters["locale"] = locale;
            }

            if (fetchPolicy != null)
            {
                parameters["fetch_policy"] = AdaptyJson.ToNode(fetchPolicy);
            }

            if (loadTimeout.HasValue)
            {
                parameters["load_timeout"] = loadTimeout.Value.TotalSeconds;
            }

            Request.Send("get_onboarding", parameters, completionHandler);
        }

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use GetFlowForDefaultAudience instead."
        )]
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["placement_id"] = placementId;

            if (locale != null)
            {
                parameters["locale"] = locale;
            }

            if (fetchPolicy != null)
            {
                parameters["fetch_policy"] = AdaptyJson.ToNode(fetchPolicy);
            }

            Request.Send("get_onboarding_for_default_audience", parameters, completionHandler);
        }

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding localization.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use GetFlowForDefaultAudience instead."
        )]
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            string locale,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, locale, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use GetFlowForDefaultAudience instead."
        )]
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use GetFlowForDefaultAudience instead."
        )]
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, null, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure onboarding screens that will be displayed in your app.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete("The legacy onboarding API is deprecated in favor of Flows. Use GetFlow instead.")]
        public static void GetOnboarding(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboarding(placementId, null, null, null, completionHandler);
    }
}
