using System;
using System.Linq;
using System.Reflection;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus
{
    public class ServiceBusFixture : Fixture
    {
        public ServiceBusFixture()
        {
            Title = "Basic Service Bus";

            var channels = typeof (HarnessSettings).GetProperties().Where(x => x.PropertyType == typeof (Uri));
            AddSelectionValues("Channels", channels.Select(x => x.Name).ToArray());

            //Assembly.GetExecutingAssembly().GetExportedTypes().Where(x => )
        }

        public override void SetUp()
        {
            
        }

        public override void TearDown()
        {
            
        }

        [Hidden, FormatAs("The node name is {name}")]
        public void NodeName(string name)
        {
            
        }

        [Hidden, ExposeAsTable("Listens to Channels")]
        public void ListensTo([SelectionList("Channels")]string Channel)
        {
            
        }

        public void Publishes(string Channel, string Message)
        {
            
        }
    }


}