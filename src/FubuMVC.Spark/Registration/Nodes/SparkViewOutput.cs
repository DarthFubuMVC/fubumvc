using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.Tokenization.Model;

namespace FubuMVC.Spark.Registration.Nodes
{
    public class SparkViewOutput : OutputNode<SparkViewRenderer>
    {
        private readonly SparkFile _file;
        public SparkViewOutput(SparkFile file) { _file = file; }
        
        protected override void configureObject(ObjectDef def)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format("Spark View [{0}]", _file.RelativePath());
            }
        }
    }
}