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
        /// <summary>
        /// An identifier of the user in Adapty
        /// </summary>
        public readonly string ProfileId;

        /// <summary>
        /// An identifier of the user in your system.
        /// </summary>
        public readonly string CustomerUserId;

        /// <summary>
        /// An identifier of the segment to which the user belongs.
        /// </summary>
        internal readonly string SegmentId;

        /// <summary>
        /// Previously set user custom attributes with `.updateProfile()` method.
        /// </summary>
        public readonly IDictionary<string, dynamic> CustomAttributes;

        /// <summary>
        /// The keys are access level identifiers configured by you in Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// The values are [AdaptyAccessLevelInfo] objects.
        /// Can be null if the customer has no access levels. 
        /// </remarks>
        public readonly IDictionary<string, AccessLevel> AccessLevels;

        /// <summary>
        /// The keys are product ids from App Store Connect.
        /// </summary>
        /// <remarks>
        /// The values are [AdaptySubscription] objects.
        /// Can be null if the customer has no subscriptions.
        /// </remarks>
        public readonly IDictionary<string, Subscription> Subscriptions;

        /// <summary>
        /// The keys are product ids from App Store Connect.
        /// </summary>
        /// <remarks>
        /// The values are array[] of [AdaptyNonSubscription] objects.
        /// Can be null if the customer has no purchases.
        /// </remarks>
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