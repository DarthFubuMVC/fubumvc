using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI;
using FubuMVC.Core.View;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.IntegrationTesting.UI
{
    [TestFixture]
    public class ElementConventions_for_Partials_in_Loop_Tester : FubuPageExtensionMultiplePartialContext
    {
         [Test]
         public void DisplayFor_in_partials_in_a_for_each_loop()
         {
             execute(page => page.DisplayFor(x => x.Name));
             theResult.ShouldEqual("<span id=\"Name\">Item1</span><span id=\"Name\">Item2</span><span id=\"Name\">Item3</span>");
         }

        [Test]
        public void LabelFor_in_partials_in_a_for_each_loop()
        {
            execute(page => page.LabelFor(x => x.Name));
            theResult.ShouldEqual("<label for=\"Name\">Name</label><label for=\"Name\">Name</label><label for=\"Name\">Name</label>");
        }

        [Test]
        public void InputFor_in_partials_in_a_for_each_loop()
        {
            execute(page => page.InputFor(x => x.Name));
            theResult.ShouldEqual("<input type=\"text\" value=\"Item1\" name=\"Name\" /><input type=\"text\" value=\"Item2\" name=\"Name\" /><input type=\"text\" value=\"Item3\" name=\"Name\" />");
        }
    }

    [TestFixture]
    public class FubuPageExtensionMultiplePartialContext
    {
        [TestFixtureSetUp]
        public void StartServer()
        {
            var port = PortFinder.FindPort(7500);

            _server = FubuApplication.DefaultPolicies().StructureMap(new Container()).RunEmbedded(port: port);
        }

        [TestFixtureTearDown]
        public void StopServer()
        {
            _server.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            theResult = string.Empty;
        }


        protected string theResult;
        private EmbeddedFubuMvcServer _server;

        public string BaseAddress
        {
            get { return _server.BaseAddress; }
        }


        protected void execute(Func<IFubuPage<PartialViewModel>, object> sourceModifier)
        {
            PartialConventionEndpoint.SourceModifier = sourceModifier;

            var response = _server.Endpoints.Get<PartialConventionEndpoint>(x => x.get_multiplepartialresult());
            response.StatusCodeShouldBe(HttpStatusCode.OK);

            theResult = response.ReadAsText().Replace("\n", "").Replace("\r", "");
        }

        public class PartialConventionEndpoint
        {
            private readonly IPartialInvoker _invoker;
            private readonly IServiceLocator _locator;
            private readonly IFubuRequest _request;

            public static Func<IFubuPage<PartialViewModel>, object> SourceModifier = page => "nothing";

            public PartialConventionEndpoint(IPartialInvoker invoker, IServiceLocator locator, IFubuRequest request)
            {
                _invoker = invoker;
                _locator = locator;
                _request = request;
            }

            public string get_multiplepartialresult()
            {
                return _invoker.Invoke<PartialInput>();
            }

            public string get_partial(PartialInput input)
            {
                var builder = new StringBuilder();
                var partialModels = new[]
                    {
                        new PartialViewModel{Name = "Item1"}, 
                        new PartialViewModel{Name = "Item2"},
                        new PartialViewModel{Name = "Item3"}
                    };

                partialModels.Each(m =>
                    {
                        _request.Set(m);
                        var page = new FubuHtmlDocument<PartialViewModel>(_locator, _request);
                        builder.AppendLine(SourceModifier(page).ToString());
                        //builder.AppendLine();
                    });

                return builder.ToString();
            }
        }

        public class PartialInput
        {
        }

        public class PartialViewModel
        {
            public string Name { get; set; }
        }
    }
}