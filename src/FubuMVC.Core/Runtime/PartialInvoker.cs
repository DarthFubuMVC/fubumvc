﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Runtime
{
    public class PartialInvoker : IPartialInvoker
    {
        private readonly IPartialFactory _factory;
        private readonly IFubuRequest _request;
        private readonly IAuthorizationPreviewService _authorization;
        private readonly IOutputWriter _writer;
        private readonly ISetterBinder _setterBinder;
        private readonly IChainResolver _resolver;

        public PartialInvoker(IPartialFactory factory, IFubuRequest request, IAuthorizationPreviewService authorization, IOutputWriter writer, ISetterBinder setterBinder, IChainResolver resolver)
        {
            _factory = factory;
            _request = request;
            _authorization = authorization;
            _writer = writer;
            _setterBinder = setterBinder;
            _resolver = resolver;
        }

        public async Task<string> Invoke<T>(string categoryOrHttpMethod = null) where T : class
        {
            var output = string.Empty;
            var input = _request.Get<T>();
            if (_authorization.IsAuthorized(input, categoryOrHttpMethod))
            {
                output = await invokeWrapped(typeof(T), categoryOrHttpMethod).ConfigureAwait(false);
            }
            return output;
        }

        public async Task<string> InvokeObject(object model, bool withModelBinding = false, string categoryOrHttpMethod = null)
        {
            var output = string.Empty;
            if (_authorization.IsAuthorized(model))
            {
                var requestType = model.GetType();
                if (withModelBinding)
                {
                    _setterBinder.BindProperties(requestType, model);
                }
                _request.Set(requestType, model);
                output = await invokeWrapped(requestType, categoryOrHttpMethod).ConfigureAwait(false);
            }
            return output;
        }

        public Task<string> InvokeAsHtml(object model)
        {
            var current = _request.Get<CurrentMimeType>();
            _request.Set(new CurrentMimeType(MimeType.HttpFormMimetype, MimeType.Html.Value));

            try
            {
                return InvokeObject(model);
            }
            finally
            {
                _request.Set(current);
            }
        }

        public async Task<object> InvokeFast(BehaviorChain chain, object input = null)
        {
            _request.Set(OutputPartialBehavior.None);
            if (input != null)
            {
                _request.Set(chain.InputType(), input);
            }

            try
            {
                var partial = _factory.BuildPartial(chain);
                await partial.InvokePartial().ConfigureAwait(false);

                // TODO -- how to detect authorization failures here?
                var resourceType = chain.ResourceType();
                return _request.Has(resourceType) ? _request.Get(resourceType) : null;
            }
            finally
            {
                _request.Set(OutputPartialBehavior.Write);
            }
        }

        private async Task<string> invokeWrapped(Type requestType, string categoryOrHttpMethod = null)
        {
            var chain = _resolver.FindUniqueByType(requestType, category: categoryOrHttpMethod ?? Categories.VIEW);
            var partial = _factory.BuildPartial(chain);

            var output = await _writer.Record(() => partial.InvokePartial()).ConfigureAwait(false);

            output.Headers().Each(x => _writer.AppendHeader(x.Name, x.Value));
            return output.GetText();
        }
    }
}