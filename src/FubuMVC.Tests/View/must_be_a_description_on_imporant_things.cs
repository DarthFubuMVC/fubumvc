using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.View
{
    [TestFixture]
    public class must_be_a_description_on_imporant_things
    {

        [Test]
        public void must_be_a_description_on_all_IViewsForActionFilter_implementations()
        {
            IEnumerable<Type> types = typeof(IViewsForActionFilter).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IViewsForActionFilter>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Console.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }
    }
}