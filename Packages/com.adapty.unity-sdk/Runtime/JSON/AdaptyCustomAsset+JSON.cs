//
//  AdaptyCustomAsset+JSON.cs
//  AdaptySDK
//
//  Created by Assistant on 14.01.2025.
//

using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public partial class AdaptyCustomAsset
    {
        internal abstract JSONNode ToJSONNode();
    }

    public partial class AdaptyCustomAssetLocalImageData
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "image");
            node.Add("value", Convert.ToBase64String(Data));
            return node;
        }
    }

    public partial class AdaptyCustomAssetLocalImageAsset
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "image");
            node.Add("asset_id", AssetId);
            return node;
        }
    }

    public partial class AdaptyCustomAssetLocalImageFile
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "image");

            // Use the same platform-specific path construction as SetFallback
#if UNITY_IOS && !UNITY_EDITOR
            node.Add("path", UnityEngine.Application.dataPath + "/Raw/" + Path);
#elif UNITY_ANDROID && !UNITY_EDITOR
            node.Add("path", "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + Path);
#else
            // For editor and other platforms, use the path as-is
            node.Add("path", Path);
#endif
            return node;
        }
    }

    public partial class AdaptyCustomAssetLocalVideoAsset
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "video");
            node.Add("asset_id", AssetId);
            return node;
        }
    }

    public partial class AdaptyCustomAssetLocalVideoFile
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "video");

            // Use the same platform-specific path construction as SetFallback
#if UNITY_IOS && !UNITY_EDITOR
            node.Add("path", UnityEngine.Application.dataPath + "/Raw/" + Path);
#elif UNITY_ANDROID && !UNITY_EDITOR
            node.Add("path", "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + Path);
#else
            // For editor and other platforms, use the path as-is
            node.Add("path", Path);
#endif
            return node;
        }
    }

    public partial class AdaptyCustomAssetColor
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "color");
            node.Add("value", ColorToHex(ColorValue));
            return node;
        }

        private static string ColorToHex(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);
            var a = Mathf.RoundToInt(color.a * 255);

            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }
    }

    public partial class AdaptyCustomAssetLinearGradient
    {
        internal override JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("type", "linear-gradient");

            var values = new JSONArray();
            foreach (var time in KeyTimes())
            {
                var valueNode = new JSONObject();
                valueNode.Add("color", ColorToHex(Gradient.Evaluate(time)));
                valueNode.Add("p", time);
                values.Add(valueNode);
            }
            node.Add("values", values);

            var pointsNode = new JSONObject();
            pointsNode.Add("x0", 0.0f); // Unity gradients start at 0
            pointsNode.Add("y0", 0.0f);
            pointsNode.Add("x1", 1.0f); // Unity gradients end at 1
            pointsNode.Add("y1", 0.0f);
            node.Add("points", pointsNode);

            return node;
        }

        /// <summary>
        /// Color keys and alpha keys are independent in a Unity Gradient: they may differ in count and sit
        /// at different times. Emit a stop at every key time of either channel and let Gradient.Evaluate
        /// resolve the RGBA there, so the serialized gradient matches what Unity renders.
        /// </summary>
        private List<float> KeyTimes()
        {
            var times = new List<float>();

            foreach (var key in Gradient.colorKeys)
            {
                if (!times.Contains(key.time))
                {
                    times.Add(key.time);
                }
            }

            foreach (var key in Gradient.alphaKeys)
            {
                if (!times.Contains(key.time))
                {
                    times.Add(key.time);
                }
            }

            times.Sort();
            return times;
        }

        private static string ColorToHex(Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);
            var a = Mathf.RoundToInt(color.a * 255);

            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptyCustomAsset customAsset)
        {
            return customAsset.ToJSONNode();
        }

        internal static JSONNode ToJSONNode(this Dictionary<string, AdaptyCustomAsset> customAssets)
        {
            var array = new JSONArray();
            foreach (var kvp in customAssets)
            {
                var assetNode = kvp.Value.ToJSONNode();
                assetNode["id"] = kvp.Key;
                array.Add(assetNode);
            }
            return array;
        }
    }
}
