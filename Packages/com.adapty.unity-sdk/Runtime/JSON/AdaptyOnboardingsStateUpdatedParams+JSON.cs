//
//  AdaptyOnboardingsStateUpdatedParams+JSON.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

using System.Collections.Generic;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        // action: { element_id, element_type, value }
        internal static AdaptySDK.AdaptyOnboardingsStateUpdatedParams GetOnboardingsStateUpdatedParams(
            this JSONNode node,
            string aKey
        )
        {
            var actionObj = JSONNodeExtensions.GetObject(node, aKey);
            var elementType = actionObj.GetString("element_type");
            switch (elementType)
            {
                case "select":
                {
                    var valueObj = JSONNodeExtensions.GetObject(actionObj, "value");
                    var id = valueObj.GetString("id");
                    var value = valueObj.GetString("value");
                    var label = valueObj.GetString("label");
                    return new AdaptySDK.AdaptyOnboardingsSelectParams(id, value, label);
                }
                case "multi_select":
                {
                    var array = JSONNodeExtensions.GetArray(actionObj, "value");
                    var list = new List<AdaptySDK.AdaptyOnboardingsSelectParams>(array.Count);
                    foreach (var item in array.Children)
                    {
                        var id = item.GetString("id");
                        var value = item.GetString("value");
                        var label = item.GetString("label");
                        list.Add(new AdaptySDK.AdaptyOnboardingsSelectParams(id, value, label));
                    }
                    return new AdaptySDK.AdaptyOnboardingsMultiSelectParams(list);
                }
                case "input":
                {
                    var valueObj = JSONNodeExtensions.GetObject(actionObj, "value");
                    var type = valueObj.GetString("type");
                    switch (type)
                    {
                        case "text":
                            return new AdaptySDK.AdaptyOnboardingsInputParams(
                                new AdaptySDK.AdaptyOnboardingsTextInput(
                                    valueObj.GetString("value")
                                )
                            );
                        case "email":
                            return new AdaptySDK.AdaptyOnboardingsInputParams(
                                new AdaptySDK.AdaptyOnboardingsEmailInput(
                                    valueObj.GetString("value")
                                )
                            );
                        case "number":
                            return new AdaptySDK.AdaptyOnboardingsInputParams(
                                new AdaptySDK.AdaptyOnboardingsNumberInput(
                                    valueObj.GetDouble("value")
                                )
                            );
                        default:
                            return null;
                    }
                }
                case "date_picker":
                {
                    var valueObj = JSONNodeExtensions.GetObject(actionObj, "value");
                    int? day = valueObj.GetIntegerIfPresent("day");
                    int? month = valueObj.GetIntegerIfPresent("month");
                    int? year = valueObj.GetIntegerIfPresent("year");
                    return new AdaptySDK.AdaptyOnboardingsDatePickerParams(day, month, year);
                }
                default:
                    return null;
            }
        }
    }
}
