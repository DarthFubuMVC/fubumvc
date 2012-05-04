using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;
using FubuCore;

namespace FubuMVC.Core.View
{
    public class ViewNode : WriterNode
    {
        private readonly IViewToken _token;

        public ViewNode(IViewToken token)
        {
            _token = token;
        }

        public override Type ResourceType
        {
            get { return _token.ViewModel; }
        }

        protected override ObjectDef toWriterDef()
        {
            var def = new ObjectDef(typeof(ViewWriter<>), ResourceType);

            def.DependencyByType<IViewFactory>(_token.ToViewFactoryObjectDef());

            return def;
        }

        public IViewToken View
        {
            get { return _token; }
        }

        public override IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }

        public override string ToString()
        {
            return "View {0}, Condition {1}".ToFormat(_token.Name(), ConditionType.Name);
        }
    }
}