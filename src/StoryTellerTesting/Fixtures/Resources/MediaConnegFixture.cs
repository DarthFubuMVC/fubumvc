using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Urls;
using FubuTestApplication.ConnegActions;
using Newtonsoft.Json.Linq;
using Serenity;
using Serenity.Endpoints;
using StoryTeller.Engine;

namespace IntegrationTesting.Fixtures.Resources
{
    
    public class MediaConnegFixture : Fixture
    {
        private IUrlRegistry _urls;

        public MediaConnegFixture()
        {
            Title = "Exercising the conneg input and output selection";

            this["MediaSelection"] = new MediaSelection(() => _urls);
        }

        public override void SetUp(ITestContext context)
        {
            _urls = context.Retrieve<IApplicationUnderTest>().Urls;
        }
    }

    // TODO -- back in StoryTeller land, this thing blows.  Watch accessibility of properties, better way to get dependencies
    public class MediaSelection : DecisionTableGrammar
    {
        private static readonly Lazy<IEnumerable<Type>> _messageTypes =
            new Lazy<IEnumerable<Type>>(
                () =>
                {
                    return
                        typeof(ConnegMessage).Assembly.GetExportedTypes().Where(
                            x => x.IsConcreteTypeOf<ConnegMessage>());
                });

        private readonly Lazy<IUrlRegistry> _urls;

        private EndpointDriver _driver;
        private EndpointInvocation _invocation;
        private HttpResponse _response;

        public MediaSelection(Func<IUrlRegistry> getUrls)
        {
            LabelName = "Media Selection Examples";
            _urls = new Lazy<IUrlRegistry>(getUrls);
        }

        protected override void beforeLine()
        {
            _driver = new EndpointDriver(_urls.Value);
            _invocation = new EndpointInvocation();
            _response = null;
        }

        public string Method
        {
            set { _invocation.Method = value; }
        }

        public string Endpoint
        {
            set
            {
                var messageType = _messageTypes.Value.First(x => x.Name == value);
                var connegMessage = Activator.CreateInstance(messageType).As<ConnegMessage>();
                connegMessage.Id = Guid.NewGuid();
                _invocation.Target = connegMessage;
            }
        }

        public string SendAs
        {
            set { _invocation.SendAs = (EndpointFormatting) Enum.Parse(typeof(EndpointFormatting), value, true); }
        }

        public string ContentType
        {
            set { _invocation.ContentType = value; }
        }

        public string Accept
        {
            set
            {
                _invocation.Accept = value;
                _response = _driver.Send(_invocation);
            }
        }

        public bool IsXml
        {
            get
            {
                try
                {
                    return _response.ToXml() != null;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsJson
        {
            get
            {
                try
                {
                    JObject.Parse(_response.ToString());
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public int Status
        {
            get { return (int) _response.StatusCode; }
        }

        public string ResponseType
        {
            get { return _response.ContentType; }
        }


    }
}