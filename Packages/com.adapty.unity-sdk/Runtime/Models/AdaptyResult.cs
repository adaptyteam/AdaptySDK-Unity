using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
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