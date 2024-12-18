//
//  AdaptyLogLevel+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptyLogLevel value) =>
            value switch
            {
                AdaptyLogLevel.Error => "error",
                AdaptyLogLevel.Warn => "warn",
                AdaptyLogLevel.Info => "info",
                AdaptyLogLevel.Verbose => "verbose",
                AdaptyLogLevel.Debug => "debug",
                _ => throw new Exception($"AdaptyLog.Level unknown value: {value}"),
            };

        internal static AdaptyLogLevel GetAdaptyLogLevel(this JSONNode node) =>
            GetString(node).ToAdaptyLogLevel();
        internal static AdaptyLogLevel GetAdaptyLogLevel(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAdaptyLogLevel();
        internal static AdaptyLogLevel? GetAdaptyLogLevelIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAdaptyLogLevel();

        private static AdaptyLogLevel ToAdaptyLogLevel(this string value) =>
            value switch
            {
                "error" => AdaptyLogLevel.Error,
                "warn" => AdaptyLogLevel.Warn,
                "info" => AdaptyLogLevel.Info,
                "verbose" => AdaptyLogLevel.Verbose,
                "debug" => AdaptyLogLevel.Debug,
                _ => throw new Exception($"AdaptyLog.Level unknown value: {value}"),
            };
    }
}