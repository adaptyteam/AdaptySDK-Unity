using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

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
    public sealed partial class AdaptyProfile
    {
        private AdaptyProfile() => Freeze();

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
        private readonly List<string> _AppliedAttributionSources = new List<string>();

        /// <summary>
        /// The attribution sources applied to this profile.
        /// </summary>
        [Preserve]
        public IReadOnlyList<string> AppliedAttributionSources { get; private set; }

        /// <summary>
        /// Previously set user custom attributes with <see cref="Adapty.UpdateProfile(AdaptyProfileParameters, Action{AdaptyError})"/> method.
        /// </summary>
        [DataMember(Name = "custom_attributes")]
        [Newtonsoft.Json.JsonConverter(typeof(Serialization.AdaptyConverterLooseJson))]
        private readonly Dictionary<string, object> _CustomAttributes = new Dictionary<string, object>();

        /// <summary>
        /// The custom attributes set on this profile. Numbers arrive as <see cref="double"/>.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, object> CustomAttributes { get; private set; }

        /// <summary>
        /// A dictionary of access levels configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// The keys are access level identifiers configured by you in the Adapty Dashboard.
        /// The values are <see cref="AccessLevel"/> objects.
        /// Can be null if the customer has no access levels.
        /// </remarks>
        [DataMember(Name = "paid_access_levels")]
        private readonly Dictionary<string, AccessLevel> _AccessLevels = new Dictionary<string, AccessLevel>();

        /// <summary>
        /// The profile's access levels, keyed by the identifier configured in the Dashboard. Empty when
        /// the user has none.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, AccessLevel> AccessLevels { get; private set; }

        /// <summary>
        /// A dictionary of active subscriptions.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are <see cref="Subscription"/> objects.
        /// Can be null if the customer has no subscriptions.
        /// </remarks>
        [DataMember(Name = "subscriptions")]
        private readonly Dictionary<string, Subscription> _Subscriptions = new Dictionary<string, Subscription>();

        /// <summary>
        /// The profile's subscriptions, keyed by store product id. Empty when the user has none.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, Subscription> Subscriptions { get; private set; }

        /// <summary>
        /// A dictionary of non-subscription purchases.
        /// </summary>
        /// <remarks>
        /// The keys are product IDs from App Store Connect or Google Play Console.
        /// The values are lists of <see cref="NonSubscription"/> objects (one product can have multiple purchases).
        /// Can be null if the customer has no non-subscription purchases.
        /// </remarks>
        [DataMember(Name = "non_subscriptions")]
        private readonly Dictionary<string, List<NonSubscription>> _NonSubscriptions = new Dictionary<string, List<NonSubscription>>();

        /// <summary>
        /// The profile's non-subscription purchases, keyed by store product id — a list each, since one
        /// product can be bought more than once. Empty when the user has none.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, IReadOnlyList<NonSubscription>> NonSubscriptions { get; private set; }

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal readonly Int64 Version;

        [DataMember(Name = "is_test_user", IsRequired = true)]
        internal readonly bool IsTestUser;

        // Replace hands the deserializer a new collection instead of filling the one the field
        // initializer made, so the views are built here rather than alongside it.
        [Preserve]
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Freeze();

        private void Freeze()
        {
            AppliedAttributionSources = new ReadOnlyCollection<string>(_AppliedAttributionSources);
            CustomAttributes = new ReadOnlyDictionary<string, object>(_CustomAttributes);
            AccessLevels = new ReadOnlyDictionary<string, AccessLevel>(_AccessLevels);
            Subscriptions = new ReadOnlyDictionary<string, Subscription>(_Subscriptions);

            var nonSubscriptions = new Dictionary<string, IReadOnlyList<NonSubscription>>();
            foreach (var entry in _NonSubscriptions)
            {
                nonSubscriptions[entry.Key] = new ReadOnlyCollection<NonSubscription>(entry.Value);
            }
            NonSubscriptions = new ReadOnlyDictionary<string, IReadOnlyList<NonSubscription>>(
                nonSubscriptions
            );
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
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
