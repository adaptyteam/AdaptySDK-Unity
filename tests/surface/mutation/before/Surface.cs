using System;

namespace AdaptySDK.Mutation
{
    public interface IMarker { }

    public abstract class Base { }

    // One mutation per type, so a removed category costs exactly one line of the diff. Two on the
    // same type would mask each other: the line still differs when only one of them is detected.

    public class LosesItsBase : Base { }

    public class BecomesSealed { }

    public abstract class BecomesConcrete { }

    public class LosesAnInterface : IMarker { }

    public class Members
    {
        public readonly string LosesReadonly = "x";
        public static string LosesStatic;
        public string Narrows;
        protected internal string NarrowsFromProtectedInternal;
        public const int Renumbered = 1;

        public string LosesItsSetter { get; set; }

        public string NarrowsItsSetter { get; set; }

        public virtual void LosesVirtual() { }

        public void LosesItsDefault(int count = 3) { }

        public void TakesAValue(int value) { }

        public T Constrained<T>(T value) where T : class => value;
    }

    public class Nested
    {
        protected internal class NarrowsFromProtectedInternal { }
    }
}
