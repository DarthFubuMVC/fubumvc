using System;
using System.CodeDom.Compiler;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.Tokenization;
using FubuMVC.Spark.Tokenization.Parsing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.Tokenization
{
    [TestFixture]
    public class ViewModelModifierTester : InteractionContext<ViewModelModifier>
    {
        private ModificationContext _context;
        private SparkItem _sparkItem;

        protected override void beforeEach()
        {
            _sparkItem = new SparkItem("", "", "");
            _context = new ModificationContext();
            
            MockFor<ISparkParser>()
                .Stub(x => x.Parse(_context.FileContent, "viewdata", "model")).Return("FubuMVC.Spark.Tests.Tokenization.Bar");
        }

        [Test]
        public void if_view_model_type_fullname_exists_in_different_assemblies_nothing_is_assigned()
        {
            _context.TypePool = duplicateFullnamePool();
            ClassUnderTest.Modify(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldBeNull();
        }

        // TODO: Test that ambiguity is logged

        [Test]
        public void if_view_model_type_exists_it_is_assigned_on_item()
        {
            _context.TypePool = new TypePool(GetType().Assembly);
            _context.TypePool.AddType(typeof(Bar));

            ClassUnderTest.Modify(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldEqual(typeof(Bar));
        }

        [Test]
        public void if_view_model_type_does_not_exist_nothing_is_assigned()
        {
            _context.TypePool = new TypePool(GetType().Assembly);

            ClassUnderTest.Modify(_sparkItem, _context);
            _sparkItem.ViewModelType.ShouldBeNull();
        }

        private TypePool duplicateFullnamePool()
        {
            var pool = new TypePool(GetType().Assembly);
            pool.AddType(generateType("namespace FubuMVC.Spark.Tests.Tokenization{public class Bar{}}", "FubuMVC.Spark.Tests.Tokenization.Bar"));
            pool.AddType(typeof(Bar));
            return pool;
        }
        private static Type generateType(string source, string fullName)
        {
            var parms = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false
            };

            return CodeDomProvider
                .CreateProvider("CSharp")
                .CompileAssemblyFromSource(parms, source)
                .CompiledAssembly
                .GetType(fullName);
        } 
    }

    public class Bar { }
}