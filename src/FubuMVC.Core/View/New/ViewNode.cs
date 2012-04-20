using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View.New
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

        public override IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }
    }
}