using FubuMVC.Razor.Rendering;
using FubuMVC.Razor.RazorModel;

namespace FubuMVC.Razor.Registration
{
    public static class ViewDescriptorHelper
    {
        public static ViewDefinition ToViewDefinition(this ViewDescriptor viewDescriptor)
        {
            var normal = viewDescriptor.ToRazorViewDescriptor();
            var partial = viewDescriptor.ToPartialRazorViewDescriptor();
            return new ViewDefinition(normal, partial);
        }

        public static RazorViewDescriptor ToRazorViewDescriptor(this ViewDescriptor viewDescriptor)
        {
            return createRazorDescriptor(true, viewDescriptor);
        }

        public static RazorViewDescriptor ToPartialRazorViewDescriptor(this ViewDescriptor viewDescriptor)
        {
            return createRazorDescriptor(false, viewDescriptor);
        }


        private static RazorViewDescriptor createRazorDescriptor(bool useMaster, ViewDescriptor viewDescriptor)
        {
            var RazorDescriptor = new RazorViewDescriptor().AddTemplate(viewDescriptor.ViewPath);
            if (useMaster && viewDescriptor.Master != null)
            {
                appendMasterPage(RazorDescriptor, viewDescriptor.Master);
            }

            return RazorDescriptor;
        }

        private static void appendMasterPage(RazorViewDescriptor descriptor, ITemplate template)
        {
            if (template == null)
            {
                return;
            }
            descriptor.AddTemplate(template.ViewPath);
            var viewDescriptor = template.Descriptor as ViewDescriptor;
            if (viewDescriptor != null)
            {
                appendMasterPage(descriptor, viewDescriptor.Master);
            }
        }
    }
}