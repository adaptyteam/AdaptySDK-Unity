//
//  SimpleJSON+Extensions.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;
using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        private static JSONNode GetJSONNodeIfPresent(this JSONNode node, string aKey)
        {
            if (node is null) throw new Exception("JSON node is Null");
            if (!node.IsObject) throw new Exception("JSON node is not Object");
            if (!node.HasKey(aKey)) return null;
            var valueNode = node[aKey];
            if (valueNode.IsNull) return null;
            return valueNode;
        }

        private static JSONNode GetJSONNode(this JSONNode node, string aKey)
        {
            if (node is null) throw new Exception("JSON node is Null");
            if (!node.IsObject) throw new Exception("JSON node is not Object");
            if (!node.HasKey(aKey)) throw new Exception($"Object has not key: {aKey}");
            return node[aKey];
        }

        internal static string GetString(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsString) throw new Exception($"Value by key: {aKey} is not String");
            return valueNode.Value;
        }

        internal static string GetStringIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsString) throw new Exception($"Value by key: {aKey} is not String");
            return valueNode.Value;
        }

        internal static IList<string> GetStringListIfPresent(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<string>();
            foreach (var item in array.Children)
            {
                if (!item.IsString) throw new Exception($"Value by index: {result.Count} is not String");
                result.Add(item.Value);
            }
            return result;
        }

        internal static double GetDouble(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsNumber) throw new Exception($"Value by key: {aKey} is not Number");
            return valueNode.AsDouble;
        }

        internal static double? GetDoubleIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsNumber) throw new Exception($"Value by key: {aKey} is not Number");
            return valueNode.AsDouble;
        }

        internal static int GetInteger(this JSONNode node, string aKey) => (int)node.GetDouble(aKey);

        internal static int? GetIntegerIfPresent(this JSONNode node, string aKey) => (int)node.GetDoubleIfPresent(aKey);

        internal static float GetFloat(this JSONNode node, string aKey) => (float)node.GetDouble(aKey);

        internal static float? GetFloatIfPresent(this JSONNode node, string aKey) => (float)node.GetDoubleIfPresent(aKey);

        internal static long GetLong(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsNumber) throw new Exception($"Value by key: {aKey} is not Number");
            return valueNode.AsLong;
        }

        internal static long? GetLongIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsNumber) throw new Exception($"Value by key: {aKey} is not Number");
            return valueNode.AsLong;
        }

        internal static bool GetBoolean(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsBoolean) throw new Exception($"Value by key: {aKey} is not Bool");
            return valueNode.AsBool;
        }

        internal static bool? GetBooleanIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsBoolean) throw new Exception($"Value by key: {aKey} is not Bool");
            return valueNode.AsBool;
        }

        internal static JSONArray GetArray(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsArray) throw new Exception($"Value by key: {aKey} is not Array");
            return valueNode.AsArray;
        }

        internal static JSONArray GetArrayIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsArray) throw new Exception($"Value by key: {aKey} is not Array");
            return valueNode.AsArray;
        }

        internal static JSONObject GetObject(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNode(node, aKey);
            if (!valueNode.IsObject) throw new Exception($"Value by key: {aKey} is not Object");
            return valueNode.AsObject;
        }

        internal static JSONObject GetObjectIfPresent(this JSONNode node, string aKey)
        {
            JSONNode valueNode = GetJSONNodeIfPresent(node, aKey);
            if (valueNode is null) return null;
            if (!valueNode.IsObject) throw new Exception($"Value by key: {aKey} is not Object");
            return valueNode.AsObject;
        }
    }
}