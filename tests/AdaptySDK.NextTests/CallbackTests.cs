using System;
using NUnit.Framework;
#if !UNITY_IOS && !UNITY_ANDROID
using AdaptySDK.Noop;
#endif

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The one policy behind every call back into the app. Requests own it in <c>Request</c> and
    /// events go through the helper, so what it does is stated once here rather than implied by
    /// each of the call sites it replaced.
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

#if !UNITY_IOS && !UNITY_ANDROID
        /// <summary>
        /// The name in the diagnostic is the compiler's, not a copy. The two tests below are what
        /// makes that true rather than merely intended: nothing else ties the text a request throws
        /// to the method the app actually called.
        /// </summary>
        /// <remarks>
        /// They drive the no-op bridge, which answers synchronously, so the app's exception comes
        /// back out of the public call itself.
        /// </remarks>
        [TearDown]
        public void ClearTheBridge() => AdaptyNoop.Handler = null;

        /// <summary>
        /// A typed request. <c>GetOnboarding</c> is deliberately the subject: it is the one call
        /// that used to hand the app's exception on raw, so this is the regression as well as the
        /// guard.
        /// </summary>
        [Test]
        public void ATypedRequestNamesTheMethodTheAppCalled()
        {
            AdaptyNoop.Handler = (method, request) => "{\"success\":null}";
            var cause = new InvalidOperationException("the app's own bug");

            Assert.That(
                () => Adapty.GetOnboarding("placement", (onboarding, error) => throw cause),
                Throws
                    .InstanceOf<Exception>()
                    .With.Message.EqualTo("Failed to invoke completionHandler in GetOnboarding(..)")
                    .And.InnerException.SameAs(cause)
            );
        }

        /// <summary>
        /// An error-only request, which reaches the transport through a second hop. The name has to
        /// survive it — without the explicit hand-off the message would read <c>SendVoid</c>.
        /// </summary>
        [Test]
        public void AnErrorOnlyRequestNamesTheMethodAndNotTheHelper()
        {
            AdaptyNoop.Handler = (method, request) => "{\"success\":true}";
            var cause = new InvalidOperationException("the app's own bug");

            Assert.That(
                () => Adapty.Logout(error => throw cause),
                Throws
                    .InstanceOf<Exception>()
                    .With.Message.EqualTo("Failed to invoke completionHandler in Logout(..)")
                    .And.InnerException.SameAs(cause)
            );
        }
#endif
    }
}
