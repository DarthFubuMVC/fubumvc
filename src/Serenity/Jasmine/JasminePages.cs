using System;
using HtmlTags;
using System.Collections.Generic;

namespace Serenity.Jasmine
{
    public class JasminePages
    {
        private const string Title = "Serenity Jasmine Runner";
        private readonly SpecificationGraph _specifications;

        public JasminePages(SpecificationGraph specifications)
        {
            _specifications = specifications;
        }

        public HtmlDocument Home()
        {
            var document = new HtmlDocument{
                Title = Title
            };

            document.Add("h1").Text(Title);

            _specifications.AllSpecifications.Each(x =>
            {
                document.Add("p").Text(x.File.FullPath);
            });

            return document;
        }

        public HtmlDocument AllSpecs()
        {
            throw new NotImplementedException();
        }

        public HtmlDocument Spec(SpecPath path)
        {
            var document = new HtmlDocument(){
                Title = Title + ":  " + path.Parts.Join("/")
            };

            document.Add("h1").Text(path.Parts.Join("/"));

            return document;
        }
    }
}