//
//  LogLevel.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.LogLevel value)
        {
            switch (value)
            {
                case Adapty.LogLevel.Error: return "error";
                case Adapty.LogLevel.Warn: return "warn";
                case Adapty.LogLevel.Info: return "info";
                case Adapty.LogLevel.Verbose: return "verbose";
                case Adapty.LogLevel.Debug: return "debug";
                default: throw new Exception($"LogLevel unknown value: {value}");
            }
        }

        internal static Adapty.LogLevel GetLogLevel(this JSONNode node, string aKey)
            => GetString(node, aKey).ToLogLevel();
        internal static Adapty.LogLevel? GetLogLevelIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToLogLevel();

        internal static Adapty.LogLevel ToLogLevel(this string value)
        {
            switch (value)
            {
                case "error": return Adapty.LogLevel.Error;
                case "warn": return Adapty.LogLevel.Warn;
                case "info": return Adapty.LogLevel.Info;
                case "verbose": return Adapty.LogLevel.Verbose;
                case "debug": return Adapty.LogLevel.Debug;
                default: throw new Exception($"LogLevel unknown value: {value}");
            }
        }
    }
}