using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuMVC.Tests.Assets.Tags
{
    [TestFixture]
    public class AssetTagBuilderTester : InteractionContext<AssetTagBuilder>
    {
       

        [Test]
        public void when_writing_a_script_tag()
        {
            MockFor<IMissingAssetHandler>().Stub(x => x.BuildTagsAndRecord(null))
                .Return(new HtmlTag[0])
                .IgnoreArguments();

            MockFor<IAssetUrls>().Stub(x => x.UrlForAsset(AssetFolder.scripts, "script.js"))
                .Return("http://myapp/_content/scripts/script.js");

            var file = new AssetFile("script.js", AssetFolder.scripts);
            var plan = AssetTagPlan.For(MimeType.Javascript, file);

            var tag = ClassUnderTest.Build(plan).Single();

            tag.ToString().ShouldEqual("<script type=\"text/javascript\" src=\"http://myapp/_content/scripts/script.js\"></script>");
        
        }

        [Test]
        public void when_writing_a_style_tag()
        {
            MockFor<IMissingAssetHandler>().Stub(x => x.BuildTagsAndRecord(null))
                .Return(new HtmlTag[0])
                .IgnoreArguments();

            MockFor<IAssetUrls>().Stub(x => x.UrlForAsset(AssetFolder.styles, "main.css"))
                .Return("http://myapp/_content/styles/main.css");

            var file = new AssetFile("main.css", AssetFolder.styles);
            var plan = AssetTagPlan.For(MimeType.Css, file);

            var tag = ClassUnderTest.Build(plan).Single();

            tag.ToString().ShouldEqual("<link href=\"http://myapp/_content/styles/main.css\" rel=\"stylesheet\" type=\"text/css\" />");
        
        }

        [Test]
        public void when_writing_a_tag_plan_with_missing_assets()
        {
            var file = new AssetFile("main.css", AssetFolder.styles);
            var missing1 = new MissingAssetTagSubject("main.css");
            var missing2 = new MissingAssetTagSubject("other.css");
            var plan = AssetTagPlan.For(MimeType.Css, file, missing1, missing2);

            var handler = new StubMissingAssetHandler();
            Services.Inject<IMissingAssetHandler>(handler);

            var allTags = ClassUnderTest.Build(plan);
            allTags.Count().ShouldEqual(3);

            handler.Subjects.ShouldHaveTheSameElementsAs(missing1, missing2);

            allTags.Contains(handler.Tags.First());
            allTags.Contains(handler.Tags.Last());
        }


    }

    public class StubMissingAssetHandler : IMissingAssetHandler
    {
        public readonly List<HtmlTag> Tags = new List<HtmlTag>();
        public readonly List<MissingAssetTagSubject> Subjects = new List<MissingAssetTagSubject>();

        public IEnumerable<HtmlTag> BuildTagsAndRecord(IEnumerable<MissingAssetTagSubject> subjects)
        {
            Subjects.AddRange(subjects);
            Subjects.Each(s => Tags.Add(new HtmlTag("div")));

            return Tags;
        }
    }
}