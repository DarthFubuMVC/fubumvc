using FubuMVC.Core.UI.Scripts;
using HtmlTags;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptProxyTester
    {
        private const string theName = "some name";
        private IScriptFinder finder;
        private IScript script;
        private ScriptProxy theProxy;

        [SetUp]
        public void SetUp()
        {
            finder = MockRepository.GenerateMock<IScriptFinder>();
            script = MockRepository.GenerateMock<IScript>();

            finder.Stub(x => x.Find(theName)).Return(script);

            theProxy = new ScriptProxy(theName, finder);
        }

        [Test]
        public void execute_all_scripts_delegates_to_the_inner()
        {
            var graph = new ScriptGraph(new StubScriptFinder());


            var scripts = new IScript[0];
            script.Expect(x => x.AllScripts(graph)).Return(scripts);

            theProxy.AllScripts(graph).ShouldBeTheSameAs(scripts);
        }

        [Test]
        public void read_all_delegates_to_the_inner()
        {
            string theJavascript = "some javascript";
            script.Stub(x => x.ReadAll()).Return(theJavascript);

            theProxy.ReadAll().ShouldEqual(theJavascript);
        }

        [Test]
        public void create_script_tag_should_delegate_to_the_inner()
        {
            var tag = new HtmlTag("script");

            script.Stub(x => x.CreateScriptTag()).Return(tag);

            theProxy.CreateScriptTag().ShouldBeTheSameAs(tag);
        }
    }
}