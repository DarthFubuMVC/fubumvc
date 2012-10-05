using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;
using FubuMVC.Core.UI;
using FubuTestingSupport;

namespace FubuMVC.Core.Assets.Testing
{
    [TestFixture]
    public class AssetFilePageExtensionsTester
    {
        private IFubuPage thePage;
        private IAssetRequirements theRequirements;
        private IAssetTagWriter theWriter;
        private TagList theTagList;

        [SetUp]
        public void SetUp()
        {
            thePage = MockRepository.GenerateMock<IFubuPage>();
            theRequirements = MockRepository.GenerateMock<IAssetRequirements>();

            thePage.Stub(x => x.Get<IAssetRequirements>()).Return(theRequirements);
            
            theWriter = MockRepository.GenerateMock<IAssetTagWriter>();
            thePage.Stub(x => x.Get<IAssetTagWriter>()).Return(theWriter);

            theTagList = new TagList(new HtmlTag[0]);
        }

        [Test]
        public void register_assets()
        {
            thePage.Asset("a.css", "b.css", "c.css", "d.js");
            theRequirements.AssertWasCalled(x => x.Require("a.css", "b.css", "c.css", "d.js"));
        }

        [Test]
        public void optional_asset()
        {
            thePage.OptionalAsset("a.js", "b.css");

            theRequirements.AssertWasCalled(x => x.UseAssetIfExists("a.js", "b.css"));
        }

        [Test]
        public void write_css_tags_no_extra_required()
        {
            theWriter.Stub(x => x.WriteTags(MimeType.Css)).Return(theTagList);
            thePage.WriteCssTags().ShouldBeTheSameAs(theTagList);
        }


        [Test]
        public void write_css_tags_with_css_names_too_should_register_the_names()
        {
            theWriter.Stub(x => x.WriteTags(MimeType.Css)).Return(theTagList);

            thePage.WriteCssTags("a.css", "b.css").ShouldBeTheSameAs(theTagList); 

            theRequirements.AssertWasCalled(x => x.Require("a.css", "b.css"));
        }

        [Test]
        public void write_script_tags_no_extra_required()
        {
            theWriter.Stub(x => x.WriteTags(MimeType.Javascript)).Return(theTagList);
            thePage.WriteScriptTags().ShouldBeTheSameAs(theTagList);
        }


        [Test]
        public void write_script_tags_with_css_names_too_should_register_the_names()
        {
            theWriter.Stub(x => x.WriteTags(MimeType.Css)).Return(theTagList);

            thePage.WriteCssTags("a.js", "b.js").ShouldBeTheSameAs(theTagList);

            theRequirements.AssertWasCalled(x => x.Require("a.js", "b.js"));
        }


        [Test]
        public void write_asset_tags_no_extra_required()
        {
            theWriter.Stub(x => x.WriteAllTags()).Return(theTagList);
            thePage.WriteAssetTags().ShouldBeTheSameAs(theTagList);
        }


        [Test]
        public void write_asset_tags_with_asset_names_too_should_register_the_names()
        {
            theWriter.Stub(x => x.WriteAllTags()).Return(theTagList);

            thePage.WriteAssetTags("a.js", "b.css").ShouldBeTheSameAs(theTagList);

            theRequirements.AssertWasCalled(x => x.Require("a.js", "b.css"));
        }
    }
}