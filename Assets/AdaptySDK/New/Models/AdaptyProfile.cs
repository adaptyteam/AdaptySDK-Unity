//
//  AdaptyProfile.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptyProfile
    {
        /// An identifier of the user in Adapty
        public readonly string ProfileId;

        //private readonly string _SegmentId;

        /// An identifier of the user in your system.
        ///
        /// [Nullable]
        public readonly string CustomerUserId;

        internal readonly string SegmentId;

        /// Previously set user custom attributes with `.updateProfile()` method.
        public readonly IDictionary<string, dynamic> CustomAttributes;

        /// The keys are access level identifiers configured by you in Adapty Dashboard.
        /// The values are [AdaptyAccessLevelInfo] objects.
        /// Can be null if the customer has no access levels.
        public readonly IDictionary<string, AccessLevel> AccessLevels;

        /// The keys are product ids from App Store Connect.
        /// The values are [AdaptySubscription] objects.
        /// Can be null if the customer has no subscriptions.
        public readonly IDictionary<string, Subscription> Subscriptions;

        /// The keys are product ids from App Store Connect.
        /// The values are array[] of [AdaptyNonSubscription] objects.
        /// Can be null if the customer has no purchases.
        public readonly IDictionary<string, IList<NonSubscription>> NonSubscriptions;

        internal readonly Int64 Version;

        internal readonly bool IsTestUser;

        public override string ToString() => 
            $"{nameof(ProfileId)}: {ProfileId}, " +
            $"{nameof(SegmentId)}: {SegmentId}, " +
            $"{nameof(CustomerUserId)}: {CustomerUserId}, " +
            $"{nameof(CustomAttributes)}: {CustomAttributes}, " +
            $"{nameof(AccessLevels)}: {AccessLevels}, " +
            $"{nameof(Subscriptions)}: {Subscriptions}, " +
            $"{nameof(NonSubscriptions)}: {NonSubscriptions}, " +
            $"{nameof(Version)}: {Version}, " +
            $"{nameof(IsTestUser)}: {IsTestUser}";
    }
}