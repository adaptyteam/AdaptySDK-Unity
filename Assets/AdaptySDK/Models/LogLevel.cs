using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum LogLevel
        {
            None,
            Errors,
            Verbose,
            All
        }

        public static LogLevel LogLevelFromJSON(JSONNode response)
        {
            return LogLevelFromString(response);
        }

        public static LogLevel LogLevelFromString(string value)
        {
            if (value == null) return LogLevel.None;
            switch (value)
            {
                case "errors":
                    return LogLevel.Errors;
                case "verbose":
                    return LogLevel.Verbose;
                case "all":
                    return LogLevel.All;
                default:
                    return LogLevel.None;
            }
        }

        public static string LogLevelToString(this LogLevel value)
        {
            switch (value)
            {
                case LogLevel.Errors:
                    return "errors";
                case LogLevel.Verbose:
                    return "verbose";
                case LogLevel.All:
                    return "all";
                case LogLevel.None:
                default:
                    return "none";
            }
        }

    }
}
