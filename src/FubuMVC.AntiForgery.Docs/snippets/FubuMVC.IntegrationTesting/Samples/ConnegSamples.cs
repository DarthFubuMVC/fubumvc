using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Ajax;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.IntegrationTesting.Samples
{
    // SAMPLE: endpoint-with-input-and-resource
    public class ConnegEndpoint
    {
        public ResourceModel get_resource(InputMessage message)
        {
            return new ResourceModel();
        }
    }
    // ENDSAMPLE

    public class ResourceModel
    {
        
    }

    // SAMPLE: custom-reader
    public class InputMessage
    {

    }


    public class SpecialContentMediaReader : IReader<InputMessage>
    {
        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "special/format";
            }
        }

        public InputMessage Read(string mimeType, IFubuRequestContext context)
        {
            // read the body of the http request from IHttpRequest
            // read header information and route information from
            // IHttpRequest

            return new InputMessage();
        }

    }
    // ENDSAMPLE

    // SAMPLE: custom-writer
    public class SomeResource
    {
        public string Color { get; set; }
    }

    public class SpecialContentMediaWriter : IMediaWriter<SomeResource>
    {
        private readonly IOutputWriter _writer;

        // As usual, all IMediaWriter<> objects
        // are resolved from you IoC continer and
        // you can have dependencies injected into
        // your constructor
        public SpecialContentMediaWriter(IOutputWriter writer)
        {
            _writer = writer;
        }


        // This signature is necessary because we are assuming
        // that some Writer's will be able to produce representations
        // for multiple mimetype's
        public void Write(string mimeType, IFubuRequestContext context, SomeResource resource)
        {
            if (mimeType == "special/format")
            {
                writeSpecial(resource);
            }
            else
            {
                writeJson(resource);
            }
        }

        private void writeJson(SomeResource resource)
        {
            // use the IOutputWriter to output the resource
        }

        private void writeSpecial(SomeResource resource)
        {
            _writer.Write("special/format", resource.ToString());
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                // You can use custom mimetypes
                // if you want to be one of the
                // ReSTful cool kids
                yield return "special/format";

                yield return "text/json";
            }
        }
    }
    // ENDSAMPLE

    // SAMPLE: conneg-policies
    public class MyConnegPolicy : Policy
    {
        public MyConnegPolicy()
        {
            // Apply some sort of matching
            // filter.  
            Where.ChainMatches(chain => true);

            // TODO -- redo this.
        }
    }
    // ENDSAMPLE

    // SAMPLE: writer-attribute-for-one-off
    // A formal version of this attribute will be in v1.3
    public class WriterAttribute : ModifyChainAttribute
    {
        private readonly Type[] _types;

        public WriterAttribute(params Type[] types)
        {
            _types = types;
        }

        public override void Alter(ActionCall call)
        {
            var outputNode = call.ParentChain().Output;
            _types.Each(t => outputNode.Add(t));
        }
    }
    // ENDSAMPLE

    // SAMPLE: reader-attribute-for-one-off
    // A formal version of this attribute will be in v1.3
    public class ReaderAttribute : ModifyChainAttribute
    {
        private readonly Type[] _types;

        public ReaderAttribute(params Type[] types)
        {
            _types = types;
        }

        public override void Alter(ActionCall call)
        {
            var inputNode = call.ParentChain().Input;

            _types.Each(inputNode.Add);
        }
    }
    // ENDSAMPLE


    
    public static class ConnegManipulation
    {
        // SAMPLE: conneg-manipulation
        public static void MessWithConneg(BehaviorChain chain)
        {
            // Remove all readers
            chain.Input.ClearAll();

            // Accept 'application/x-www-form-urlencoded' with model binding
            chain.Input.Add(typeof(ModelBindingReader<>));
            
            // Add basic Json reading
            chain.Input.Add(new JsonSerializer());

            // Query whether or not the chain uses the basic Json reading
            bool readsJson = chain.Input.CanRead(MimeType.Json);

            // Add a completely custom Reader
            chain.Input
                .Add(new SpecialContentMediaReader());

            // Are there any Readers?
            chain.HasReaders();

            // Is there any output?
            chain.HasOutput();

            // Remove all writers
            chain.Output.ClearAll();

        }


        // ENDSAMPLE

        // SAMPLE: add-conditions-to-writer
        // This method is just an example of how you can add 
        // runtime conditions to an existing WriterNode
        // hanging off of BehaviorChain.Output.Writers
        

        // TODO -- need to redo this

        // IConditional services are resolved from the IoC
        // container, so you can declare dependencies
        // in the constructor function
        public class MyRuntimeCondition : IConditional
        {
            public bool ShouldExecute(IFubuRequestContext context)
            {
                // apply your own runtime logic
                return false;
            }
        }
        // ENDSAMPLE

    }


    public class JsonInput
    {
        
    }

    public class ConnegSampleEndpoints
    {
        // Because JsonInput implements the FubuMVC.Core.JsonMessage 
        // marker interface, this endpoint will be configured as
        // "Asymmetric Json"
        public AjaxContinuation post_json_input(JsonInput input)
        {
            return AjaxContinuation.Successful();
        }
    }


    // SAMPLE: spoofing-current-mimetype

    // The actual custom behavior
    public class CorrectMimetypeForAjaxBehavior : WrappingBehavior
    {
        private readonly IHttpRequest _httpRequest;
        private readonly IFubuRequest _fubuRequest;

        public CorrectMimetypeForAjaxBehavior(IHttpRequest httpRequest, IFubuRequest fubuRequest)
        {
            _httpRequest = httpRequest;
            _fubuRequest = fubuRequest;
        }

        protected override void invoke(Action action)
        {
            // Just let the normal model binding do its thing here,
            // but "correct" anything that is missing
            var mimetype = _fubuRequest.Get<CurrentMimeType>();

            if (!mimetype.AcceptTypes.Any() && _httpRequest.IsAjaxRequest())
            {
                mimetype.AcceptTypes = new MimeTypeList("application/json");
            }
        }
    }
    // ENDSAMPLE

    // SAMPLE: spoofing-mimetype-policy
    public class CorrectMimetypePolicy : Policy
    {
        public CorrectMimetypePolicy()
        {
            // Any chain that has an Action where the handler class name
            // contains the text "Ajax"
            Where.AnyActionMatches(call => call.HandlerType.Name.Contains("Ajax"));

            Wrap.WithBehavior<CorrectMimetypeForAjaxBehavior>();
        }
    }
    // ENDSAMPLE

}