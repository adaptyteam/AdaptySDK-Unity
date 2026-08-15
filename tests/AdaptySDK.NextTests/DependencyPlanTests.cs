using System.Linq;
using AdaptySDK.Editor;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// What <b>Adapty SDK &gt; Install Dependencies</b> decides to install, from what the project
    /// already has.
    /// </summary>
    /// <remarks>
    /// The v3 to v4 upgrade is the case worth pinning: v3 declared External Dependency Manager
    /// 1.2.187, v4 needs 1.2.188, and the difference is invisible until an iOS build resolves
    /// against the wrong Xcode project path. Presence alone used to be the whole test, so an
    /// upgraded project was told it had everything.
    /// </remarks>
    [TestFixture]
    public class DependencyPlanTests
    {
        private const string Edm = AdaptyDependencyPlan.EdmId + "@" + AdaptyDependencyPlan.EdmVersion;
        private const string Newtonsoft =
            AdaptyDependencyPlan.NewtonsoftId + "@" + AdaptyDependencyPlan.NewtonsoftVersion;

        [Test]
        public void AnAbsentDependencyManagerIsInstalled() =>
            Assert.That(Plan(AdaptyEdmSource.None), Is.EqualTo(new[] { Edm }));

        [TestCase("1.2.187", TestName = "the version v3 declared")]
        [TestCase("1.2.0", TestName = "older still")]
        [TestCase("1.1.999", TestName = "an older minor")]
        public void AnOlderDependencyManagerIsUpgraded(string installed) =>
            Assert.That(Plan(AdaptyEdmSource.Package, installed), Is.EqualTo(new[] { Edm }));

        [TestCase("1.2.188", TestName = "exactly what the SDK asks for")]
        [TestCase("1.2.189", TestName = "newer")]
        [TestCase("1.3.0", TestName = "a newer minor")]
        public void ADependencyManagerNewEnoughIsLeftAlone(string installed)
        {
            Assert.That(Plan(AdaptyEdmSource.Package, installed), Is.Empty);
            Assert.That(AdaptyDependencyPlan.EdmCaution(AdaptyEdmSource.Package, installed), Is.Null);
        }

        /// <summary>
        /// Google ships its own <c>.unitypackage</c> under <c>Assets/</c>, where Package Manager
        /// describes nothing. Installing the package over it would leave two copies, so the plan
        /// says so instead of acting.
        /// </summary>
        [Test]
        public void ADependencyManagerOutsidePackageManagerIsReportedRatherThanReplaced()
        {
            Assert.That(Plan(AdaptyEdmSource.Unmanaged), Is.Empty);

            Assert.That(
                AdaptyDependencyPlan.EdmCaution(AdaptyEdmSource.Unmanaged, null),
                Does.Contain(AdaptyDependencyPlan.EdmVersion).And.Contain("cannot be read")
            );
        }

        /// <summary>
        /// A version string that will not parse is not evidence of anything, and least of all a
        /// reason to install over whatever is there.
        /// </summary>
        [TestCase("")]
        [TestCase("1.2.188-preview.1")]
        [TestCase("latest")]
        public void AVersionThatCannotBeComparedIsReported(string installed)
        {
            Assert.That(Plan(AdaptyEdmSource.Package, installed), Is.Empty);
            Assert.That(
                AdaptyDependencyPlan.EdmCaution(AdaptyEdmSource.Package, installed),
                Does.Contain(AdaptyDependencyPlan.EdmVersion)
            );
        }

        [Test]
        public void NewtonsoftIsInstalledWhenTheProjectDoesNotHaveIt() =>
            Assert.That(
                AdaptyDependencyPlan.Missing(false, AdaptyEdmSource.Package, "1.2.188"),
                Is.EqualTo(new[] { Newtonsoft })
            );

        [Test]
        public void BothAreInstalledWhenTheProjectHasNeither() =>
            Assert.That(
                AdaptyDependencyPlan.Missing(false, AdaptyEdmSource.None, null),
                Is.EqualTo(new[] { Newtonsoft, Edm })
            );

        /// <summary>
        /// Newtonsoft carries no version check of its own: the SDK assembly is gated on the package
        /// being there, and any version of it that Package Manager resolves makes the SDK compile.
        /// </summary>
        [Test]
        public void APresentNewtonsoftIsLeftAtWhateverVersionItIs() =>
            Assert.That(
                AdaptyDependencyPlan.Missing(true, AdaptyEdmSource.Package, "1.2.188"),
                Is.Empty
            );

        private static string[] Plan(AdaptyEdmSource edm, string version = null) =>
            AdaptyDependencyPlan.Missing(true, edm, version).ToArray();
    }
}
