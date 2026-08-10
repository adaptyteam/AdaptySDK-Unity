using System;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The one policy behind every call back into the app from the live API — the deprecated
    /// onboarding API keeps its own copies. It replaced 57 hand-written ones, so what it does is
    /// stated once here rather than implied by each of them.
    /// </summary>
    [TestFixture]
    public class CallbackTests
    {
        [Test]
        public void TheInvocationHappens()
        {
            var called = false;

            Callbacks.InvokeSafe(() => called = true, "context");

            Assert.That(called, Is.True);
        }

        /// <summary>
        /// Safe is not swallowed: the app's exception still reaches whoever asked for the call. What
        /// the helper adds is the context — on a request that text is what the caller sees, and on
        /// an event it is what <c>OnMessage</c> logs before containing it.
        /// </summary>
        [Test]
        public void AThrowingCallbackKeepsItsContextAndItsCause()
        {
            var cause = new InvalidOperationException("the app's own bug");

            Assert.That(
                () => Callbacks.InvokeSafe(() => throw cause, "Failed to invoke Something(..)"),
                Throws
                    .InstanceOf<Exception>()
                    .With.Message.EqualTo("Failed to invoke Something(..)")
                    .And.InnerException.SameAs(cause)
            );
        }

        /// <summary>
        /// Every call site guards a callback the app may not have supplied, and passes the
        /// null-conditional in rather than a null delegate — so the lambda runs and does nothing.
        /// </summary>
        [Test]
        public void AnAbsentCallbackIsNotAnError()
        {
            Action<int> absent = null;

            Assert.That(() => Callbacks.InvokeSafe(() => absent?.Invoke(1), "context"), Throws.Nothing);
        }
    }
}
