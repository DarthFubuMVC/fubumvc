using System.Linq;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Tags;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Tags
{
    [TestFixture]
    public class YellowScreenMissingAssetHandlerTester
    {
        [Test]
        public void do_not_throw_any_exception_with_no_missing_subjects()
        {
            var handler = new YellowScreenMissingAssetHandler();
            handler.BuildTagsAndRecord(new MissingAssetTagSubject[0]).Any().ShouldBeFalse();
        }

        [Test]
        public void throw_an_exception_if_there_are_any_missing_subjects()
        {
            var subjects = new[]{
                new MissingAssetTagSubject("script1.js"),
                new MissingAssetTagSubject("script2.js"),
                new MissingAssetTagSubject("script3.js")
            };

            var ex = Exception<MissingAssetsException>.ShouldBeThrownBy(() =>
            {
                var handler = new YellowScreenMissingAssetHandler();
                handler.BuildTagsAndRecord(subjects);
            });

            ex.Message.ShouldContain("script1.js");
            ex.Message.ShouldContain("script2.js");
            ex.Message.ShouldContain("script3.js");
        }
    }
}