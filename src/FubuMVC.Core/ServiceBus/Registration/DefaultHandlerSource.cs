namespace FubuMVC.Core.ServiceBus.Registration
{
    public class DefaultHandlerSource : HandlerSource
    {
        public DefaultHandlerSource()
        {
            UseThisAssembly();
            IncludeClassesSuffixedWithConsumer();
            IncludeClassesSuffixedWithHandler();
            IncludeClassesMatchingSagaConvention();
        }
    }
}