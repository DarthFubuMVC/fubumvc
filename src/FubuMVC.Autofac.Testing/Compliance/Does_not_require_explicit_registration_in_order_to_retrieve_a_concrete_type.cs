using NUnit.Framework;

using FubuTestingSupport;


namespace FubuMVC.Autofac.Testing.Compliance
{
    [TestFixture]
    public class Does_not_require_explicit_registration_in_order_to_retrieve_a_concrete_type
    {
        [Test]
        public void can_retrieve_a_concrete_class()
        {
            ContainerFacilitySource.New(x => { })
                .Get<SomeSettings>().ShouldNotBeNull();
        }
    }

    public class SomeSettings
    {
        
    }
}