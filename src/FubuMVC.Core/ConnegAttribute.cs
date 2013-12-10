using System;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;

namespace FubuMVC.Core
{
    /// <summary>
    /// Explicitly applies the selected content negotiation policies and formats
    /// to this endpoint
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ConnegAttribute : ModifyChainAttribute
    {
        private FormatterOptions _formatters = FormatterOptions.Html | FormatterOptions.Json | FormatterOptions.Xml;

        public ConnegAttribute() : this(FormatterOptions.All)
        {
        }

        public ConnegAttribute(FormatterOptions formatters)
        {
            _formatters = formatters;
        }

        public FormatterOptions Formatters
        {
            get { return _formatters; }
            set { _formatters = value; }
        }

        public override void Alter(ActionCall call)
        {
            var chain = call.ParentChain();

            if (_formatters == FormatterOptions.All)
            {
                chain.Input.AllowHttpFormPosts = true;
                chain.UseFormatter<JsonFormatter>();
                chain.UseFormatter<XmlFormatter>();

                return;
            }


            if ((_formatters & FormatterOptions.Json) != 0)
            {
                chain.UseFormatter<JsonFormatter>();
            }

            if ((_formatters & FormatterOptions.Xml) != 0)
            {
                chain.UseFormatter<XmlFormatter>();
            }

            if ((_formatters & FormatterOptions.Html) != 0)
            {
                chain.Input.AllowHttpFormPosts = true;
            }
            else
            {
                chain.Input.AllowHttpFormPosts = false;
            }
        }
    }
}