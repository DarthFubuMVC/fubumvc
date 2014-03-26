using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using System.Collections.Generic;
using StringWriter = System.IO.StringWriter;

namespace FubuMVC.Diagnostics.Tests
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void generate_visualizers()
        {
            var path = @"C:\code\diagnostics\src\FubuMVC.Diagnostics\Visualization\Visualizers\ConfigurationActions";

            var types = typeof (FubuRequest).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<IConfigurationAction>());

            var fileSystem = new FileSystem();

            types.Each(x =>
            {
                var writer = new StringWriter();
                /*
<use master="" />
<viewdata model="FubuMVC.Diagnostics.Visualization.BehaviorNodeViewModel" />
                 */
                writer.WriteLine("<use master=\"\" />");
                writer.WriteLine("<viewdata model=\"{0}\" />", x.FullName);

                var file = path.AppendPath(x.Name + ".spark");

                fileSystem.WriteStringToFile(file, writer.ToString());

            });
        }
    }
}