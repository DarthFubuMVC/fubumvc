using System.Collections.Generic;
namespace FubuMVC.Spark.Rendering
{
    public interface IRenderAction
    {
        void Render();
    }
	
	// Note: Perhaps we could use SparkViewDecorator 
	// to handle "set asside" / "restore previous state of "view content" + proactive disposal.
	
    public class NestedRenderAction : IRenderAction
    {				
        private readonly IViewFactory _viewFactory;
        private readonly NestedOutput _nestedOutput;

        public NestedRenderAction(IViewFactory viewFactory, NestedOutput nestedOutput)
        {
            _viewFactory = viewFactory;
            _nestedOutput = nestedOutput;
        }
		
		public void Render()
        {            
			var view = _viewFactory.GetView();
			var outerView = _nestedOutput.View;
            view.RenderView(outerView.Output);
        }
    }

    public class DefaultRenderAction : IRenderAction
    {
        private readonly IViewFactory _viewFactory;
        private readonly ViewOutput _viewOutput;

        public DefaultRenderAction(IViewFactory viewFactory, ViewOutput viewOutput)
        {
            _viewFactory = viewFactory;
            _viewOutput = viewOutput;
        }

        public void Render()
        {
            var view = _viewFactory.GetView();            
			view.RenderView(_viewOutput);
			
			// proactively dispose named content. pools spoolwriter pages. avoids finalizers.
	        view.Content.Values.Each(c => c.Close());
    	    view.Content.Clear();
        }
    }
}