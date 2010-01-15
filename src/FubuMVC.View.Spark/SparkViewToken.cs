using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;

namespace FubuMVC.View.Spark
{
    public class SparkViewToken : IViewToken
    {
        public BehaviorNode ToBehavioralNode()
        {
            throw new NotImplementedException();
        }

        public Type ViewModelType
        {
            get { throw new NotImplementedException(); } }

        public string Namespace
        {
            get { throw new NotImplementedException(); } }

        public string Name
        {
            get { throw new NotImplementedException(); } }
    }
}