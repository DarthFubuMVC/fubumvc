using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Bottles;
using FubuCore;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class HtmlHeadInjectionMiddleware : IOwinMiddleware
    {
        public static readonly string TEXT_PROPERTY = "HTML-HEAD-INJECTION";

        public static void ApplyInjection(OwinSettings settings)
        {
            if (!FubuMode.InDevelopment()) return;

            var injectedContent = PackageRegistry.Properties[TEXT_PROPERTY];
            if (injectedContent.IsNotEmpty())
            {
                settings.AddMiddleware<HtmlHeadInjectionMiddleware>().Arguments.With(new InjectionOptions
                {
                    Content = _ => injectedContent
                });
            }
        }

        private readonly AppFunc _inner;
        private readonly InjectionOptions _options;

        public HtmlHeadInjectionMiddleware(AppFunc inner, InjectionOptions options)
        {
            _inner = inner;
            _options = options;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            if (!environment.Get<string>(OwinConstants.RequestMethodKey).EqualsIgnoreCase("GET"))
            {
                return _inner(environment);
            }

            var originalOutput = environment.Get<Stream>(OwinConstants.ResponseBodyKey);
            var recordedStream = new MemoryStream();
            environment.Set(OwinConstants.ResponseBodyKey, recordedStream);

            return _inner(environment).ContinueWith(t => {
                recordedStream.Position = 0;
                environment[OwinConstants.ResponseBodyKey] = originalOutput;

                var response = new OwinHttpResponse(environment);

                if (IsGetHtmlRequest(environment) && response.StatusCode < 500)
                {
                    injectContent(environment, recordedStream, response);
                }
                else
                {
                    response.StreamContents(recordedStream);
                }
            });
        }

        private void injectContent(IDictionary<string, object> environment, MemoryStream recordedStream, OwinHttpResponse response)
        {
            var html = recordedStream.ReadAllText();
            var builder = new StringBuilder(html);
            var position = html.IndexOf("</head>", 0, StringComparison.OrdinalIgnoreCase);

            if (position >= 0)
            {
                builder.Insert(position, _options.Content(environment));
            }

            response.Write(builder.ToString());
            response.Flush();
        }

        public static bool IsGetHtmlRequest(IDictionary<string, object> environment)
        {
            return environment.Get<string>(OwinConstants.RequestMethodKey).EqualsIgnoreCase("GET") &&
                   new OwinHttpResponse(environment).ContentTypeMatches(MimeType.Html);
        }
    }

    public class InjectionOptions
    {
        public Func<IDictionary<string, object>, string> Content = x => "";
    }
}