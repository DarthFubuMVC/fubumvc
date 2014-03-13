using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Http.Scenarios
{
    // TODO -- flush out a lot more of this
    public interface IScenario : IDisposable
    {
        IUrlExpression Get { get; }
        IUrlExpression Put { get; }
        IUrlExpression Delete { get; }
        IUrlExpression Post { get; }
        IUrlExpression Head { get; }

        OwinHttpRequest Request { get; }
        OwinHttpResponse Response { get; }

        //void StatusCodeShouldBe(HttpStatusCode code);
        //void StatusDescriptionShouldBe(string text);
        //void ShouldHaveHeader(HttpResponseHeader header);
        //void ContentShouldBe(MimeType mimeType, string content);
        //void ContentTypeShouldBe(MimeType mimeType);
        //void LengthShouldBe(int length);
        //void ContentShouldBe(string mimeType, string content);

        void ContentShouldContain(string text);
    }

    public interface IUrlExpression
    {
        void Action<T>(Expression<Action<T>> expression);
        void Url(string relativeUrl);
        void Input<T>(T input = null) where T : class;
    }

    public class Scenario : IScenario, IUrlExpression
    {
        private readonly IUrlRegistry _urls;
        private readonly OwinHttpRequest _request;
        private readonly Lazy<OwinHttpResponse> _response;
        private readonly Lazy<string> _bodyText;
        private readonly ScenarioAssertionException _assertion = new ScenarioAssertionException();

        void IDisposable.Dispose()
        {
            _assertion.AssertValid();
        }

        public Scenario(IUrlRegistry urls, OwinHttpRequest request, Func<OwinHttpRequest, OwinHttpResponse> runner)
        {
            _urls = urls;
            _request = request;
            _response = new Lazy<OwinHttpResponse>(() => runner(request));

            _bodyText = new Lazy<string>(() => _response.Value.Body.ReadAsText());
        }

        public IUrlExpression Get
        {
            get
            {
                _request.HttpMethod("GET");
                return this;
            }
        }
        public IUrlExpression Put { get; private set; }
        public IUrlExpression Delete { get; private set; }
        public IUrlExpression Post { get; private set; }
        public IUrlExpression Head { get; private set; }

        public OwinHttpRequest Request
        {
            get
            {
                return _request;
            }
        }

        public OwinHttpResponse Response
        {
            get
            {
                return _response.Value;
            }
        }

        public void ContentShouldContain(string text)
        {
            if (!_bodyText.Value.Contains(text))
            {
                _assertion.Add("The response body does not contain expected text \"{0}\"", text);

                _assertion.Body = _bodyText.Value;
            }
        }

        void IUrlExpression.Action<T>(Expression<Action<T>> expression)
        {
            _request.RelativeUrl(_urls.UrlFor(expression, _request.HttpMethod()));
        }

        void IUrlExpression.Url(string relativeUrl)
        {
            _request.RelativeUrl(relativeUrl);
        }

        void IUrlExpression.Input<T>(T input)
        {
            var url = input == null
                ? _urls.UrlFor<T>(_request.HttpMethod())
                : _urls.UrlFor(input, _request.HttpMethod());

            _request.RelativeUrl(url);
        }
    }

    [Serializable]
    public class ScenarioAssertionException : Exception
    {
        private readonly IList<string> _messages = new List<string>();

        public ScenarioAssertionException()
        {
        }

        protected ScenarioAssertionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void Add(string format, params object[] parameters)
        {
            _messages.Add(format.ToFormat(parameters));
        }

        public void AssertValid()
        {
            if (_messages.Any())
            {
                throw this;
            }
        }

        public override string Message
        {
            get
            {
                var writer = new StringWriter();
                _messages.Each(x => writer.WriteLine(x));

                if (Body.IsNotEmpty())
                {
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("Actual body text was:");
                    writer.WriteLine();
                    writer.WriteLine(Body);
                }

                return writer.ToString();
            }
        }

        public string Body { get; set; }
    }
}