//
//  AdaptyOnboardingsStateUpdatedParams.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

using UnityEngine.Scripting;
using System.Collections.Generic;

namespace AdaptySDK
{
    [Preserve]
    public abstract class AdaptyOnboardingsStateUpdatedParams { }

    [Preserve]
    public sealed class AdaptyOnboardingsSelectParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly string Id;
        public readonly string Value;
        public readonly string Label;

        public AdaptyOnboardingsSelectParams(string id, string value, string label)
        {
            Id = id;
            Value = value;
            Label = label;
        }

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, {nameof(Value)}: {Value}, {nameof(Label)}: {Label}";
    }

    [Preserve]
    public sealed class AdaptyOnboardingsMultiSelectParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly IList<AdaptyOnboardingsSelectParams> Params;

        public AdaptyOnboardingsMultiSelectParams(IList<AdaptyOnboardingsSelectParams> @params)
        {
            Params = @params;
        }

        public override string ToString() => $"{nameof(Params)}: {Params}";
    }

    [Preserve]
    public abstract class AdaptyOnboardingsInput { }

    [Preserve]
    public sealed class AdaptyOnboardingsTextInput : AdaptyOnboardingsInput
    {
        public readonly string Value;

        public AdaptyOnboardingsTextInput(string value)
        {
            Value = value;
        }
    }

    [Preserve]
    public sealed class AdaptyOnboardingsEmailInput : AdaptyOnboardingsInput
    {
        public readonly string Value;

        public AdaptyOnboardingsEmailInput(string value)
        {
            Value = value;
        }
    }

    [Preserve]
    public sealed class AdaptyOnboardingsNumberInput : AdaptyOnboardingsInput
    {
        public readonly double Value;

        public AdaptyOnboardingsNumberInput(double value)
        {
            Value = value;
        }
    }

    [Preserve]
    public sealed class AdaptyOnboardingsInputParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly AdaptyOnboardingsInput Input;

        public AdaptyOnboardingsInputParams(AdaptyOnboardingsInput input)
        {
            Input = input;
        }
    }

    [Preserve]
    public sealed class AdaptyOnboardingsDatePickerParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly int? Day;
        public readonly int? Month;
        public readonly int? Year;

        public AdaptyOnboardingsDatePickerParams(int? day, int? month, int? year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public override string ToString() =>
            $"{nameof(Day)}: {Day}, {nameof(Month)}: {Month}, {nameof(Year)}: {Year}";
    }
}
