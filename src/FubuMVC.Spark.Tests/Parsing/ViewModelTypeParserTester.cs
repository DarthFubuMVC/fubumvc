using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Parsing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Parsing
{
    public class ViewModelTypeParserTester
    {
        private readonly IViewModelTypeParser _parser;
        public ViewModelTypeParserTester()
        {
            var typePool = new TypePool(GetType().Assembly) { ShouldScanAssemblies = true };
            var extractor = new ElementNodeExtractor();

            _parser = new ViewModelTypeParser(extractor, typePool);
        }

        [Test]
        public void when_no_view_model_is_present_null_is_returned()
        {
            var content = 
                Templates.UseMaster.ToFormat("Fubu") + 
                Templates.UseNamespace.ToFormat("A.B.C") +
                Templates.Content.ToFormat("parsing");
            
            _parser.Parse(content).ShouldBeNull();
        }

        [Test]
        public void when_full_named_view_model_is_present_type_is_returned()
        {
            var content = 
                Templates.UseMaster.ToFormat("Spark") +
                Templates.ViewdataModel.ToFormat(GetType().FullName);

            _parser.Parse(content).ShouldEqual(GetType());
        }

        [Test]
        public void when_partial_named_view_model_is_present_null_is_returned()
        {
            var content = 
                Templates.UseMaster.ToFormat("Bottles") + 
                Templates.ViewdataModel.ToFormat(GetType().Name);
            
            _parser.Parse(content).ShouldBeNull();
        }

        [Test]
        public void when_full_named_view_model_but_ambiguity_among_assemblies_null_is_returned()
        {
            var typePool = new TypePool(GetType().Assembly)
            {
                ShouldScanAssemblies = true
            };
            
            typePool.AddAssembly(GetType().Assembly);
            typePool.AddAssembly(typeof(Core.DoNext).Assembly);

            
            var extractor = new ElementNodeExtractor();
            var parser = new ViewModelTypeParser(extractor, typePool);
            var content = Templates.ViewdataModel.ToFormat("FubuMVC.Core.View.ViewPath");

            parser.Parse(content).ShouldBeNull();
        }
    }
}

namespace FubuMVC.Core.View
{
    public class ViewPath
    {

    }
}
