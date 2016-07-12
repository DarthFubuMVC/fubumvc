using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Formatters;
using HtmlTags;
using Newtonsoft.Json;
using StringWriter = System.IO.StringWriter;

namespace FubuMVC.Core.Diagnostics
{
    public class DiagnosticChain : RoutedChain
    {
        public const string DiagnosticsUrl = "_fubu";

        public static IRouteDefinition BuildRoute(ActionCall call)
        {
            // I hate this.
            if (call.HandlerType == typeof (FubuDiagnosticsEndpoint))
            {
                return buildStandardRoute(call);
            }

            var prefix = call.HandlerType.Name.Replace("FubuDiagnostics", "").ToLower();

            if (call.Method.Name == "Index")
            {
                return new RouteDefinition("{0}/{1}".ToFormat(DiagnosticsUrl, prefix).TrimEnd('/'));
            }

            var route = call.ToRouteDefinition();
            MethodToUrlBuilder.Alter(route, call);
            route.Prepend(prefix);
            route.Prepend(DiagnosticsUrl);

            return route;
        }

        private static IRouteDefinition buildStandardRoute(ActionCall call)
        {
            var route = call.ToRouteDefinition();
            MethodToUrlBuilder.Alter(route, call);
            return route;
        }

        public DiagnosticChain(ActionCall call) : base(BuildRoute(call))
        {
            if (call.HasInput)
            {
                Route.ApplyInputType(call.InputType());
            }

            Tags.Add(NoTracing);

            RouteName = call.HandlerType.Name.Replace("FubuDiagnostics", "") 
                + ":" 
                + call.Method.Name.Replace("get_", "").Replace("post_", "").Replace("{", "").Replace("}", "");

            AddToEnd(call);

            if (call.HasInput) Input.Add(new NewtonsoftJsonFormatter());
            if (call.HasOutput && call.OutputType() != typeof(HtmlTag) && !call.OutputType().CanBeCastTo<HtmlDocument>())
            {
                var writerType = typeof(DiagnosticJsonWriter<>).MakeGenericType(call.OutputType());
                var writer = Activator.CreateInstance(writerType).As<IMediaWriter>();
                Output.Add(writer);
            }
        }

        public new static DiagnosticChain For<T>(Expression<Action<T>> method)
        {
            var call = ActionCall.For(method);
            return new DiagnosticChain(call);
        }
    }

    [Description("Diagnostics Json Writer")]
    public class DiagnosticJsonWriter<T> : IMediaWriter<T>
    {
        private readonly JsonSerializer _serializer;

        public DiagnosticJsonWriter()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "application/json";
                yield return "text/json";
            }
        }


        
        public Task Write(string mimeType, IFubuRequestContext context, T resource)
        {
            var stringWriter = new StringWriter();
            var writer = new JsonTextWriter(stringWriter);
            _serializer.Serialize(writer, resource);

            return context.Writer.Write(mimeType, stringWriter.ToString());
        }
        
    }
}