using System;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Extensions;
using FubuMVC.Core.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.UI.Extensions
{
    [TestFixture]
    public class ExtensionsExpressionTester
    {
        private FubuRegistry theRegistry;
        private ExtensionsExpression theExpression;

        [SetUp]
        public void SetUp()
        {
            theRegistry = new FubuRegistry();
            theExpression = new ExtensionsExpression(theRegistry);
        }

        private ContentExtensionGraph theGraph
        {
            get
            {
                var graph = BehaviorGraph.BuildFrom(theRegistry);
                return graph.Services.DefaultServiceFor<ContentExtensionGraph>().Value.As<ContentExtensionGraph>();
            }
        }

        [Test]
        public void registers_the_content_extension()
        {
            var extension = MockRepository.GenerateStub<IContentExtension<ExtendedModel>>();
            theExpression.For(extension);

            theGraph
                .ShelfFor<ExtendedModel>()
                .AllExtensions()
                .ShouldHaveTheSameElementsAs(extension);
        }

        [Test]
        public void registers_the_content_extension_for_the_tag()
        {
            var tag = "1234";
            var extension = MockRepository.GenerateStub<IContentExtension<ExtendedModel>>();
            theExpression.For(tag, extension);

            theGraph
                .ShelfFor<ExtendedModel>()
                .ExtensionsFor(tag)
                .ShouldHaveTheSameElementsAs(extension);
        }

        [Test]
        public void registers_a_lambda_expression()
        {
            var value = Guid.NewGuid();
            Func<IFubuPage<ExtendedModel>, object> func = x => value;
            theExpression.For(func);

            var extension = theGraph.ShelfFor<ExtendedModel>().AllExtensions().Single();
            extension.GetExtensions(null).ShouldHaveTheSameElementsAs(value);
        }

        [Test]
        public void registers_a_lambda_expression_for_the_tag()
        {
            var value = Guid.NewGuid();
            var tag = "Test Tag";
            Func<IFubuPage<ExtendedModel>, object> func = x => value;
            theExpression.For(tag, func);

            var extension = theGraph.ShelfFor<ExtendedModel>().ExtensionsFor(tag).Single();
            extension.GetExtensions(null).ShouldHaveTheSameElementsAs(value);
        }

        public class ExtendedModel { }
    }
}