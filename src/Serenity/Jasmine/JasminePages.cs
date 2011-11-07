using HtmlTags;
using System.Collections.Generic;

namespace Serenity.Jasmine
{
    public class JasminePages
    {
        private readonly SpecificationGraph _specifications;

        public JasminePages(SpecificationGraph specifications)
        {
            _specifications = specifications;
        }

        public HtmlDocument Home()
        {
            var document = new HtmlDocument{
                Title = "Serenity Jasmine Tester"
            };

            document.Add("h1").Text("Serenity Jasmine Tester");

            _specifications.AllSpecifications.Each(x =>
            {
                document.Add("p").Text(x.File.FullPath);
            });

            return document;
        }
    }
}