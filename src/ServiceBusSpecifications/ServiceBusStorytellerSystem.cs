using Serenity.ServiceBus;
using ServiceNode;
using WebsiteNode;

namespace ServiceBusSpecifications
{
    public class ServiceBusStorytellerSystem : FubuTransportSystem<WebsiteApplication>
    {
        public ServiceBusStorytellerSystem()
        {
            AddRemoteSubSystem("ServiceNode", x => {
                x.UseParallelServiceDirectory("ServiceNode");
                x.Setup.ShadowCopyFiles = false.ToString();
            });

            

            OnContextCreation(TextFileWriter.Clear);

            OnContextCreation<WebsiteNode.MessageRecorder>(x => x.Messages.Clear());
        }


    }
}