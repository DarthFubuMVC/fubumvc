using FubuCore;
using FubuMVC.Core.View.Model;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.RazorModel;
using RazorEngine.Templating;

namespace FubuMVC.Razor.Rendering
{
    public class LayoutActivation : BasicViewModifier<IFubuRazorView>
    {
        public override IFubuRazorView Modify(IFubuRazorView view)
        {
            if (view.Layout != null)
            {
                view.Layout.As<IFubuRazorView>().ServiceLocator = view.ServiceLocator;
                Modify(view.Layout.As<IFubuRazorView>());
            }
            return view;
        }
    }

    public class PartialRendering : BasicViewModifier<IFubuRazorView>
    {
        private readonly ISharedTemplateLocator<IRazorTemplate> _locator;
        private readonly IFubuTemplateService _templateService;

        public PartialRendering(ISharedTemplateLocator<IRazorTemplate> locator, IFubuTemplateService templateService)
        {
            _locator = locator;
            _templateService = templateService;
        }

        public override IFubuRazorView Modify(IFubuRazorView view)
        {
            var temporary = view;
            while(temporary != null)
            {
                temporary.RenderPartialWith = name =>
                {
                    var template = _locator.LocatePartial(name, view.OriginTemplate);
                    var partialView = _templateService.GetView(template.Descriptor.As<ViewDescriptor<IRazorTemplate>>());

                    var modifier = view.Get<IViewModifierService<IFubuRazorView>>();
                    partialView = modifier.Modify(partialView);

                    var partialRendered = partialView.Run(new ExecuteContext());
                    return new TemplateWriter(x => x.Write(partialRendered));
                };
                temporary = temporary.Layout.As<IFubuRazorView>();
            }
            return view;
        }
    }

    public class FubuPartialRendering : IViewModifier<IFubuRazorView>
    {
        private bool _shouldInvokeAsPartial;

        public bool Applies(IFubuRazorView view)
        {
            if (_shouldInvokeAsPartial)
            {
                return true;
            }

            _shouldInvokeAsPartial = true;
            return false;
        }

        public IFubuRazorView Modify(IFubuRazorView view)
        {
            view.NoLayout();
            return view;
        }
    }
}