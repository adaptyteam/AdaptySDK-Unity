//
//  AdaptyResult.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 14.01.2022.
//

namespace AdaptySDK
{
    internal class AdaptyResult<T>
    {
        public readonly AdaptyError Error;
        public readonly T Value;

        public override string ToString() => 
            $"{nameof(Value)}: {Value}, " +
            $"{nameof(Error)}: {Error}";

        internal AdaptyResult(T value, AdaptyError error)
        {
            Error = error;
            Value = value;
        }
    }
}