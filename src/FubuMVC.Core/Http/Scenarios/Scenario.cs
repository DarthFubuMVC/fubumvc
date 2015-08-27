using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Json;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Http.Scenarios
{
    public interface IScenarioSupport
    {
        string RootUrl { get; }
        T Get<T>();
        OwinHttpResponse Send(OwinHttpRequest request);
    }

    public static class ScenarioSupportExtensions
    {
        public static OwinHttpResponse Send(this IScenarioSupport support, Action<OwinHttpRequest> configuration)
        {
            var scenario = new Scenario(support);
            configuration(scenario.Request);

            return scenario.Response;
        }
    }

    public class Scenario : IUrlExpression, IDisposable
    {
        private readonly IScenarioSupport _support;
        private readonly OwinHttpRequest _request;
        private readonly Lazy<OwinHttpResponse> _response;
        private readonly Lazy<string> _bodyText;
        private readonly ScenarioAssertionException _assertion = new ScenarioAssertionException();
        private int _expectedStatusCode = HttpStatusCode.OK.As<int>();
        private string _expectedStatusReason = "Ok";
        private bool _ignoreStatusCode;

        void IDisposable.Dispose()
        {
            _assertion.AssertValid();
        }

        public Scenario(IScenarioSupport support)
        {
            _support = support;
            _request = OwinHttpRequest.ForTesting();
            _request.FullUrl(support.RootUrl);
            _request.Environment.Add("scenario-support", _support);
            _request.Accepts("*/*");

            support.Get<SecuritySettings>().Reset();

            _response = new Lazy<OwinHttpResponse>(() =>
            {
                var response = _support.Send(_request);

                if (!_ignoreStatusCode)
                {
                    validateStatusCode(response);
                }


                return response;
            });

            _bodyText = new Lazy<string>(() => _response.Value.Body.ReadAsText());
        }

        private void validateStatusCode(OwinHttpResponse response)
        {
            var httpStatusCode = response.StatusCode;
            if (httpStatusCode != _expectedStatusCode)
            {
                _assertion.Add("Expected status code {0} ({1}), but was {2}", _expectedStatusCode,
                    _expectedStatusReason, httpStatusCode);

                if (httpStatusCode >= 500)
                {
                    _assertion.Body = response.Body.ReadAsText();
                }
            }
        }

        public IUrlExpression Get
        {
            get
            {
                _request.HttpMethod("GET");
                return this;
            }
        }

        public IUrlExpression Put
        {
            get
            {
                _request.HttpMethod("PUT");
                return this;
            }
        }

        public IUrlExpression Delete
        {
            get
            {
                _request.HttpMethod("DELETE");
                return this;
            }
        }

        public IUrlExpression Post
        {
            get
            {
                _request.HttpMethod("POST");
                return this;
            }
        }

        public IUrlExpression Head
        {
            get
            {
                _request.HttpMethod("HEAD");
                return this;
            }
        }

        public SecuritySettings Security
        {
            get { return _support.Get<SecuritySettings>(); }
        }


        public OwinHttpRequest Request
        {
            get { return _request; }
        }

        public OwinHttpResponse Response
        {
            get { return _response.Value; }
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

        public void ContentShouldBe(string exactContent)
        {
            if (_bodyText.Value != exactContent)
            {
                _assertion.Add("Expected the content to be '{0}'", exactContent);

                _assertion.Body = _bodyText.Value;
            }
        }

        private IUrlRegistry urls
        {
            get { return _support.Get<IUrlRegistry>(); }
        }

        SendExpression IUrlExpression.Action<T>(Expression<Action<T>> expression)
        {
            _request.RelativeUrl(urls.UrlFor(expression, _request.HttpMethod()));
            return new SendExpression(_request);
        }

        SendExpression IUrlExpression.Url(string relativeUrl)
        {
            _request.RelativeUrl(relativeUrl);
            return new SendExpression(_request);
        }

        SendExpression IUrlExpression.Input<T>(T input)
        {
            var url = input == null
                ? urls.UrlFor<T>(_request.HttpMethod())
                : urls.UrlFor(input, _request.HttpMethod());

            _request.RelativeUrl(url);

            return new SendExpression(_request);
        }

        SendExpression IUrlExpression.Json<T>(T input)
        {
            this.As<IUrlExpression>().Input(input);
            _request.Body.JsonInputIs(_support.Get<IJsonSerializer>().Serialize(input));

            _request.ContentType("application/json");
            _request.Accepts("application/json");

            return new SendExpression(_request);
        }

        SendExpression IUrlExpression.Xml<T>(T input)
        {
            var writer = new StringWriter();

            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, input);

            var bytes = Encoding.Default.GetBytes(writer.ToString());

            Request.Input.Write(bytes, 0, bytes.Length);

            Request.ContentType("application/xml");
            Request.Accepts("application/xml");

            this.As<IUrlExpression>().Input(input);

            return new SendExpression(_request);
        }

        SendExpression IUrlExpression.FormData<T>(T input)
        {
            this.As<IUrlExpression>().Input(input);

            _request.ContentType(MimeType.HttpFormMimetype);

            var dictionary = new Dictionary<string, object>();
            new TypeDescriptorCache().ForEachProperty(typeof(T), prop =>
            {
                var rawValue = prop.GetValue(input, null);
                var httpValue = rawValue == null ? string.Empty : rawValue.ToString().UrlEncoded();

                dictionary.Add(prop.Name, httpValue);
            });

            var post = dictionary.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)).Join("&");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(post);
            writer.Flush();
            stream.Position = 0;

            _request.Body.ReplaceBody(stream);

            return new SendExpression(_request);
        }


        public void StatusCodeShouldBe(HttpStatusCode httpStatusCode)
        {
            _expectedStatusCode = (int) httpStatusCode;
            _expectedStatusReason = httpStatusCode.ToString();
        }

        public void StatusCodeShouldBe(int statusCode)
        {
            _expectedStatusCode = statusCode;
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
                        _parent._assertion.Add(
                            "Expected a single header value of '{0}'='{1}', but no values were found on the response",
                            _headerKey, expected);
                        break;

                    case 1:
                        var actual = values.Single();
                        if (actual != expected)
                        {
                            _parent._assertion.Add(
                                "Expected a single header value of '{0}'='{1}', but the actual value was '{2}'",
                                _headerKey, expected, actual);
                        }
                        break;

                    default:
                        var valueText = values.Select(x => "'" + x + "'").Join(", ");
                        _parent._assertion.Add(
                            "Expected a single header value of '{0}'='{1}', but the actual values were {2}", _headerKey,
                            expected, valueText);
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
                        _parent._assertion.Add(
                            "Expected a single header value of '{0}', but no values were found on the response",
                            _headerKey);
                        break;
                    case 1:
                        return this;

                    default:
                        var valueText = values.Select(x => "'" + x + "'").Join(", ");
                        _parent._assertion.Add(
                            "Expected a single header value of '{0}', but found multiple values on the response: {1}",
                            _headerKey, valueText);
                        break;
                }


                return this;
            }

            public void ShouldNotBeWritten()
            {
                var values = _parent.Response.HeaderValueFor(_headerKey);
                if (values.Any())
                {
                    var valueText = values.Select(x => "'" + x + "'").Join(", ");
                    _parent._assertion.Add("Expected no value for header '{0}', but found values {1}", _headerKey,
                        valueText);
                }
            }
        }


        public void StatusCodeShouldBeOk()
        {
            StatusCodeShouldBe(HttpStatusCode.OK);
        }

        public void ContentTypeShouldBe(MimeType mimeType)
        {
            ContentTypeShouldBe(mimeType.Value);
        }


        public void ContentTypeShouldBe(string mimeType)
        {
            Header(HttpResponseHeaders.ContentType).SingleValueShouldEqual(mimeType);
        }

        public void FormData<T>(T target, string method = "POST",
            string contentType = "application/x-www-form-urlencoded", string accept = "*/*") where T : class
        {
            Request.Body.WriteFormData(target);

            Request.HttpMethod(method);
            Request.ContentType(contentType);

            this.As<IUrlExpression>().Input(target);
        }

        public void IgnoreStatusCode()
        {
            _ignoreStatusCode = true;
        }
    }
}