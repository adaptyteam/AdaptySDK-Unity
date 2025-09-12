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

            var colorKeys = Gradient.colorKeys;
            var alphaKeys = Gradient.alphaKeys;

            if (colorKeys.Length != alphaKeys.Length)
            {
                throw new ArgumentException(
                    "Color keys and alpha keys arrays must have the same length"
                );
            }

            var values = new JSONArray();
            for (int i = 0; i < colorKeys.Length; i++)
            {
                var color = colorKeys[i].color;
                color.a = alphaKeys[i].alpha;

                var valueNode = new JSONObject();
                valueNode.Add("color", ColorToHex(color));
                valueNode.Add("p", colorKeys[i].time);
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
