using System;
using System.Collections.Generic;
using System.Web;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.AspNet;

namespace FubuMVC.AspNet
{
    public class FubuAspNetHost : IHttpModule
    {
        public static FubuRuntime Runtime;
        private static Action _startRequest = () => { };
        private static Action _endRequest = () => { };

        public static void Bootstrap<T>() where T : FubuRegistry, new()
        {
            Bootstrap(new T());
        }

        public static void Bootstrap(FubuRegistry registry)
        {
            Runtime = registry.ToRuntime();

            var settings = Runtime.Get<DiagnosticsSettings>();
            if (settings.TraceLevel == TraceLevel.None) return;

            var logger = Runtime.Get<IExecutionLogger>();
            _startRequest = () =>
            {
                try
                {
                    HttpContext.Current.Items.Add("owin.Environment", new Dictionary<string, object>());

                    var log = new ChainExecutionLog();
                    HttpContext.Current.Response.AppendHeader(HttpRequestExtensions.REQUEST_ID, log.Id.ToString());
                    HttpContext.Current.Items.Add(AspNetServiceArguments.CHAIN_EXECUTION_LOG, log);
                }
                catch (Exception e)
                {
                    Runtime.Get<ILogger>().Error("Error in request logging", e);
                }
            };

            _endRequest = () =>
            {
                try
                {
                    if (!HttpContext.Current.Items.Contains(AspNetServiceArguments.CHAIN_EXECUTION_LOG)) return;

                    var log = HttpContext.Current.Items[AspNetServiceArguments.CHAIN_EXECUTION_LOG].As<ChainExecutionLog>();
                    log.MarkFinished();

                    logger.Record(log, new AspNetDictionary());
                }
                catch (Exception e)
                {
                    Runtime.Get<ILogger>().Error("Error in request logging", e);
                }
            };
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextOnBeginRequest;
            context.EndRequest += ContextOnEndRequest;
        }

        private void ContextOnEndRequest(object sender, EventArgs eventArgs)
        {
            _endRequest();
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            _startRequest();
        }


        public void Dispose()
        {
            if (Runtime != null) Runtime.Dispose();
        }
    }
}