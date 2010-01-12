using System;
using System.Web.UI;
using FubuMVC.Core.View.WebForms;
using StructureMap;

namespace FubuMVC.StructureMap
{
    public class StructureMapWebFormsControlBuilder : WebFormsControlBuilder
    {
        private readonly IContainer _container;

        public StructureMapWebFormsControlBuilder(IContainer container)
        {
            _container = container;
        }

        public override Control LoadControlFromVirtualPath(string virtualPath, Type mustImplementInterfaceType)
        {
            Control control = base.LoadControlFromVirtualPath(virtualPath, mustImplementInterfaceType);
            _container.BuildUp(control);

            return control;
        }
    }
}