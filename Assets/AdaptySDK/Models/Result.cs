//
//  Result.cs
//  Adapty
//
//  Created by Aleksei Valiano on 14.01.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        internal class Result<T>
        {
            public readonly Error Error;
            public readonly T Value;

            public override string ToString()
            {
                return $"{nameof(Value)}: {Value}, " +
                       $"{nameof(Error)}: {Error}";
            }

            internal Result(T value, Error error)
            {
                Error = error;
                Value = value;
            }
        }
    }
}

