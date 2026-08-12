using System;
using System.IO;
using System.Runtime.CompilerServices;
using AdaptySDK.Editor;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The edit that decides whether a Kids Category build ships IDFA. It is text surgery on
    /// project.pbxproj, so the only meaningful evidence is a project Unity and External Dependency
    /// Manager really produced — a hand-written sample would test the shape this suite imagines.
    /// </summary>
    /// <remarks>
    /// The fixtures are a contiguous excerpt of one: lines 2865-2956 of the Unity-iPhone project
    /// this repository generates today - Unity 6000.4.5f1, External Dependency Manager 1.2.188,
    /// AdaptySDK-iOS pinned at 4.0.2 - from the XCConfigurationList section through the end of the
    /// package sections. <c>.applied.pbxproj</c> is that build untouched, trait and all;
    /// <c>edm-package-reference.pbxproj</c> is what External Dependency Manager wrote, recovered by
    /// deleting the three lines the postprocessor had inserted. Round-tripping between them is what
    /// pins the format down to the tab.
    ///
    /// Regenerate both by building for iOS with ADAPTY_KIDS_MODE and taking the same excerpt; the
    /// object ids differ every build, so no test may name one.
    /// </remarks>
    [TestFixture]
    public class KidsModeTraitTests
    {
        private const string Section = "/* Begin XCRemoteSwiftPackageReference section */";

        [Test]
        public void TheTraitLandsExactlyWhereTheRealBuildPutIt()
        {
            Assert.That(
                AdaptyIOSKidsModeTrait.Enable(Fixture("edm-package-reference.pbxproj")),
                Is.EqualTo(Fixture("edm-package-reference.applied.pbxproj"))
            );
        }

        /// <summary>
        /// The postprocessor runs on every build, including the ones after the first.
        /// </summary>
        [Test]
        public void ASecondPassChangesNothing()
        {
            var applied = Fixture("edm-package-reference.applied.pbxproj");

            Assert.That(AdaptyIOSKidsModeTrait.Enable(applied), Is.EqualTo(applied));
        }

        /// <summary>
        /// The reference is found by its <c>isa</c>, not by the brace nearest the URL. Left to the
        /// brace alone, a URL appearing anywhere else in the project would aim the insertion at
        /// whatever object happened to enclose it — and the build would report Kids Mode while
        /// still linking IDFA, which nothing downstream would catch.
        /// </summary>
        [Test]
        public void AUrlOutsideAPackageReferenceIsNotMistakenForOne()
        {
            // An object that mentions the URL and is not a package reference. It sits before the
            // real one, so a search that stops at the first occurrence stops here.
            const string Decoy =
                "\t\tAAAA1111 /* PBXBuildFile */ = {\n"
                + "\t\t\tisa = PBXBuildFile;\n"
                + "\t\t\tcomment = \"see https://github.com/adaptyteam/AdaptySDK-iOS.git\";\n"
                + "\t\t};\n"
                + Section;

            Assert.That(
                AdaptyIOSKidsModeTrait.Enable(
                    Fixture("edm-package-reference.pbxproj").Replace(Section, Decoy)
                ),
                Is.EqualTo(Fixture("edm-package-reference.applied.pbxproj").Replace(Section, Decoy))
            );
        }

        [Test]
        public void AProjectWithoutTheReferenceFails()
        {
            var project = Fixture("edm-package-reference.pbxproj")
                .Replace(AdaptyIOSKidsModeTrait.PackageUrl, "someoneelse/OtherSDK");

            Assert.That(
                () => AdaptyIOSKidsModeTrait.Enable(project),
                Throws.InvalidOperationException.With.Message.Contains("no AdaptySDK-iOS")
            );
        }

        /// <summary>
        /// Two references are as unsafe as none: the trait would go on whichever came first.
        /// </summary>
        [Test]
        public void TwoReferencesFailRatherThanPickOne()
        {
            // Located by the section markers rather than by the object's id, which is generated
            // afresh on every build.
            var project = Fixture("edm-package-reference.pbxproj");
            var open = project.IndexOf(Section, StringComparison.Ordinal) + Section.Length;
            var close = project.IndexOf("/* End XCRemoteSwiftPackageReference", StringComparison.Ordinal);
            var reference = project.Substring(open, close - open);

            Assert.That(
                () => AdaptyIOSKidsModeTrait.Enable(project.Replace(reference, reference + reference)),
                Throws.InvalidOperationException.With.Message.Contains("2 AdaptySDK-iOS")
            );
        }

        /// <summary>
        /// A traits block that is not ours is not merged into — it is reported, because guessing
        /// the merge is how a trait someone added by hand gets dropped.
        /// </summary>
        [Test]
        public void AForeignTraitsBlockIsReported()
        {
            var project = Fixture("edm-package-reference.applied.pbxproj")
                .Replace(AdaptyIOSKidsModeTrait.Trait, "SomeOtherTrait");

            Assert.That(
                () => AdaptyIOSKidsModeTrait.Enable(project),
                Throws.InvalidOperationException.With.Message.Contains("already declares a traits block")
            );
        }

        private static string Fixture(string name) =>
            File.ReadAllText(
                Path.Combine(
                    Path.GetDirectoryName(SourcePath()),
                    "..",
                    "shared",
                    "Fixtures",
                    "pbxproj",
                    name
                )
            );

        private static string SourcePath([CallerFilePath] string path = null) => path;
    }
}
