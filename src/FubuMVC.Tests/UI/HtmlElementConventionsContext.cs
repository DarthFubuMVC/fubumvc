using System;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using FubuMVC.Core.UI.Scenarios;
using NUnit.Framework;

namespace FubuMVC.Tests.UI
{
    public abstract class HtmlElementConventionsContext
    {
        protected HtmlConventionRegistry theRegistry;
        protected Lazy<IElementGenerator<ConventionTarget>> _generator;
        protected ConventionTarget theTarget;

        [SetUp]
        public void SetUp()
        {
            theTarget = new ConventionTarget();
            theRegistry = new HtmlConventionRegistry();
            _generator = new Lazy<IElementGenerator<ConventionTarget>>(() =>
            {
                return HtmlElementScenario<ConventionTarget>.For(x =>
                {
                    x.Library.Import(theRegistry.Library);
                    x.Model = theTarget;
                });
            });


        }

        protected IElementGenerator<ConventionTarget> generator
        {
            get
            {
                return _generator.Value;
            }
        }
    }
}