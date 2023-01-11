//
//  Profile.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Profile
        {
            /// An identifier of the user in Adapty
            public readonly string ProfileId;

            /// An identifier of the user in your system.
            ///
            /// [Nullable]
            public readonly string CustomerUserId;

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

            public override string ToString()
            {
                return $"{nameof(ProfileId)}: {ProfileId}, " +
                       $"{nameof(CustomerUserId)}: {CustomerUserId}, " +
                       $"{nameof(CustomAttributes)}: {CustomAttributes}, " +
                       $"{nameof(AccessLevels)}: {AccessLevels}, " +
                       $"{nameof(Subscriptions)}: {Subscriptions}, " +
                       $"{nameof(NonSubscriptions)}: {NonSubscriptions}";
            }
        }
    }
}