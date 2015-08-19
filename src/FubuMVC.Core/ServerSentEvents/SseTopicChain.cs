using System;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.ServerSentEvents
{
    public class SseTopicChain : RoutedChain
    {
        private static string WriteMethod = ReflectionHelper.GetMethod<ChannelWriter<Topic>>(x => x.Write(null))
            .Name;

        public static IRouteDefinition ToRoute(Type topicType)
        {
            var url = "_events/" + topicType.Name.ToLower().Replace("topic", "");
            var routeDefinition = RouteBuilder.Build(topicType, url);   
            return routeDefinition;
        }

        public SseTopicChain(Type topicType) : base(ToRoute(topicType))
        {
            var handlerType = typeof(ChannelWriter<>).MakeGenericType(topicType);
            var method = handlerType.GetMethod(WriteMethod);

            var action = new ActionCall(handlerType, method);

            DefaultUrlPolicy.AddBasicRouteInputs(action, (RouteDefinition) Route);

            AddToEnd(action);
        }
    }
}