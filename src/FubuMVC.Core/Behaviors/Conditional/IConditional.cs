using System;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public interface IConditional
    {
        bool ShouldExecute();
    }

    public class Always : IConditional
    {
        private Always()
        {
        }

        public bool ShouldExecute()
        {
            return true;
        }

        public static readonly Always Flyweight = new Always();
    }
}