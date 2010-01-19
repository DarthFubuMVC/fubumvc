using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Spark.Web.FubuMVC
{
    public class SparkView : ISparkView
    {
        public void RenderView(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public Guid GeneratedViewId
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryGetViewData(string name, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
