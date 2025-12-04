//
//  AdaptyProfile.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a user profile in Adapty.
    /// </summary>
    /// <remarks>
    /// The profile contains all information about the user including access levels, subscriptions, non-subscription purchases, and custom attributes.
    /// Read more at <see href="https://adapty.io/docs/unity-check-subscription-status">Adapty Documentation</see>
    /// </remarks>
    public partial class AdaptyProfile
    {
        /// <summary>
        /// An identifier of the user in Adapty.
        /// </summary>
        public readonly string ProfileId;

        /// <summary>
        /// An identifier of the user in your system.
        /// </summary>
        /// <remarks>
        /// This is the customer user ID that you set using <see cref="Adapty.Identify(string, Action{AdaptyError})"/>.
        /// </remarks>
        public readonly string CustomerUserId;

        /// <summary>
        /// An identifier of the segment to which the user belongs.
        /// </summary>
        internal readonly string SegmentId;

        /// <summary>
        /// Previously set user custom attributes with <see cref="Adapty.UpdateProfile(AdaptyProfileParameters, Action{AdaptyError})"/> method.
        /// </summary>
        public readonly IDictionary<string, dynamic> CustomAttributes;

        /// <summary>
        /// A dictionary of access levels configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// The keys are access level identifiers configured by you in the Adapty Dashboard.
        /// The values are <see cref="AccessLevel"/> objects.
        /// Can be null if the customer has no access levels.
        /// </remarks>
        public readonly IDictionary<string, AccessLevel> AccessLevels;

        /// <summary>
        /// A dictionary of active subscriptions.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are <see cref="Subscription"/> objects.
        /// Can be null if the customer has no subscriptions.
        /// </remarks>
        public readonly IDictionary<string, Subscription> Subscriptions;

        /// <summary>
        /// A dictionary of non-subscription purchases.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are lists of <see cref="NonSubscription"/> objects (one product can have multiple purchases).
        /// Can be null if the customer has no non-subscription purchases.
        /// </remarks>
        public readonly IDictionary<string, IList<NonSubscription>> NonSubscriptions;

        internal readonly Int64 Version;

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
                + $"{nameof(CustomAttributes)}: {customAttributesStr}, "
                + $"{nameof(AccessLevels)}: {accessLevelsStr}, "
                + $"{nameof(Subscriptions)}: {subscriptionsStr}, "
                + $"{nameof(NonSubscriptions)}: {nonSubscriptionsStr}, "
                + $"{nameof(Version)}: {Version}, "
                + $"{nameof(IsTestUser)}: {IsTestUser}";
        }
    }
}
