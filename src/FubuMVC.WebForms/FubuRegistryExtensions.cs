using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.WebForms
{
    public static class FubuRegistryExtensions
    {
        public static void RegisterPartials(this FubuRegistry registry,
                                            Action<IPartialViewTypeRegistrationExpression> registration)
        {
            registry.Configure(x =>
            {
                var services = x.Services;

                services.SetServiceIfNone(typeof(IPartialViewTypeRegistry), ObjectDef.ForValue(new PartialViewTypeRegistry()));
                var partialRegistry = services.FindAllValues<IPartialViewTypeRegistry>().FirstOrDefault();

                var expression = new PartialViewTypeRegistrationExpression(partialRegistry);
                registration(expression);
            });
        }
    }
}