//
//  SimpleJSON+Collections.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;
using System;

namespace AdaptySDK.SimpleJSON
{
    public static partial class SimpleJSONCollections
    {
        public static IDictionary<string, dynamic> GetDictionary(this JSONNode node) => node.AsObject.ToDictionary();
        public static IDictionary<string, dynamic> GetDictionary(this JSONNode node, string aKey) => node.GetObject(aKey).ToDictionary();
        public static IDictionary<string, dynamic> GetDictionaryIfPresent(this JSONNode node, string aKey) => node.GetObjectIfPresent(aKey).ToDictionary();
        public static IDictionary<string, dynamic> GetDictionaryIfPresent(this JSONNode node) => node is null || node.IsNull ? null : node.AsObject.ToDictionary();

        public static IList<dynamic> GetList(this JSONNode node) => node.AsArray.ToList();
        public static IList<dynamic> GetList(this JSONNode node, string aKey) => node.GetArray(aKey).ToList();
        public static IList<dynamic> GetListIfPresent(this JSONNode node) => node is null || node.IsNull ? null : node.AsArray.ToList();
        public static IList<dynamic> GetListIfPresent(this JSONNode node, string aKey) => node.GetArrayIfPresent(aKey).ToList();

        private static IDictionary<string, dynamic> ToDictionary(this JSONObject obj)
        {
            if (obj == null) return null;

            var result = new Dictionary<string, dynamic>();

            foreach (var item in obj)
            {
                switch (item.Value.Tag)
                {
                    case JSONNodeType.Array:
                        result.Add(item.Key, ToList(item.Value.AsArray));
                        break;
                    case JSONNodeType.Object:
                        result.Add(item.Key, ToDictionary(item.Value.AsObject));
                        break;
                    case JSONNodeType.Boolean:
                        result.Add(item.Key, item.Value.AsBool);
                        break;
                    case JSONNodeType.String:
                        result.Add(item.Key, item.Value.Value);
                        break;
                    case JSONNodeType.Number:
                        result.Add(item.Key, item.Value.AsDouble);
                        break;
                    case JSONNodeType.NullValue:
                        result.Add(item.Key, null);
                        break;
                }
            }
            return result;
        }

        private static IList<dynamic> ToList(this JSONArray obj)
        {
            if (obj == null) return null;

            var result = new List<dynamic>();

            foreach (var item in obj.Children)
            {
                switch (item.Tag)
                {
                    case JSONNodeType.Array:
                        result.Add(ToList(item.AsArray));
                        break;
                    case JSONNodeType.Object:
                        result.Add(ToDictionary(item.AsObject));
                        break;
                    case JSONNodeType.Boolean:
                        result.Add(item.AsBool);
                        break;
                    case JSONNodeType.String:
                        result.Add(item.Value);
                        break;
                    case JSONNodeType.Number:
                        result.Add(item.AsDouble);
                        break;
                    case JSONNodeType.NullValue:
                        result.Add(null);
                        break;
                }
            }
            return result;
        }

        public static JSONObject ToJSONObject(this IDictionary<string, dynamic> obj)
        {
            var result = new JSONObject();

            foreach (var item in obj)
            {
                if (item.Value is JSONNode)
                {
                    result.Add(item.Key, item.Value as JSONNode);
                }
                else if (item.Value is Dictionary<string, dynamic>)
                {
                    result.Add(item.Key, ToJSONObject(item.Value as Dictionary<string, dynamic>));
                }
                else if (item.Value is IList<dynamic>)
                {
                    result.Add(item.Key, ToJSONArray(item.Value as IList<dynamic>));
                }
                else if (item.Value is null)
                {
                    result.Add(item.Key, JSONNull.CreateOrGet());
                }
                else if (item.Value is string)
                {
                    result.Add(item.Key, new JSONString(item.Value as string));
                }
                else if (item.Value is int || item.Value is uint
                || item.Value is long || item.Value is ulong
                || item.Value is short || item.Value is ushort
                || item.Value is sbyte || item.Value is byte)
                {
                    result.Add(item.Key, new JSONNumber(Convert.ToInt64((object)item.Value)));
                }
                else if (item.Value is float || item.Value is double || item.Value is decimal)
                {
                    result.Add(item.Key, new JSONNumber(Convert.ToDouble((object)item.Value)));
                }
            }

            return result;
        }

        public static JSONArray ToJSONArray(this IList<dynamic> obj)
        {
            var result = new JSONArray();

            foreach (var item in obj)
            {
                if (item is JSONNode)
                {
                    result.Add(item as JSONNode);
                }
                else if (item is Dictionary<string, dynamic>)
                {
                    result.Add(ToJSONObject(item as Dictionary<string, dynamic>));
                }
                else if (item is IList<dynamic>)
                {
                    result.Add(ToJSONArray(item as IList<dynamic>));
                }
                else if (item is null)
                {
                    result.Add(JSONNull.CreateOrGet());
                }
                else if (item is string)
                {
                    result.Add(new JSONString(item as string));
                }
                else if (item is int || item is uint
                || item is long || item is ulong
                || item is short || item is ushort
                || item is sbyte || item is byte)
                {
                    result.Add(new JSONNumber(Convert.ToInt64((object)item)));
                }
                else if (item is float || item is double || item is decimal)
                {
                    result.Add(new JSONNumber(Convert.ToDouble((object)item)));
                }
            }
            return result;
        }
    }
}