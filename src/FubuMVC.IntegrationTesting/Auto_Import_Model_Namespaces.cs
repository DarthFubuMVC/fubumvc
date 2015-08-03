using FubuMVC.Core;
using FubuMVC.Core.View;
using Shouldly;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting
{
    [TestFixture]
    public class Auto_Import_Model_Namespaces
    {
        [Test]
        public void has_all_the_namespaces_for_the_input_and_output_models()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var namespaces = runtime.Get<CommonViewNamespaces>();
                namespaces.Namespaces.ShouldContain("Red.Testing");
                namespaces.Namespaces.ShouldContain("Green.Testing");
                namespaces.Namespaces.ShouldContain("Blue.Testing");
            }
        }
    }
}

namespace Red.Testing
{
    public class Model1
    {
    }

    public class RedModelEndpoint
    {
        public void post_red_model(Model1 model)
        {
        }
    }
}

namespace Green.Testing
{
    public class Model1
    {
    }

    public class GreenModelEndpoint
    {
        public void post_Green_model(Model1 model)
        {
        }
    }
}

namespace Blue.Testing
{
    public class Model1
    {
    }

    public class BlueModelEndpoint
    {
        public void post_Blue_model(Model1 model)
        {
        }
    }
}