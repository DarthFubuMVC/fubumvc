﻿namespace FubuMVC.Core.View.Rendering
{
    public class RenderAction : IRenderAction
    {
        private readonly IViewFactory _viewFactory;

        public RenderAction(IViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public void Render()
        {
            _viewFactory.GetView().Render();
        }

        public void RenderPartial()
        {
            _viewFactory.GetPartialView().Render();
        }
    }
}