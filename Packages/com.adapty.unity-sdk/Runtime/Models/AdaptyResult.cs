using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
    internal class AdaptyResult<T>
    {
        public readonly AdaptyError Error;
        public readonly T Value;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
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