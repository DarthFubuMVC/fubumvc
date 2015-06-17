using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuTestingSupport;
using FubuTransportation.ErrorHandling;
using FubuTransportation.Runtime.Invocation;
using NUnit.Framework;

namespace FubuTransportation.Testing.ErrorHandling
{
    [TestFixture]
    public class ErrorHandling_services_have_descriptions_for_diagnostics
    {
        [Test]
        public void must_be_a_description_on_all_IErrorHandler_types()
        {
            IEnumerable<Type> types = typeof(IErrorHandler).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IErrorHandler>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Debug.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_IExceptionMatch_types()
        {
            IEnumerable<Type> types = typeof(IErrorHandler).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IExceptionMatch>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Debug.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }

        [Test]
        public void must_be_a_description_on_all_IContinuation_types()
        {
            IEnumerable<Type> types = typeof(IErrorHandler).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IContinuation>())
                .Where(x => !Description.HasExplicitDescription(x));

            types.Each(x => Debug.WriteLine(x.Name));

            types.Any().ShouldBeFalse();
        }
    }
}