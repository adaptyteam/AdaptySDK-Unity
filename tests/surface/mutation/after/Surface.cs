using System;

namespace AdaptySDK.Mutation
{
    public interface IMarker { }

    public abstract class Base { }

    public class LosesItsBase { }

    public sealed class BecomesSealed { }

    public class BecomesConcrete { }

    public class LosesAnInterface { }

    public class Members
    {
        public string LosesReadonly = "x";
        public string LosesStatic;
        protected string Narrows;
        protected string NarrowsFromProtectedInternal;
        public const int Renumbered = 2;

        public string LosesItsSetter { get; }

        public string NarrowsItsSetter { get; protected set; }

        public void LosesVirtual() { }

        public void LosesItsDefault(int count) { }

        public void TakesAValue(ref int value) { }

        public T Constrained<T>(T value) where T : struct => value;
    }

    public class Nested
    {
        protected class NarrowsFromProtectedInternal { }
    }
}
