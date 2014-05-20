using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.UI;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using Process = System.Diagnostics.Process;

namespace TestHarnessApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new TestHarnessApplication().BuildApplication().RunEmbeddedWithAutoPort())
            {
                Process.Start(server.BaseAddress);

                Console.WriteLine("Press any key to quit");
                Console.ReadLine();
            }

        }
    }

    public class TestHarnessApplication : IApplicationSource
    {
        public FubuApplication BuildApplication()
        {
            return FubuApplication.For<TestHarnessRegistry>().StructureMap();
        }
    }

    public class TestHarnessRegistry : FubuRegistry
    {
        public TestHarnessRegistry()
        {
            AlterSettings<AssetSettings>(x => {
                x.CdnAssets.Add(new CdnAsset { Url = "http://code.jquery.com/all-wrong.js", Fallback = "typeof jQuery == 'undefined'", File = "jquery-2.1.0.min.js"});
                //x.CdnAssets.Add(new CdnAsset { Url = "http://code.jquery.com/jquery-2.1.0.min.js", Fallback = "typeof jQuery == 'undefined'" });
            });
        }
    }

    public class HomeEndpoint
    {
        private readonly FubuHtmlDocument _document;

        public HomeEndpoint(FubuHtmlDocument document)
        {
            _document = document;
        }

        public FubuHtmlDocument Index()
        {
            _document.Title = "FubuMVC Demonstrator";
            _document.Head.Append(_document.Css("myStyles.css"));

            _document.Add("h1").Text("FubuMVC Demonstrator");

            _document.Add("p").Text("Generated at " + DateTime.Now);

            _document.Add("hr");
            _document.Add("h2").Text("Images");

            _document.Add("p").Text("There should be a picture of the Serenity right below me...");
            _document.Add(x => x.Image("Firefly.jpg"));
            _document.Add("p").Text("The url of the image above is '{0}'".ToFormat(_document.ImageUrl("Firefly.jpg")));

            _document.Add("hr");
            _document.Add("h1").Text("Stylesheets");

            _document.Add("div")
                .Text("This text should be green because of the .green class in myStyles.css")
                .AddClass("green");

            _document.Add("hr");
            _document.Add("p").Text("If the script tags are working, you should see text right below me");

            _document.Add("div").Id("script-div");

            _document.Push("footer");

            _document.Add(x => x.Script("jquery-2.1.0.min.js", "Go.js"));

            return _document;
        }
    }



}
