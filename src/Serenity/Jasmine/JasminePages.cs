using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.UI;
using HtmlTags;

namespace Serenity.Jasmine
{
    public class JasminePages
    {
        private const string Title = "Serenity Jasmine Runner";
        private static readonly string _header;
        private readonly SpecHierarchyBuilder _builder;
        private readonly FubuHtmlDocument _document;
        private readonly SpecAssetRequirements _requirements;
        private readonly SpecificationGraph _specifications;

        static JasminePages()
        {
            _header =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (JasminePages), "Header.htm").
                    ReadAllText();
        }

        public JasminePages(SpecificationGraph specifications, SpecHierarchyBuilder builder,
                            SpecAssetRequirements requirements, FubuHtmlDocument document)
        {
            _specifications = specifications;
            _builder = builder;
            _requirements = requirements;
            _document = document;


            _document.Body.Append(new HtmlTag("div").Text(_header).Encoded(false));
        }

        public HtmlDocument Home()
        {
            _document.Title = Title;

            //_requirements.WriteBasicAssetsInto(_document);

            var tag = _builder.CompleteHierarchy();
            _document.Add(tag);

            return _document;
        }

        public HtmlDocument AllSpecs()
        {
            _document.Title = Title + ":  All Specifications";

            writeNode(_specifications);

            return _document;
        }

        public HtmlDocument Spec(SpecPath path)
        {
            _document.Title = Title + ":  " + path.Parts.Join("/");

            var node = _specifications.FindSpecNode(path);

            writeNode(node);

            return _document;
        }

        private void writeNode(ISpecNode node)
        {
            var tag = _builder.BuildInPlaceHierarchyFor(node);


            _document.Add(tag);
            _document.Add("hr");

            _requirements.WriteAssetsInto(_document, node.AllSpecifications);

            var fileSystem = new FileSystem();
            node.AllSpecifications.SelectMany(x => x.HtmlFiles).Each(file =>
            {
                var html = fileSystem.ReadStringFromFile(file.FullPath);
                _document.Add("div").Text(html).Encoded(false);
            });
        }
    }
}