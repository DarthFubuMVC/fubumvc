using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Runtime;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class ModelBinderCacheTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void throw_fubu_exception_if_there_is_no_suitable_binder()
        {
            var cache = StructureMapContainerFacility.GetBasicFubuContainer().GetInstance<ModelBinderCache>();


            Exception<FubuException>.ShouldBeThrownBy(() =>
            {
                cache.BinderFor(typeof (ClassWithNoCtor));
            });

            

        }

        public class ClassWithNoCtor
        {
            private ClassWithNoCtor()
            {
            }
        }
    }
}