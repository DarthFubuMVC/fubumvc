using FubuMVC.Core.Registration;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuTransportation.Runtime;

namespace FubuTransportation.InMemory
{
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