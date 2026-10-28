#if UNITY_ANDROID
using System.Collections.Generic;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// What an iOS-only method answers on Android, where it never reaches the bridge — so it can
    /// run on a desktop host, unlike <c>TransportTests</c>.
    /// </summary>
    [TestFixture]
    public class AndroidPlatformTests
    {
        [Test]
        public void PendingStoreMessageTypesAreUnknownOnAndroid()
        {
            var called = false;
            IReadOnlyList<AdaptyStoreMessageType> types = new List<AdaptyStoreMessageType>();
            AdaptyError error = new AdaptyError(AdaptyErrorCode.Unknown, "not called", null);

            Adapty.GetPendingStoreMessageTypes((value, e) => (called, types, error) = (true, value, e));

            Assert.That(called, Is.True);
            Assert.That(types, Is.Null);
            Assert.That(error, Is.Null);
        }
    }
}
#endif
