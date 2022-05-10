using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum PeriodUnit
        {
            Day = 0,
            Week = 1,
            Month = 2,
            Year = 3,
            Unknown = 4
        }

        public class Period
        {

            public readonly PeriodUnit Unit;
            public readonly long NumberOfUnits;

            internal Period(JSONNode response)
            {
                Unit = PeriodUnitFromJSON(response["unit"]);
                NumberOfUnits = response["number_of_units"];
                if (NumberOfUnits == 0) NumberOfUnits = response["numberOfUnits"];
            }

            public override string ToString()
            {
                return $"{nameof(Unit)}: {Unit}, " +
                       $"{nameof(NumberOfUnits)}: {NumberOfUnits}";
            }
        }

        public static Period PeriodFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try { 
                return new Period(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding Period: {e} source: {response}");
                return null;
            }
        }

        public static PeriodUnit PeriodUnitFromJSON(JSONNode response)
        {
           
           if (response.IsNumber )
            {
                return PeriodUnitFromInt(response);
            }
            else if (response.IsString )
            {
                return PeriodUnitFromString(response);
            } else
            {
                return PeriodUnit.Unknown;
            }
        }

        public static PeriodUnit PeriodUnitFromString(string value)
        {
            if (value == null) return PeriodUnit.Unknown;
            switch (value)
            {
                case "day":
                    return PeriodUnit.Day;
                case "week":
                    return PeriodUnit.Week;
                case "month":
                    return PeriodUnit.Month;
                case "year":
                    return PeriodUnit.Year;
                default:
                    return PeriodUnit.Unknown;
            }
        }

        public static PeriodUnit PeriodUnitFromInt(int value)
        {
            if (value < 0 || value > 4)
            {
                return PeriodUnit.Unknown;
            } else
            {
                return (PeriodUnit)value;
            }
        }
    }
}
