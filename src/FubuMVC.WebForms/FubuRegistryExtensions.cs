using System;
using System.Linq;
using FubuMVC.Core;

namespace FubuMVC.WebForms
{
    public static class FubuRegistryExtensions
    {
        public static void RegisterPartials(this FubuRegistry registry, Action<IPartialViewTypeRegistrationExpression> registration)
        {
            registry.Services(x =>
            {
                x.SetServiceIfNone<IPartialViewTypeRegistry>(new PartialViewTypeRegistry());
                var partialRegistry = x.FindAllValues<IPartialViewTypeRegistry>().FirstOrDefault();

                var expression = new PartialViewTypeRegistrationExpression(partialRegistry);
                registration(expression);
            });
        }

        
    }
}