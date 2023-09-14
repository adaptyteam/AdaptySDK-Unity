//
//  ProfileGender.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.ProfileGender value)
        {
            switch (value)
            {
                case Adapty.ProfileGender.Female: return "f";
                case Adapty.ProfileGender.Male: return "m";
                default: return "o";
            }
        }

        internal static Adapty.ProfileGender GetProfileGender(this JSONNode node, string aKey)
            => GetString(node, aKey).ToProfileGender();

        internal static Adapty.ProfileGender? GetProfileGenderIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToProfileGender();

        internal static Adapty.ProfileGender ToProfileGender(this string value)
        {
            switch (value)
            {
                case "f": return Adapty.ProfileGender.Female;
                case "m": return Adapty.ProfileGender.Male;
                default: return Adapty.ProfileGender.Other;
            }
        }
    }
}