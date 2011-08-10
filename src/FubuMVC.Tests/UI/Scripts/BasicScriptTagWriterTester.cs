using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Content;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class BasicScriptTagWriterTester
    {
        [Test]
        public void write_the_tags()
        {
            var scripts = new List<IFileDependency>(){
                new FileDependency("jquery.js"),
                new FileDependency("script1.js"),
                new FileDependency("script2.js")
            };

            var writer = new BasicScriptTagWriter(new StubContentRegistry());
            writer.Write(scripts.Select(x => x.Name)).Select(x => x.ToString())
                .ShouldHaveTheSameElementsAs(
                "<script src=\"url for jquery.js\" type=\"text/javascript\"></script>", 
                "<script src=\"url for script1.js\" type=\"text/javascript\"></script>", 
                "<script src=\"url for script2.js\" type=\"text/javascript\"></script>" 
            
            );
        }
    }

    public class StubContentRegistry : IContentRegistry
    {
        public string ImageUrl(string name)
        {
            throw new NotImplementedException();
        }

        public string CssUrl(string name, bool optional)
        {
            throw new NotImplementedException();
        }

        public string ScriptUrl(string name, bool optional)
        {
            return "url for " + name;
        }
    }
}