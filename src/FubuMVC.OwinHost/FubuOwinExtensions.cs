using System;
using FubuMVC.Core;
using Owin;

namespace FubuMVC.OwinHost
{
    public static class FubuOwinExtensions
    {
         public static void RunFubu<TSource>(this IAppBuilder builder) where TSource : IApplicationSource, new()
         {
             var source = new TSource();
             var application = source.BuildApplication();

             builder.RunFubu(application);
         }

        public static void RunFubu(this IAppBuilder builder, FubuApplication application)
        {
            application.Bootstrap();
            var host = new FubuOwinHost();
            builder.Run(host);
        }
    }
}