//
//  AdaptyProfile.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a user profile in Adapty.
    /// </summary>
    /// <remarks>
    /// The profile contains all information about the user including access levels, subscriptions, non-subscription purchases, and custom attributes.
    /// Read more at <see href="https://adapty.io/docs/unity-check-subscription-status">Adapty Documentation</see>
    /// </remarks>
    [DataContract]
    public partial class AdaptyProfile
    {
        private AdaptyProfile() { }

        /// <summary>
        /// An identifier of the user in Adapty.
        /// </summary>
        [DataMember(Name = "profile_id", IsRequired = true)]
        public readonly string ProfileId;

        /// <summary>
        /// An identifier of the user in your system.
        /// </summary>
        /// <remarks>
        /// This is the customer user ID that you set using <see cref="Adapty.Identify(string, Action{AdaptyError})"/>.
        /// </remarks>
        [DataMember(Name = "customer_user_id")]
        public readonly string CustomerUserId;

        /// <summary>
        /// An identifier of the segment to which the user belongs.
        /// </summary>
        [DataMember(Name = "segment_hash", IsRequired = true)]
        internal readonly string SegmentId;

        /// <summary>
        /// Identifiers of attribution sources applied to the profile.
        /// </summary>
        [DataMember(Name = "applied_attribution_sources")]
        public readonly IList<string> AppliedAttributionSources = new List<string>();

        /// <summary>
        /// Previously set user custom attributes with <see cref="Adapty.UpdateProfile(AdaptyProfileParameters, Action{AdaptyError})"/> method.
        /// </summary>
        [DataMember(Name = "custom_attributes")]
        public readonly IDictionary<string, object> CustomAttributes = new Dictionary<string, object>();

        /// <summary>
        /// A dictionary of access levels configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// The keys are access level identifiers configured by you in the Adapty Dashboard.
        /// The values are <see cref="AccessLevel"/> objects.
        /// Can be null if the customer has no access levels.
        /// </remarks>
        [DataMember(Name = "paid_access_levels")]
        public readonly IDictionary<string, AccessLevel> AccessLevels = new Dictionary<string, AccessLevel>();

        /// <summary>
        /// A dictionary of active subscriptions.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are <see cref="Subscription"/> objects.
        /// Can be null if the customer has no subscriptions.
        /// </remarks>
        [DataMember(Name = "subscriptions")]
        public readonly IDictionary<string, Subscription> Subscriptions = new Dictionary<string, Subscription>();

        /// <summary>
        /// A dictionary of non-subscription purchases.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are lists of <see cref="NonSubscription"/> objects (one product can have multiple purchases).
        /// Can be null if the customer has no non-subscription purchases.
        /// </remarks>
        [DataMember(Name = "non_subscriptions")]
        public readonly IDictionary<string, IList<NonSubscription>> NonSubscriptions = new Dictionary<string, IList<NonSubscription>>();

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal readonly Int64 Version;

        [DataMember(Name = "is_test_user", IsRequired = true)]
        internal readonly bool IsTestUser;

        public override string ToString()
        {
            var customAttributesStr =
                CustomAttributes == null
                    ? "null"
                    : "{"
                        + string.Join(", ", CustomAttributes.Select(kv => $"{kv.Key}: {kv.Value}"))
                        + "}";

            var accessLevelsStr =
                AccessLevels == null
                    ? "null"
                    : "{"
                        + string.Join(", ", AccessLevels.Select(kv => $"{kv.Key}: [{kv.Value}]"))
                        + "}";

            var subscriptionsStr =
                Subscriptions == null
                    ? "null"
                    : "{"
                        + string.Join(", ", Subscriptions.Select(kv => $"{kv.Key}: [{kv.Value}]"))
                        + "}";

            var nonSubscriptionsStr =
                NonSubscriptions == null
                    ? "null"
                    : "{"
                        + string.Join(
                            ", ",
                            NonSubscriptions.Select(kv =>
                                $"{kv.Key}: [{string.Join(", ", kv.Value.Select(ns => $"[{ns}]"))}]"
                            )
                        )
                        + "}";

            return $"{nameof(ProfileId)}: {ProfileId}, "
                + $"{nameof(SegmentId)}: {SegmentId}, "
                + $"{nameof(CustomerUserId)}: {CustomerUserId}, "
                + $"{nameof(AppliedAttributionSources)}: [{string.Join(", ", AppliedAttributionSources)}], "
                + $"{nameof(CustomAttributes)}: {customAttributesStr}, "
                + $"{nameof(AccessLevels)}: {accessLevelsStr}, "
                + $"{nameof(Subscriptions)}: {subscriptionsStr}, "
                + $"{nameof(NonSubscriptions)}: {nonSubscriptionsStr}, "
                + $"{nameof(Version)}: {Version}, "
                + $"{nameof(IsTestUser)}: {IsTestUser}";
        }
    }
}
