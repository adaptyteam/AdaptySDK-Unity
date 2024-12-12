//
//  AdaptyProfileGender.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this AdaptyProfileGender value)
        {
            switch (value)
            {
                case AdaptyProfileGender.Female: return "f";
                case AdaptyProfileGender.Male: return "m";
                default: return "o";
            }
        }

        internal static AdaptyProfileGender GetProfileGender(this JSONNode node, string aKey)
            => GetString(node, aKey).ToProfileGender();

        internal static AdaptyProfileGender? GetProfileGenderIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToProfileGender();

        internal static AdaptyProfileGender ToProfileGender(this string value)
        {
            switch (value)
            {
                case "f": return AdaptyProfileGender.Female;
                case "m": return AdaptyProfileGender.Male;
                default: return AdaptyProfileGender.Other;
            }
        }
    }
}