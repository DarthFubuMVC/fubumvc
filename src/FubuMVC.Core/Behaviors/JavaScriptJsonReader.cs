using System.Web.Script.Serialization;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class JavaScriptJsonReader : IJsonReader
    {
        private readonly IStreamingData _data;
        private readonly IObjectResolver _objectResolver;
        private readonly IRequestData _requestData;
        private readonly IServiceLocator _serviceLocator;

        public JavaScriptJsonReader(IStreamingData data
             ,IObjectResolver objectResolver,
            IRequestData requestData,
            IServiceLocator serviceLocator
            )
        {
            _data = data;
            _objectResolver = objectResolver;
            _requestData = requestData;
            _serviceLocator = serviceLocator;
        }

        public T Read<T>()
        {
            var serializer = new JavaScriptSerializer();
            string inputText = _data.InputText();
            var model = serializer.Deserialize<T>(inputText);
            _objectResolver.BindProperties(model, new BindingContext(_requestData, _serviceLocator, new NulloBindingLogger()));
            return model;
        }
    }
}