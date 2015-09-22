using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class QueueMessageResourceNotFoundAttribute : ModifyChainAttribute
    {
        public override void Alter(ActionCallBase call)
        {
            call.ParentChain().Output.As<OutputNode>().UseForResourceNotFound<QueueMessageNotFoundWriter>();
        }
    }

    public class QueueMessageNotFoundWriter : IResourceNotFoundHandler
    {
        private readonly IOutputWriter _outputWriter;
        private readonly IFubuRequest _fubuRequest;

        public QueueMessageNotFoundWriter(IOutputWriter outputWriter, IFubuRequest fubuRequest)
        {
            _outputWriter = outputWriter;
            _fubuRequest = fubuRequest;
        }

        public void HandleResourceNotFound<T>()
        {
            var messageNotFound = _fubuRequest.Get<QueueMessageNotFound>();
            _outputWriter.Write(MimeType.Html,
                "Message with Id {0} cannot be found in queue {1}; message may have moved out of this queue."
                .ToFormat(messageNotFound.Id.ToString(), messageNotFound.QueueName));
        }
    }

    public class QueueMessageNotFound
    {
        public MessageId Id { get; set; }
        public string QueueName { get; set; }
    }
}