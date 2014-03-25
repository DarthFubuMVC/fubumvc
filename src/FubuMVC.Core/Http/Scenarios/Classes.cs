using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.Serialization;
using System.Web.UI.WebControls;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Runtime;
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

        void PostAsJson<T>(T input) where T : class;

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
        void ContentShouldNotContain(string text);
        void StatusCodeShouldBe(HttpStatusCode httpStatusCode);
        Scenario.HeaderExpectations Header(string headerKey);
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

        public IUrlExpression Post
        {
            get
            {
                _request.HttpMethod("POST");
                return this;
            }
        }

        public IUrlExpression Head { get; private set; }

        public void PostAsJson<T>(T input) where T : class
        {
            Post.Input(input);
            _request.Body.JsonInputIs(input);
            _request.Header(HttpRequestHeaders.ContentType, MimeType.Json.Value);
        }

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

        public void ContentShouldNotContain(string text)
        {
            if (_bodyText.Value.Contains(text))
            {
                _assertion.Add("The response body should not contain text \"{0}\"", text);

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


        public void StatusCodeShouldBe(HttpStatusCode httpStatusCode)
        {
            if (_response.Value.StatusCode != (int)httpStatusCode)
            {
                _assertion.Add("Expected status code {0} ({1}), but was {2}", httpStatusCode.As<int>(), httpStatusCode, _response.Value.StatusCode);
            }
        }

        public HeaderExpectations Header(string headerKey)
        {
            return new HeaderExpectations(this, headerKey);
        }

        public class HeaderExpectations
        {
            private readonly Scenario _parent;
            private readonly string _headerKey;

            public HeaderExpectations(Scenario parent, string headerKey)
            {
                _parent = parent;
                _headerKey = headerKey;
            }

            public HeaderExpectations SingleValueShouldEqual(string expected)
            {
                var values = _parent.Response.HeaderValueFor(_headerKey);
                switch (values.Count())
                {
                    case 0:
                        _parent._assertion.Add("Expected a single header value of '{0}'='{1}', but no values were found on the response", _headerKey, expected);
                        break;

                    case 1:
                        var actual = values.Single();
                        if (actual != expected)
                        {
                            _parent._assertion.Add("Expected a single header value of '{0}'='{1}', but the actual value was '{2}'", _headerKey, expected, actual);
                        }
                        break;

                    default:
                        var valueText = values.Select(x => "'" + x + "'").Join(", ");
                        _parent._assertion.Add("Expected a single header value of '{0}'='{1}', but the actual values were {2}", _headerKey, expected, valueText);
                        break;
                }



                return this;
            }

            public HeaderExpectations ShouldHaveOneNonNullValue()
            {
                var values = _parent.Response.HeaderValueFor(_headerKey);
                switch (values.Count())
                {
                    case 0:
                        _parent._assertion.Add("Expected a single header value of '{0}', but no values were found on the response", _headerKey);
                        break;
                    case 1:
                        return this;

                    default:
                        var valueText = values.Select(x => "'" + x + "'").Join(", ");
                        _parent._assertion.Add("Expected a single header value of '{0}', but found multiple values on the response: {1}", _headerKey, valueText);
                        break;
                }


                return this;
            }
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