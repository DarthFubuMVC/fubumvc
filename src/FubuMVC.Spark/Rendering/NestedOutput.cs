using System;
using System.IO;

namespace FubuMVC.Spark.Rendering
{
    public class NestedOutput
    {
        private Func<IFubuSparkView> _view;
        private bool _isActive;

        public void SetView(Func<IFubuSparkView> view)
        {
            _isActive = true;
            _view = view;
        }

        public IFubuSparkView View
        {
            get { return _view(); }
        }
		
        public bool IsActive()
        {
            return _isActive;
        }
    }
}