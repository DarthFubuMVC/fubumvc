using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuTestApplication.ConnegActions
{
    public class ConnegController
    {
        [SymmetricJson]
        public SymmetricJson SymmetricSend(SymmetricJson message)
        {
            message.Name = "I was here";
            return message;
        }

        [AsymmetricJson]
        public AsymmetricJson AsymmetricSend(AsymmetricJson message)
        {
            return message;
        }

        public XmlJsonHtmlMessage Mixed(XmlJsonHtmlMessage message)
        {
            return message;
        }

        public XmlAndJsonOnlyMessage FormatterOnly(XmlAndJsonOnlyMessage message)
        {
            return message;
        }
    }

    public class ConnegMessageOutputter : BasicBehavior
    {
        private readonly IOutputWriter _writer;
        private readonly IFubuRequest _request;

        public ConnegMessageOutputter(IOutputWriter writer, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _writer = writer;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var message = _request.Find<ConnegMessage>().FirstOrDefault();
            _writer.Write("text/html", "Message:" + message.Id);

            return DoNext.Continue;
        }
    }

    public interface ConnegMessage
    {
        Guid Id { get; set; } 
    }

    public class SymmetricJson : ConnegMessage
    {
        public Guid Id { get; set;}
        public string Name { get; set; }
    }

    public class AsymmetricJson : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    public class XmlJsonHtmlMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    public class XmlAndJsonOnlyMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    


}