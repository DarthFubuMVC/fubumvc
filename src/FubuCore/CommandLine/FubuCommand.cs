using System;

namespace FubuCore.CommandLine
{
    public abstract class FubuCommand<T> : IFubuCommand<T>
    {
        public Type InputType
        {
            get
            {
                return typeof (T);
            }
        }

        public void Execute(object input)
        {
            Execute((T)input);
        }

        public abstract void Execute(T input);
    }
}