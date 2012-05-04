using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using NUnit.Framework;

namespace FubuMVC.Tests.NewConneg
{
    [TestFixture]
    public class mimetype_information_must_be_exposed_on_readers_writers_and_formatters
    {
        [Test]
        public void find_and_verify()
        {
            var list = typeof (JsonFormatter).Assembly.GetExportedTypes()
                .ToList()
                .Where(type => type.IsConcreteTypeOf<IFormatter>() && !type.HasAttribute<MimeTypeAttribute>())
                .ToList();

            if (list.Any())
            {
                var message = "These formatters do not expose mimetype information" + Environment.NewLine +
                              list.Select(x => x.FullName).Join(Environment.NewLine);
                Assert.Fail(message);
            }
        }
    }
}