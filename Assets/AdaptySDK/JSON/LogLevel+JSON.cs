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
            => value switch
            {
                Adapty.LogLevel.Error => "error",
                Adapty.LogLevel.Warn => "warn",
                Adapty.LogLevel.Info => "info",
                Adapty.LogLevel.Verbose => "verbose",
                Adapty.LogLevel.Debug => "debug",
                _ => throw new Exception($"LogLevel unknown value: {value}"),
            };

        internal static Adapty.LogLevel GetLogLevel(this JSONNode node, string aKey)
            => GetString(node, aKey).ToLogLevel();
        internal static Adapty.LogLevel? GetLogLevelIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToLogLevel();

        internal static Adapty.LogLevel ToLogLevel(this string value)
            => value switch
            {
                "error" => Adapty.LogLevel.Error,
                "warn" => Adapty.LogLevel.Warn,
                "info" => Adapty.LogLevel.Info,
                "verbose" => Adapty.LogLevel.Verbose,
                "debug" => Adapty.LogLevel.Debug,
                _ => throw new Exception($"LogLevel unknown value: {value}"),
            };
    }
}