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

    // This attribute will NOT be necessary in FubuMVC 2.0, but is
    // for FubuMVC 1.* to feed the configuration model
    [MimeType("special/format", "text/json")]
    public class SpecialContentMediaReader : IReader<InputMessage>
    {
        private readonly IStreamingData _streaming;
        private readonly ICurrentHttpRequest _httpRequest;

        public SpecialContentMediaReader(IStreamingData streaming, ICurrentHttpRequest httpRequest)
        {
            _streaming = streaming;
            _httpRequest = httpRequest;
        }

        public IEnumerable<string> Mimetypes
        {
            get
            {
                yield return "special/format";
            }
        }

        public InputMessage Read(string mimeType)
        {
            // read the body of the http request from IStreamingData
            // read header information and route information from
            // ICurrentHttpRequest

            return new InputMessage();
        }
    }
    // ENDSAMPLE

    // SAMPLE: custom-writer
    public class SomeResource
    {
        public string Color { get; set; }
    }

    [MimeType("special/format", "text/json")]
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
        public void Write(string mimeType, SomeResource resource)
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

            Conneg.AcceptJson();

            Conneg.AllowHttpFormPosts();

            Conneg.ApplyConneg();

            Conneg.AddHtml();

            Conneg.AddWriter(typeof(SpecialContentMediaWriter));

            Conneg.ClearAllWriters();

            Conneg.MakeAsymmetricJson();

            Conneg.MakeSymmetricJson();
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
            _types.Each(outputNode.AddWriter);
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

            _types.Each(type => {
                inputNode.Readers.AddToEnd(new Reader(type));
            });
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
            chain.Input.AllowHttpFormPosts = true;
            
            // Add basic Json reading
            chain.Input.AddFormatter<JsonFormatter>();

            // Query whether or not the chain uses the basic Json reading
            bool readsJson = chain.Input.UsesFormatter<JsonFormatter>();

            // Add a completely custom Reader
            var specialReader = chain.Input
                .AddReader<SpecialContentMediaReader>();

            // Reorder the special reader to move it to the first
            // as the default
            specialReader.MoveToFront();

            // Add a new Reader as the last reader
            chain.Input.Readers.AddToEnd(new ModelBind(chain.InputType()));
            
            // Are there any Readers?
            chain.HasReaders();

            // Is there any output?
            chain.HasOutput();


            // Add the default Conneg policies to this chain
            // model binding, json, or xml in and json or xml out
            chain.ApplyConneg();

            // Manipulate an existing writer
            var writer = chain.Output.Writers.First();
            manipulateWriter(writer);

            // Remove all writers
            chain.Output.ClearAll();

            // Add the HtmlStringWriter
            chain.Output.AddHtml();

            // Add basic Json output
            chain.OutputJson();

            // Add basic Xml output
            chain.OutputXml();
        }

        private static void manipulateWriter(WriterNode writer)
        {
            writer.ReplaceWith(new WriteString());
            writer.MoveToFront();
        }

        // ENDSAMPLE

        // SAMPLE: add-conditions-to-writer
        // This method is just an example of how you can add 
        // runtime conditions to an existing WriterNode
        // hanging off of BehaviorChain.Output.Writers
        private static void addConditions(WriterNode node)
        {
            node.Condition<MyRuntimeCondition>();

            node.ConditionByModel<SomeResource>(x => x.Color == "Red");

            node.ConditionByService<Customer>(x => x.IsSpecial());
        }

        // IConditional services are resolved from the IoC
        // container, so you can declare dependencies
        // in the constructor function
        public class MyRuntimeCondition : IConditional
        {
            public bool ShouldExecute()
            {
                // apply your own runtime logic
                return false;
            }
        }
        // ENDSAMPLE

    }


    public class JsonInput : JsonMessage
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
        private readonly ICurrentHttpRequest _httpRequest;
        private readonly IFubuRequest _fubuRequest;

        public CorrectMimetypeForAjaxBehavior(ICurrentHttpRequest httpRequest, IFubuRequest fubuRequest)
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

    // SAMPLE: conneg-add-json-to-views
    public class AddJsonToViewsPolicy : Policy
    {
        public AddJsonToViewsPolicy()
        {
            // Assuming that you have *some* sort
            // of restriction on when and where
            // this policy applies
            Where.IsNotPartial().And.RespondsToHttpMethod("GET")
                .And.ChainMatches(chain => chain.GetRoutePattern().Contains("foo"));

            // Adds Json reading and writing to
            // the each chain that matches the Where filter
            // above
            ModifyBy(chain => {
                chain.Output.AddFormatter<JsonFormatter>();
                chain.Input.AddFormatter<JsonFormatter>();
            });
        }
    }
    // ENDSAMPLE

}