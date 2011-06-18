using System;
using System.Collections.Generic;
using FubuMVC.Core.Content;
using FubuMVC.Core.UI.Scripts;
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
            var jQuery = new Script("jquery.js");
            jQuery.AddFallback("jquery-1.5.2.js", "jQuery");
            var scripts = new List<IScript>(){
                jQuery,
                new Script("script1.js"),
                new Script("script2.js")
            };

            var writer = new BasicScriptTagWriter(new StubContentRegistry());
            writer.Write(scripts).Select(x => x.ToString())
                .ShouldHaveTheSameElementsAs(
                "<script src=\"url for jquery.js\" type=\"text/javascript\"></script><script type=\"text/javascript\">window.jQuery || document.write('<script src=\"jquery-1.5.2.js\"><\\/script>')</script>",
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

        public string CssUrl(string name)
        {
            throw new NotImplementedException();
        }

        public string ScriptUrl(string name)
        {
            return "url for " + name;
        }
    }
}