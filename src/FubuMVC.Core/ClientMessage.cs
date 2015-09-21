using System;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core
{
    /// <summary>
    /// Marks this input or resource type as a 
    /// known message type for the client to facilitate
    /// data aggregation
    /// </summary>
    public abstract class ClientMessage
    {
        private readonly string _messageName;

        protected ClientMessage()
        {

        }

        protected ClientMessage(string messageName)
        {
            _messageName = messageName;
        }

        public string MessageName()
        {
            return _messageName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ClientMessageAttribute : Attribute
    {
        private readonly string _messageName;

        public ClientMessageAttribute()
        {
        }

        public ClientMessageAttribute(string messageName)
        {
            _messageName = messageName;
        }

        public string MessageName
        {
            get { return _messageName; }
        }
    }
}