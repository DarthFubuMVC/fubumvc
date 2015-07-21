using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    [System.ComponentModel.Description("All service bus channels use an in-memory queue")]
    public class AllQueuesInMemoryPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            var settings = graph.Settings.Get<TransportSettings>();
            settings.SettingTypes.Each(settingType => {
                var settingObject = InMemoryTransport.ToInMemory(settingType);

                graph.Services.AddService(settingType, ObjectDef.ForValue(settingObject));
                graph.Services.Clear(typeof(ITransport));
            });
        }
    }
}