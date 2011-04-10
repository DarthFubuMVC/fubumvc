using System;

namespace FubuMVC.Core.Diagnostics
{
    public class WannaKillAttribute : Attribute
    {
        private readonly string _message;

        public WannaKillAttribute()
        {
        }

        public WannaKillAttribute(string message)
        {
            _message = message;
        }

        public string Message
        {
            get { return _message; }
        }
    }
}