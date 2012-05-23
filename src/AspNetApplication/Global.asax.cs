using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using AspNetApplication.ServerSideEvents;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;
using FubuCore;
using IActivator = Bottles.IActivator;

namespace AspNetApplication
{
    public class Global : System.Web.HttpApplication
    {
        public class BandActivator : Bottles.IActivator
        {
            private readonly EventSource<SimpleInput> _source;

            public BandActivator(EventSource<SimpleInput> source)
            {
                _source = source;
            }

            public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
            {
                var bands =
                    @"
The Black Keys
Florence & the Machine
Alabama Shakes
Delta Spirit
Neil Young and Crazy Horse
Jack White
The Shins
Steve Earle
The Avett Brothers
The Civil Wars
Gary Clark Jr.
Old 97's
Ruthie Foster
";


                Action writeBands = () =>
                {
                    int i = 0;
                    bands.ReadLines().Where(o => o.IsNotEmpty()).Each(band =>
                    {
                        _source.QueueEvent(new ServerEvent
                        {
                            Data = band,
                            Id = i++.ToString()
                        });

                        Thread.Sleep(i * 500);
                    });
                };

                ThreadPool.QueueUserWorkItem(o =>
                {
                    writeBands();
                });
            }
        }


        protected void Application_Start(object sender, EventArgs e)
        {
            // TEMPORARY!!!
            var source = new EventSource<SimpleInput>();


            ObjectFactory.Initialize(x =>
            {
                x.For<IServerEventWriter>().Use<ServerEventWriter>();
                x.ForSingletonOf<IEventSource<SimpleInput>>().Use(source);

                x.For<IActivator>().Add(new BandActivator(source));  
            });


            FubuApplication
                .For<AspNetApplicationFubuRegistry>()
                .StructureMapObjectFactory()
                .Bootstrap();

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}