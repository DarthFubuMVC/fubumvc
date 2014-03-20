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
            using (var server = FubuApplication.DefaultPolicies().StructureMap().RunEmbeddedWithAutoPort())
            {
                Process.Start(server.BaseAddress);

                Console.WriteLine("Press any key to quit");
                Console.ReadLine();
            }

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


            _document.Add("h1").Text("FubuMVC Demonstrator");

            _document.Add("hr");
            _document.Add("h2").Text("Images");

            _document.Add("p").Text("There should be a picture of the Serenity right below me...");
            _document.Add(x => x.Image("Firefly.jpg"));
            _document.Add("p").Text("The url of the image above is '{0}'".ToFormat(_document.ImageUrl("Firefly.jpg")));

            return _document;
        }
    }



}
