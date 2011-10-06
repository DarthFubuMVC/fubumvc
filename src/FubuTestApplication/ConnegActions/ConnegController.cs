using System;

namespace FubuTestApplication.ConnegActions
{
    public class ConnegController
    {
        public SymmetricJson Send(SymmetricJson message)
        {
            return message;
        }

        public AsymmetricJson Send(AsymmetricJson message)
        {
            return message;
        }
    }

    public abstract class ConnegMessage
    {
        Guid Id { get; set; } 
    }

    public class SymmetricJson : ConnegMessage
    {
    }

    public class AsymmetricJson : ConnegMessage
    {
    }

    public class XmlAndJsonMessage : ConnegMessage
    {
        
    }

    public class XmlAndJsonOnlyMessage : ConnegMessage
    {
        
    }

    


}