using System;
using FubuMVC.Core;

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

    public class XmlAndJsonMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    public class XmlAndJsonOnlyMessage : ConnegMessage
    {
        public Guid Id { get; set; }
    }

    


}