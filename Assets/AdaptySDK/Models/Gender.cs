using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum Gender
        {
            Female,
            Male,
            Other
        }


        public static Gender GenderFromJSON(JSONNode response)
        {
            return GenderFromString(response);
        }

        public static Gender GenderFromString(string value)
        {
            if (value == null) return Gender.Other;
            switch (value)
            {
                case "f":
                case "female":
                    return Gender.Female;
                case "m":
                case "male":
                    return Gender.Male;
                default:
                    return Gender.Other;
            }
        }

        public static string GenderToString(this Gender value)
        {
            switch (value)
            {
                case Gender.Female:
                    return "f";
                case Gender.Male:
                    return "m";
                case Gender.Other:
                default:
                    return "o";
            }
        }
    }
}