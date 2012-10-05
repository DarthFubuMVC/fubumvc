using System;
using FubuMVC.Core;
using FubuMVC.TestingHarness;
using NUnit.Framework;
using FubuTestingSupport;

namespace ViewEngineIntegrationTesting.ActionlessViews
{
    [TestFixture]
    public class unattached_views_are_always_actionless_views : SharedHarnessContext
    {
        [Test]
        public void can_happily_render_the_actionless_view_as_a_partial()
        {
            endpoints.GetByInput(new AttachedInput{Name = "Jeremy"}).ReadAsText().ShouldContain("The name is Jeremy");
        }
    }

    public class UnattachedModel
    {
        public string Name { get; set;}
    }

    public class AttachedEndpoint
    {
        public AttachedModel get_attached_Name(AttachedInput input)
        {
            return new AttachedModel{
                PartialRequest = new UnattachedModel{Name = input.Name}
            };
        }
    }

   public class AttachedInput
   {
       public string Name { get; set;}
   } 

    public class AttachedModel
    {
        public UnattachedModel PartialRequest { get; set;}
    }
}