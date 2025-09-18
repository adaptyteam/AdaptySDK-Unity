//
//  AdaptyOnboardingsStateUpdatedParams.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    public abstract class AdaptyOnboardingsStateUpdatedParams { }

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

    public sealed class AdaptyOnboardingsMultiSelectParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly IList<AdaptyOnboardingsSelectParams> Params;

        public AdaptyOnboardingsMultiSelectParams(IList<AdaptyOnboardingsSelectParams> @params)
        {
            Params = @params;
        }

        public override string ToString() => $"{nameof(Params)}: {Params}";
    }

    public abstract class AdaptyOnboardingsInput { }

    public sealed class AdaptyOnboardingsTextInput : AdaptyOnboardingsInput
    {
        public readonly string Value;

        public AdaptyOnboardingsTextInput(string value)
        {
            Value = value;
        }
    }

    public sealed class AdaptyOnboardingsEmailInput : AdaptyOnboardingsInput
    {
        public readonly string Value;

        public AdaptyOnboardingsEmailInput(string value)
        {
            Value = value;
        }
    }

    public sealed class AdaptyOnboardingsNumberInput : AdaptyOnboardingsInput
    {
        public readonly double Value;

        public AdaptyOnboardingsNumberInput(double value)
        {
            Value = value;
        }
    }

    public sealed class AdaptyOnboardingsInputParams : AdaptyOnboardingsStateUpdatedParams
    {
        public readonly AdaptyOnboardingsInput Input;

        public AdaptyOnboardingsInputParams(AdaptyOnboardingsInput input)
        {
            Input = input;
        }
    }

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
